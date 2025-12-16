import api from '@/api/client';
import type { ApiResponse, CreateListingPayload, Listing } from '@/types';

export async function getListing(id: string): Promise<Listing> {
  const response = await api.get<ApiResponse<Listing>>(`/v1/listings/${id}`);
  return response.data.data;
}

export async function createListing(payload: CreateListingPayload): Promise<string> {
  const response = await api.post<ApiResponse<string>>('/v1/listings', {
    sellerId: payload.sellerId,
    itemEntryId: payload.itemEntryId,
    currencyCode: payload.currencyCode,
    priceAmount: payload.priceAmount,
    status: payload.status ?? 2, // Approved by default
  });
  return response.data.data;
}

export async function updateListingStatus(id: string, status: number): Promise<void> {
  await api.patch(`/v1/listings/${id}/status`, status);
}
