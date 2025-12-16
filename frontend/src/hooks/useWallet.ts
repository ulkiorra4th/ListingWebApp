import { useCallback, useEffect, useState } from 'react';
import type { Wallet } from '@/types';
import { creditWallet, debitWallet, getWallet } from '@/services/walletService';

export function useWallet(accountId?: string | null, currencyCode?: string | null) {
  const [wallet, setWallet] = useState<Wallet | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    if (!accountId || !currencyCode) return;
    setLoading(true);
    setError(null);
    try {
      const data = await getWallet(accountId, currencyCode);
      setWallet(data);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoading(false);
    }
  }, [accountId, currencyCode]);

  useEffect(() => {
    refresh();
  }, [refresh]);

  const credit = useCallback(
    async (amount: number) => {
      if (!accountId || !currencyCode) return;
      setLoading(true);
      setError(null);
      try {
        await creditWallet(accountId, currencyCode, amount);
        await refresh();
      } catch (err) {
        setError((err as Error).message);
      } finally {
        setLoading(false);
      }
    },
    [accountId, currencyCode, refresh],
  );

  const debit = useCallback(
    async (amount: number) => {
      if (!accountId || !currencyCode) return;
      setLoading(true);
      setError(null);
      try {
        await debitWallet(accountId, currencyCode, amount);
        await refresh();
      } catch (err) {
        setError((err as Error).message);
      } finally {
        setLoading(false);
      }
    },
    [accountId, currencyCode, refresh],
  );

  return { wallet, loading, error, credit, debit, refresh };
}
