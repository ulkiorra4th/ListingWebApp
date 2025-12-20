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
import { createProfile as createProfileApi, getProfiles, uploadProfileIcon, getProfileIconUrl } from '@/services/profilesService';
import type { CreateProfilePayload } from '@/services/profilesService';
import { createWallet, getWallet } from '@/services/walletService';
import { tokenStorage } from '@/api/token';

export type AuthStep = 'login' | 'verify' | 'profile' | 'avatar' | 'wallet' | 'ready';

export type AuthContextValue = {
  account: Account | null;
  profile: Profile | null;
  isAuthenticated: boolean;
  loading: boolean;
  authStep: AuthStep;
  accountId: string | null;
  walletReady: boolean;
  walletCurrency: string | null;
  profileAvatarUrl: string | null;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string) => Promise<void>;
  verify: (code: string) => Promise<void>;
  createProfile: (payload: CreateProfilePayload) => Promise<void>;
  createWallet: (currencyCode: string) => Promise<void>;
  uploadAvatar: (file: File) => Promise<void>;
  skipAvatar: () => void;
  refresh: () => Promise<void>;
  logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);
const WALLET_CURRENCY_KEY = 'lw_wallet_currency';

function isVerified(account?: Account | null) {
  if (!account) return false;
  if (account.status === AccountStatus.Verified) return true;
  if (typeof account.status === 'number' && account.status === 1) return true; // backend может отдавать enum числом
  if (account.status === 'Verified') return true;
  return false;
}

function deriveStep(account: Account | null, profiles: Profile[], avatarSkipped: boolean, walletReady: boolean): AuthStep {
  if (!account) return 'login';
  if (!isVerified(account)) return 'verify';
  if (!profiles.length) return 'profile';
  if (!avatarSkipped && !profiles[0].iconKey) return 'avatar';
  if (!walletReady && !account) return 'wallet';
  return 'ready';
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [account, setAccount] = useState<Account | null>(null);
  const [profiles, setProfiles] = useState<Profile[]>([]);
  const [accountId, setAccountId] = useState<string | null>(tokenStorage.getAccountId());
  const [loading, setLoading] = useState(false);
  const [avatarSkipped, setAvatarSkipped] = useState(false);
  const [walletReady, setWalletReady] = useState(false);
  const [walletCurrency, setWalletCurrency] = useState<string | null>(() => localStorage.getItem(WALLET_CURRENCY_KEY));
  const [profileAvatarUrl, setProfileAvatarUrl] = useState<string | null>(null);

  const profile = profiles[0] ?? null;
  const isAuthenticated = Boolean(accountId && tokenStorage.getAccessToken());
  const authStep = deriveStep(account, profiles, avatarSkipped, walletReady);

  const rememberWalletCurrency = useCallback((currencyCode: string) => {
    localStorage.setItem(WALLET_CURRENCY_KEY, currencyCode);
    setWalletCurrency(currencyCode);
  }, []);

  const clearWalletInfo = useCallback(() => {
    localStorage.removeItem(WALLET_CURRENCY_KEY);
    setWalletCurrency(null);
    setWalletReady(false);
  }, []);

  const resolveAvatar = useCallback(
    async (id: string, profs: Profile[]) => {
      const mainProfile = profs[0];
      if (!mainProfile?.iconKey) {
        setProfileAvatarUrl(null);
        return;
      }

      try {
        const url = await getProfileIconUrl(id, mainProfile.id);
        setProfileAvatarUrl(url);
      } catch {
        setProfileAvatarUrl(null);
      }
    },
    [],
  );

  const ensureWallet = useCallback(
    async (id: string) => {
      const code = localStorage.getItem(WALLET_CURRENCY_KEY);
      if (!code) {
        setWalletReady(false);
        return;
      }

      try {
        await getWallet(id, code);
        setWalletReady(true);
        setWalletCurrency(code);
      } catch {
        setWalletReady(false);
      }
    },
    [],
  );

  const loadAccountData = useCallback(
    async (id: string) => {
      const [acc, prof] = await Promise.all([getAccountById(id), getProfiles(id)]);
      setAccount(acc);
      setProfiles(prof);
      setAvatarSkipped(false);
      await resolveAvatar(id, prof);
      await ensureWallet(id);
    },
    [ensureWallet, resolveAvatar],
  );

  const refresh = useCallback(async () => {
    const storedId = tokenStorage.getAccountId();
    const hasToken = tokenStorage.getAccessToken();
    if (!storedId || !hasToken) {
      setAccount(null);
      setProfiles([]);
      setAccountId(null);
      setAvatarSkipped(false);
      clearWalletInfo();
      return;
    }
    setAccountId(storedId);
    await loadAccountData(storedId);
  }, [clearWalletInfo, loadAccountData]);

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

  const handleCreateWallet = useCallback(
    async (currencyCode: string) => {
      if (!accountId) return;
      setLoading(true);
      try {
        await createWallet(accountId, currencyCode);
        rememberWalletCurrency(currencyCode);
        setWalletReady(true);
      } finally {
        setLoading(false);
      }
    },
    [accountId, rememberWalletCurrency],
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
      clearWalletInfo();
      setProfileAvatarUrl(null);
      setLoading(false);
    }
  }, [clearWalletInfo]);

  const value = useMemo<AuthContextValue>(
    () => ({
      account,
      profile,
      isAuthenticated,
      loading,
      authStep,
      accountId,
      walletReady,
      walletCurrency,
      login: handleLogin,
      register: handleRegister,
      verify: handleVerify,
      createProfile: handleCreateProfile,
      createWallet: handleCreateWallet,
      profileAvatarUrl,
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
      walletReady,
      walletCurrency,
      handleLogin,
      handleRegister,
      handleVerify,
      handleCreateProfile,
      handleCreateWallet,
      profileAvatarUrl,
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
