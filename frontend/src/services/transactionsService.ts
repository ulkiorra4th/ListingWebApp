import api from '@/api/client';
import type { ApiResponse, TradeTransaction } from '@/types';

export async function getTransaction(id: string): Promise<TradeTransaction> {
  const response = await api.get<ApiResponse<TradeTransaction>>(`/v1/trade-transactions/${id}`);
  return response.data.data;
}
