import api from '@/api/client';
import type { ApiResponse, Wallet } from '@/types';

export async function getWallet(accountId: string, currencyCode: string): Promise<Wallet> {
  const response = await api.get<ApiResponse<Wallet>>(`/v1/accounts/${accountId}/wallets/${currencyCode}`);
  return response.data.data;
}

export async function createWallet(accountId: string, currencyCode: string): Promise<void> {
  await api.post(`/v1/accounts/${accountId}/wallets/${currencyCode}`);
}

export async function creditWallet(accountId: string, currencyCode: string, amount: number): Promise<void> {
  await api.post(`/v1/accounts/${accountId}/wallets/${currencyCode}/credit`, {
    amount,
    transactionDate: new Date().toISOString(),
  });
}

export async function debitWallet(accountId: string, currencyCode: string, amount: number): Promise<void> {
  await api.post(`/v1/accounts/${accountId}/wallets/${currencyCode}/debit`, {
    amount,
    transactionDate: new Date().toISOString(),
  });
}

export async function upsertWallet(
  accountId: string,
  currencyCode: string,
  payload: { balance: number; lastTransactionDate?: string | null; isActive: boolean },
): Promise<void> {
  await api.put(`/v1/accounts/${accountId}/wallets/${currencyCode}`, {
    balance: payload.balance,
    lastTransactionDate: payload.lastTransactionDate,
    isActive: payload.isActive,
  });
}
