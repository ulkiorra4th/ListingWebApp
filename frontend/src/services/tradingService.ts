import api from '@/api/client';
import type { ApiResponse, TradeTransaction } from '@/types';

export async function purchaseListing(listingId: string): Promise<TradeTransaction> {
  const response = await api.post<ApiResponse<TradeTransaction>>(`/v1/listings/${listingId}/purchase`);
  return response.data.data;
}
