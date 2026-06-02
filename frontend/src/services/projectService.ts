import { apiClient } from './apiClient'
import type {
  AddProjectMemberDto,
  CreateProjectDto,
  ProjectDto,
  ProjectMemberDto,
  UpdateProjectDto,
} from '../types/project'

export const projectService = {
  getAll: (includeDeleted = false) =>
    apiClient.get<ProjectDto[]>('/projects', { params: { includeDeleted } }),

  getById: (id: string) =>
    apiClient.get<ProjectDto>(`/projects/${id}`),

  getByUser: (userId: string) =>
    apiClient.get<ProjectDto[]>(`/projects/user/${userId}`),

  create: (data: CreateProjectDto) =>
    apiClient.post<ProjectDto>('/projects', data),

  update: (id: string, data: UpdateProjectDto) =>
    apiClient.put<ProjectDto>(`/projects/${id}`, data),

  remove: (id: string) =>
    apiClient.delete(`/projects/${id}`),

  getMembers: (id: string) =>
    apiClient.get<ProjectMemberDto[]>(`/projects/${id}/members`),

  addMember: (id: string, data: AddProjectMemberDto) =>
    apiClient.post<ProjectMemberDto>(`/projects/${id}/members`, data),

  removeMember: (projectId: string, userId: string) =>
    apiClient.delete(`/projects/${projectId}/members/${userId}`),

  leave: (projectId: string) =>
    apiClient.delete(`/projects/${projectId}/leave`),
}
