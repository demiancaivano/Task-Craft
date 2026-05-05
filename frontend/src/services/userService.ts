import { apiClient } from './apiClient'
import type { CreateUserDto, UpdateUserDto, UserDto } from '../types/user'
import type { PagedResult } from '../types/api'

export const userService = {
  getAll: (params?: { includeDeleted?: boolean; page?: number; pageSize?: number }) =>
    apiClient.get<PagedResult<UserDto>>('/users', { params }),

  getById: (id: string) =>
    apiClient.get<UserDto>(`/users/${id}`),

  getByEmail: (email: string) =>
    apiClient.get<UserDto>(`/users/email/${email}`),

  getByUsername: (username: string) =>
    apiClient.get<UserDto>(`/users/username/${username}`),

  create: (data: CreateUserDto) =>
    apiClient.post<UserDto>('/users', data),

  update: (id: string, data: UpdateUserDto) =>
    apiClient.put<UserDto>(`/users/${id}`, data),

  remove: (id: string) =>
    apiClient.delete(`/users/${id}`),
}
