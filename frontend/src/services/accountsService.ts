import api from '@/api/client';
import type { Account, AccountStatus, ApiResponse } from '@/types';

export async function getAccountById(id: string): Promise<Account> {
  const response = await api.get<ApiResponse<Account>>(`/v1/accounts/${id}`);
  return response.data.data;
}

export async function updateAccountStatus(id: string, status: AccountStatus): Promise<void> {
  await api.patch(`/v1/accounts/${id}/status`, { status });
}

export async function deleteAccount(id: string): Promise<void> {
  await api.delete(`/v1/accounts/${id}`);
}
