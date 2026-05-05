import { apiClient } from './apiClient'
import type { AuthResponse, LoginRequest, RegisterRequest, RefreshTokenRequest, ValidateTokenResponse } from '../types/auth'

const authBasePath = '/auth'

export const authService = {
  async login(payload: LoginRequest) {
    const { data } = await apiClient.post<AuthResponse>(`${authBasePath}/login`, {
      username: payload.identifier,
      password: payload.password,
    })
    return data
  },

  async register(payload: RegisterRequest) {
    const { data } = await apiClient.post<AuthResponse>(`${authBasePath}/register`, payload)
    return data
  },

  async refresh(payload: RefreshTokenRequest) {
    const { data } = await apiClient.post<AuthResponse>(`${authBasePath}/refresh`, payload)
    return data
  },

  async revoke(payload: RefreshTokenRequest) {
    const { data } = await apiClient.post<{ message: string }>(`${authBasePath}/revoke`, payload)
    return data
  },

  async validate(payload: RefreshTokenRequest) {
    const { data } = await apiClient.post<ValidateTokenResponse>(`${authBasePath}/validate`, payload)
    return data
  },
}