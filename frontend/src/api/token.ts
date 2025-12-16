import type { AuthTokens } from '@/types';

const ACCESS_TOKEN_KEY = 'lw_access_token';
const REFRESH_TOKEN_KEY = 'lw_refresh_token';
const ACCOUNT_ID_KEY = 'lw_account_id';
const THIRTY_DAYS_SECONDS = 60 * 60 * 24 * 30;

export const tokenStorage = {
  getAccessToken: () => localStorage.getItem(ACCESS_TOKEN_KEY),
  getRefreshToken: () => localStorage.getItem(REFRESH_TOKEN_KEY),
  getAccountId: () => localStorage.getItem(ACCOUNT_ID_KEY),
  setTokens: (tokens: AuthTokens) => {
    localStorage.setItem(ACCESS_TOKEN_KEY, tokens.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);
    localStorage.setItem(ACCOUNT_ID_KEY, tokens.accountId);
    setRefreshCookie(tokens.refreshToken);
  },
  setAccessToken: (accessToken: string) => {
    localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  },
  clear: () => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(ACCOUNT_ID_KEY);
    clearRefreshCookie();
  },
};

export function setRefreshCookie(token: string) {
  if (!token) return;
  document.cookie = `refreshToken=${token}; Max-Age=${THIRTY_DAYS_SECONDS}; Path=/; SameSite=Lax`;
  localStorage.setItem(REFRESH_TOKEN_KEY, token);
}

export function clearRefreshCookie() {
  document.cookie = 'refreshToken=; Max-Age=0; Path=/; SameSite=Lax';
}

export function getCookie(name: string) {
  const parts = document.cookie.split(';').map((part) => part.trim());
  for (const part of parts) {
    if (part.startsWith(`${name}=`)) {
      return part.split('=')[1];
    }
  }
  return null;
}
