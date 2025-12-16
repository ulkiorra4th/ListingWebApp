import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react';
import type { Account, Profile } from '@/types';
import { AccountStatus } from '@/types';
import { login, logout as logoutApi, register, verifyAccount } from '@/services/authService';
import { getAccountById } from '@/services/accountsService';
import { createProfile as createProfileApi, getProfiles, uploadProfileIcon } from '@/services/profilesService';
import type { CreateProfilePayload } from '@/services/profilesService';
import { tokenStorage } from '@/api/token';

export type AuthStep = 'login' | 'verify' | 'profile' | 'avatar' | 'ready';

export type AuthContextValue = {
  account: Account | null;
  profile: Profile | null;
  isAuthenticated: boolean;
  loading: boolean;
  authStep: AuthStep;
  accountId: string | null;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string) => Promise<void>;
  verify: (code: string) => Promise<void>;
  createProfile: (payload: CreateProfilePayload) => Promise<void>;
  uploadAvatar: (file: File) => Promise<void>;
  skipAvatar: () => void;
  refresh: () => Promise<void>;
  logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function isVerified(account?: Account | null) {
  if (!account) return false;
  if (account.status === AccountStatus.Verified) return true;
  if (typeof account.status === 'number' && account.status === 1) return true; // backend может отдавать enum числом
  if (account.status === 'Verified') return true;
  return false;
}

function deriveStep(account: Account | null, profiles: Profile[], avatarSkipped: boolean): AuthStep {
  if (!account) return 'login';
  if (!isVerified(account)) return 'verify';
  if (!profiles.length) return 'profile';
  if (!avatarSkipped && !profiles[0].iconKey) return 'avatar';
  return 'ready';
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [account, setAccount] = useState<Account | null>(null);
  const [profiles, setProfiles] = useState<Profile[]>([]);
  const [accountId, setAccountId] = useState<string | null>(tokenStorage.getAccountId());
  const [loading, setLoading] = useState(false);
  const [avatarSkipped, setAvatarSkipped] = useState(false);

  const profile = profiles[0] ?? null;
  const isAuthenticated = Boolean(accountId && tokenStorage.getAccessToken());
  const authStep = deriveStep(account, profiles, avatarSkipped);

  const loadAccountData = useCallback(
    async (id: string) => {
      const [acc, prof] = await Promise.all([getAccountById(id), getProfiles(id)]);
      setAccount(acc);
      setProfiles(prof);
      setAvatarSkipped(false);
    },
    [],
  );

  const refresh = useCallback(async () => {
    const storedId = tokenStorage.getAccountId();
    const hasToken = tokenStorage.getAccessToken();
    if (!storedId || !hasToken) {
      setAccount(null);
      setProfiles([]);
      setAccountId(null);
      setAvatarSkipped(false);
      return;
    }
    setAccountId(storedId);
    await loadAccountData(storedId);
  }, [loadAccountData]);

  useEffect(() => {
    refresh().catch(() => {
      // ignore bootstrap errors
    });
  }, [refresh]);

  const handleLogin = useCallback(
    async (email: string, password: string) => {
      setLoading(true);
      try {
        const tokens = await login(email, password);
        setAccountId(tokens.accountId);
        await loadAccountData(tokens.accountId);
      } finally {
        setLoading(false);
      }
    },
    [loadAccountData],
  );

  const handleRegister = useCallback(
    async (email: string, password: string) => {
      setLoading(true);
      try {
        const tokens = await register(email, password);
        setAccountId(tokens.accountId);
        await loadAccountData(tokens.accountId);
      } finally {
        setLoading(false);
      }
    },
    [loadAccountData],
  );

  const handleVerify = useCallback(
    async (code: string) => {
      if (!accountId) return;
      setLoading(true);
      try {
        await verifyAccount(code);
        await loadAccountData(accountId);
      } finally {
        setLoading(false);
      }
    },
    [accountId, loadAccountData],
  );

  const handleCreateProfile = useCallback(
    async (payload: CreateProfilePayload) => {
      if (!accountId) return;
      setLoading(true);
      try {
        await createProfileApi(accountId, payload);
        await loadAccountData(accountId);
      } finally {
        setLoading(false);
      }
    },
    [accountId, loadAccountData],
  );

  const handleUploadAvatar = useCallback(
    async (file: File) => {
      if (!accountId || !profile?.id) return;
      setLoading(true);
      try {
        await uploadProfileIcon(accountId, profile.id, file);
        await loadAccountData(accountId);
      } finally {
        setLoading(false);
      }
    },
    [accountId, profile?.id, loadAccountData],
  );

  const handleSkipAvatar = useCallback(() => {
    setAvatarSkipped(true);
  }, []);

  const handleLogout = useCallback(async () => {
    setLoading(true);
    try {
      await logoutApi();
    } finally {
      setAccount(null);
      setProfiles([]);
      setAccountId(null);
      setAvatarSkipped(false);
      setLoading(false);
    }
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      account,
      profile,
      isAuthenticated,
      loading,
      authStep,
      accountId,
      login: handleLogin,
      register: handleRegister,
      verify: handleVerify,
      createProfile: handleCreateProfile,
      uploadAvatar: handleUploadAvatar,
      skipAvatar: handleSkipAvatar,
      refresh,
      logout: handleLogout,
    }),
    [
      account,
      profile,
      isAuthenticated,
      loading,
      authStep,
      accountId,
      handleLogin,
      handleRegister,
      handleVerify,
      handleCreateProfile,
      handleUploadAvatar,
      handleSkipAvatar,
      refresh,
      handleLogout,
    ],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error('useAuth must be used inside AuthProvider');
  }
  return ctx;
}
