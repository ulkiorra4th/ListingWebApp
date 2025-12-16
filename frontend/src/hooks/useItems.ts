import { useCallback, useState } from 'react';
import type { Item, ItemEntry } from '@/types';
import { createItemEntry, getItem, getItemEntry } from '@/services/itemsService';

export function useItems() {
  const [item, setItem] = useState<Item | null>(null);
  const [entry, setEntry] = useState<ItemEntry | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchItem = useCallback(async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      const data = await getItem(id);
      setItem(data);
      return data;
    } catch (err) {
      setError((err as Error).message);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const fetchEntry = useCallback(async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      const data = await getItemEntry(id);
      setEntry(data);
      return data;
    } catch (err) {
      setError((err as Error).message);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const createEntry = useCallback(async (ownerId: string, itemTypeId: string, pseudonym?: string) => {
    setLoading(true);
    setError(null);
    try {
      const id = await createItemEntry({ ownerId, itemTypeId, pseudonym });
      await fetchEntry(id);
      return id;
    } catch (err) {
      setError((err as Error).message);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [fetchEntry]);

  return { item, entry, loading, error, fetchItem, fetchEntry, createEntry };
}
