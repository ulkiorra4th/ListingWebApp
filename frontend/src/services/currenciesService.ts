import api from '@/api/client';
import type { ApiResponse, Currency } from '@/types';

export async function fetchCurrencies(): Promise<Currency[]> {
  const response = await api.get<ApiResponse<Currency[]>>('/v1/currencies');
  return response.data.data;
}

export async function getCurrency(code: string): Promise<Currency> {
  const response = await api.get<ApiResponse<Currency>>(`/v1/currencies/${code}`);
  return response.data.data;
}
