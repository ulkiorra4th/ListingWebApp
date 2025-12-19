import { useCallback, useEffect, useState } from 'react';
import type { Currency } from '@/types';
import { fetchCurrencies } from '@/services/currenciesService';

export function useCurrencies(reloadKey?: unknown) {
  const [currencies, setCurrencies] = useState<Currency[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await fetchCurrencies();
      setCurrencies(data);
    } catch (err) {
      setCurrencies([]);
      setError((err as Error).message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    refresh();
  }, [refresh, reloadKey]);

  return { currencies, loading, error, refresh };
}
