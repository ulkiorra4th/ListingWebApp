import api from '@/api/client';
import type { ApiResponse, Profile } from '@/types';

export type CreateProfilePayload = {
  nickname: string;
  age: number;
  languageCode: number;
  countryCode: number;
};

export type UpdateProfilePayload = CreateProfilePayload;

export async function getProfileById(accountId: string, id: string): Promise<Profile> {
  const response = await api.get<ApiResponse<Profile>>(`/v1/accounts/${accountId}/profiles/${id}`);
  return response.data.data;
}

export async function getProfiles(accountId: string): Promise<Profile[]> {
  const response = await api.get<ApiResponse<Profile[]>>(`/v1/accounts/${accountId}/profiles`);
  return response.data.data;
}

export async function createProfile(accountId: string, payload: CreateProfilePayload): Promise<Profile> {
  const response = await api.post<ApiResponse<Profile>>(`/v1/accounts/${accountId}/profiles`, payload);
  return response.data.data;
}

export async function updateProfile(
  accountId: string,
  id: string,
  payload: UpdateProfilePayload,
): Promise<Profile> {
  const response = await api.put<ApiResponse<Profile>>(`/v1/accounts/${accountId}/profiles/${id}`, payload);
  return response.data.data;
}

export async function deleteProfile(accountId: string, id: string): Promise<void> {
  await api.delete(`/v1/accounts/${accountId}/profiles/${id}`);
}

export async function uploadProfileIcon(accountId: string, profileId: string, file: File): Promise<void> {
  const formData = new FormData();
  formData.append('file', file);
  await api.patch(`/v1/accounts/${accountId}/profiles/${profileId}/icon`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
}
