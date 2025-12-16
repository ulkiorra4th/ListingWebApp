import { useCallback, useState } from 'react';
import type { TradeTransaction } from '@/types';
import { getTransaction } from '@/services/transactionsService';

export function useTransactions() {
  const [transaction, setTransaction] = useState<TradeTransaction | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchTransaction = useCallback(async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      const data = await getTransaction(id);
      setTransaction(data);
      return data;
    } catch (err) {
      setError((err as Error).message);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  return { transaction, loading, error, fetchTransaction };
}
