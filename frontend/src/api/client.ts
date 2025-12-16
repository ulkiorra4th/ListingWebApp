import axios, { AxiosError, InternalAxiosRequestConfig } from 'axios';
import type { ApiResponse, LoginResponse } from '@/types';
import { getCookie, setRefreshCookie, tokenStorage } from './token';

const baseURL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080/api';

const api = axios.create({
  baseURL,
  withCredentials: true,
});

const refreshClient = axios.create({
  baseURL,
  withCredentials: true,
});

type RequestConfig = InternalAxiosRequestConfig & { _retry?: boolean };

let isRefreshing = false;
let refreshSubscribers: Array<(token: string | null) => void> = [];

api.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();
  if (token) {
    // eslint-disable-next-line no-param-reassign
    config.headers = {
      ...config.headers,
      Authorization: `Bearer ${token}`,
    };
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const status = error.response?.status;
    const originalRequest = error.config as RequestConfig;

    if (status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      const newToken = await refreshAccessToken();

      if (newToken) {
        originalRequest.headers = {
          ...originalRequest.headers,
          Authorization: `Bearer ${newToken}`,
        };
        return api(originalRequest);
      }
    }

    return Promise.reject(error);
  },
);

async function refreshAccessToken(): Promise<string | null> {
  if (isRefreshing) {
    return new Promise((resolve) => {
      refreshSubscribers.push(resolve);
    });
  }

  isRefreshing = true;
  const refreshToken = getCookie('refreshToken') || tokenStorage.getRefreshToken();
  if (!refreshToken) {
    isRefreshing = false;
    return null;
  }

  setRefreshCookie(refreshToken);

  try {
    const response = await refreshClient.post<ApiResponse<LoginResponse>>('/v1/auth/refresh', {});
    const tokens = response.data.data;
    tokenStorage.setTokens(tokens);
    refreshSubscribers.forEach((callback) => callback(tokens.accessToken));
    refreshSubscribers = [];
    return tokens.accessToken;
  } catch (err) {
    tokenStorage.clear();
    refreshSubscribers.forEach((callback) => callback(null));
    refreshSubscribers = [];
    return null;
  } finally {
    isRefreshing = false;
  }
}

export default api;
