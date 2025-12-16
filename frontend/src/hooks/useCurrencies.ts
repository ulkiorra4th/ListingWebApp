import { useEffect, useState } from 'react';
import type { Currency } from '@/types';
import { fetchCurrencies } from '@/services/currenciesService';

export function useCurrencies() {
  const [currencies, setCurrencies] = useState<Currency[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setLoading(true);
    fetchCurrencies()
      .then((data) => setCurrencies(data))
      .finally(() => setLoading(false));
  }, []);

  return { currencies, loading };
}
