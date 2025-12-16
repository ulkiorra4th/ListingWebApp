import api from '@/api/client';
import type { ApiResponse, Item, ItemEntry } from '@/types';

export type CreateItemPayload = {
  name: string;
  rarity: number;
  basePrice: number;
  description?: string;
  iconKey?: string;
  releaseDate: string;
  isTrading: boolean;
};

export type CreateItemEntryPayload = {
  ownerId: string;
  itemTypeId: string;
  pseudonym?: string;
};

export async function getItem(id: string): Promise<Item> {
  const response = await api.get<ApiResponse<Item>>(`/v1/items/${id}`);
  return response.data.data;
}

export async function createItem(payload: CreateItemPayload): Promise<string> {
  const response = await api.post<ApiResponse<string>>('/v1/items', payload);
  return response.data.data;
}

export async function updateItemIcon(itemId: string, file: File): Promise<void> {
  const formData = new FormData();
  formData.append('file', file);
  await api.patch(`/v1/items/${itemId}/icon`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
}

export async function getItemEntry(id: string): Promise<ItemEntry> {
  const response = await api.get<ApiResponse<ItemEntry>>(`/v1/item-entries/${id}`);
  return response.data.data;
}

export async function createItemEntry(payload: CreateItemEntryPayload): Promise<string> {
  const response = await api.post<ApiResponse<string>>('/v1/item-entries', payload);
  return response.data.data;
}
