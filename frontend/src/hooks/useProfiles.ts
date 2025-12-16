import { useCallback, useEffect, useState } from 'react';
import type { Profile } from '@/types';
import { createProfile, deleteProfile, getProfiles, updateProfile } from '@/services/profilesService';
import type { CreateProfilePayload, UpdateProfilePayload } from '@/services/profilesService';

export function useProfiles(accountId?: string | null) {
  const [profiles, setProfiles] = useState<Profile[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    if (!accountId) return;
    setLoading(true);
    setError(null);
    try {
      const data = await getProfiles(accountId);
      setProfiles(data);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoading(false);
    }
  }, [accountId]);

  useEffect(() => {
    refresh();
  }, [refresh]);

  const create = useCallback(
    async (payload: CreateProfilePayload) => {
      if (!accountId) return;
      setLoading(true);
      try {
        const profile = await createProfile(accountId, payload);
        setProfiles((prev) => [profile, ...prev]);
      } finally {
        setLoading(false);
      }
    },
    [accountId],
  );

  const update = useCallback(
    async (id: string, payload: UpdateProfilePayload) => {
      if (!accountId) return;
      setLoading(true);
      try {
        const profile = await updateProfile(accountId, id, payload);
        setProfiles((prev) => prev.map((p) => (p.id === id ? profile : p)));
      } finally {
        setLoading(false);
      }
    },
    [accountId],
  );

  const remove = useCallback(
    async (id: string) => {
      if (!accountId) return;
      setLoading(true);
      try {
        await deleteProfile(accountId, id);
        setProfiles((prev) => prev.filter((p) => p.id !== id));
      } finally {
        setLoading(false);
      }
    },
    [accountId],
  );

  return { profiles, loading, error, create, update, remove, refresh };
}
