import api from '@/api/client';
import { setRefreshCookie, tokenStorage } from '@/api/token';
import type { ApiResponse, LoginResponse } from '@/types';

export async function login(email: string, password: string): Promise<LoginResponse> {
  const response = await api.post<ApiResponse<LoginResponse>>('/v1/auth/login', { email, password });
  const tokens = response.data.data;
  tokenStorage.setTokens(tokens);
  return tokens;
}

export async function register(email: string, password: string): Promise<LoginResponse> {
  const response = await api.post<ApiResponse<LoginResponse>>('/v1/auth/register', { email, password });
  const tokens = response.data.data;
  tokenStorage.setTokens(tokens);
  return tokens;
}

export async function verifyAccount(code: string): Promise<void> {
  await api.post('/v1/auth/verify', { code });
}

export async function logout(): Promise<void> {
  try {
    await api.post('/v1/auth/logout');
  } finally {
    tokenStorage.clear();
  }
}

export async function refreshWithStoredCookie(): Promise<LoginResponse | null> {
  const refreshToken = tokenStorage.getRefreshToken();
  if (!refreshToken) return null;

  setRefreshCookie(refreshToken);
  const response = await api.post<ApiResponse<LoginResponse>>('/v1/auth/refresh', {});
  const tokens = response.data.data;
  tokenStorage.setTokens(tokens);
  return tokens;
}
