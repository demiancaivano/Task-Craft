import { apiClient } from './apiClient'
import type {
  AssignTaskDto,
  CreateTaskDto,
  TaskAssignmentDto,
  TaskDto,
  UpdateTaskDto,
  UpdateTaskPriorityDto,
  UpdateTaskStatusDto,
} from '../types/task'

export const taskService = {
  getAll: (includeDeleted = false) =>
    apiClient.get<TaskDto[]>('/tasks', { params: { includeDeleted } }),

  getById: (id: string) =>
    apiClient.get<TaskDto>(`/tasks/${id}`),

  getByProject: (projectId: string) =>
    apiClient.get<TaskDto[]>(`/tasks/project/${projectId}`),

  getByUser: (userId: string) =>
    apiClient.get<TaskDto[]>(`/tasks/user/${userId}`),

  getByStatus: (status: number) =>
    apiClient.get<TaskDto[]>(`/tasks/status/${status}`),

  getOverdue: () =>
    apiClient.get<TaskDto[]>('/tasks/overdue'),

  getSubTasks: (parentTaskId: string) =>
    apiClient.get<TaskDto[]>(`/tasks/${parentTaskId}/subtasks`),

  getAssignments: (taskId: string) =>
    apiClient.get<TaskAssignmentDto[]>(`/tasks/${taskId}/assignments`),

  create: (data: CreateTaskDto) =>
    apiClient.post<TaskDto>('/tasks', data),

  update: (id: string, data: UpdateTaskDto) =>
    apiClient.put<TaskDto>(`/tasks/${id}`, data),

  updateStatus: (id: string, data: UpdateTaskStatusDto) =>
    apiClient.patch<TaskDto>(`/tasks/${id}/status`, data),

  updatePriority: (id: string, data: UpdateTaskPriorityDto) =>
    apiClient.patch<TaskDto>(`/tasks/${id}/priority`, data),

  remove: (id: string) =>
    apiClient.delete(`/tasks/${id}`),

  assign: (id: string, data: AssignTaskDto) =>
    apiClient.post<TaskAssignmentDto>(`/tasks/${id}/assign`, data),
}
