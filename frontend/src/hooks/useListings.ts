import { useCallback, useState } from 'react';
import type { CreateListingPayload, Listing, TradeTransaction } from '@/types';
import { createListing, getListing } from '@/services/listingsService';
import { purchaseListing } from '@/services/tradingService';

export function useListings() {
  const [listing, setListing] = useState<Listing | null>(null);
  const [purchaseResult, setPurchaseResult] = useState<TradeTransaction | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchListing = useCallback(async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      const data = await getListing(id);
      setListing(data);
      return data;
    } catch (err) {
      setError((err as Error).message);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const create = useCallback(async (payload: CreateListingPayload) => {
    setLoading(true);
    setError(null);
    try {
      const id = await createListing(payload);
      const created = await fetchListing(id);
      return created;
    } catch (err) {
      setError((err as Error).message);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [fetchListing]);

  const purchase = useCallback(async (listingId: string) => {
    setLoading(true);
    setError(null);
    try {
      const tx = await purchaseListing(listingId);
      setPurchaseResult(tx);
      await fetchListing(listingId);
      return tx;
    } catch (err) {
      setError((err as Error).message);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [fetchListing]);

  return {
    listing,
    purchaseResult,
    loading,
    error,
    fetchListing,
    create,
    purchase,
  };
}
