export const TaskStatus = {
  ToDo: 1,
  InProgress: 2,
  Done: 3,
  Blocked: 4,
} as const
export type TaskStatus = (typeof TaskStatus)[keyof typeof TaskStatus]

export const TaskPriority = {
  Low: 1,
  Medium: 2,
  High: 3,
  Critical: 4,
} as const
export type TaskPriority = (typeof TaskPriority)[keyof typeof TaskPriority]

export interface TaskDto {
  id: string
  title: string
  description: string | null
  status: TaskStatus
  priority: TaskPriority
  startDate: string | null
  dueDate: string | null
  projectId: string
  projectName: string | null
  parentTaskId: string | null
  parentTaskTitle: string | null
  assigneeCount: number
  assignees: { userId: string; username: string }[]
  subTaskCount: number
  commentCount: number
  isOverdue: boolean
  hasSubTasks: boolean
  createdAt: string
  updatedAt: string
  createdBy: string | null
  updatedBy: string | null
}

export interface TaskAssignmentDto {
  taskId: string
  userId: string
  username: string
  email: string
  firstName: string | null
  lastName: string | null
  assignedAt: string
  assignedBy: string | null
}

export interface CreateTaskDto {
  title: string
  description?: string
  status?: TaskStatus
  priority?: TaskPriority
  startDate?: string
  dueDate?: string
  projectId: string
  parentTaskId?: string
}

export interface UpdateTaskDto {
  id: string
  title: string
  description?: string
  status: TaskStatus
  priority: TaskPriority
  startDate?: string
  dueDate?: string
}

export interface UpdateTaskStatusDto {
  taskId: string
  newStatus: TaskStatus
}

export interface UpdateTaskPriorityDto {
  taskId: string
  newPriority: TaskPriority
}

export interface AssignTaskDto {
  taskId: string
  userId: string
  assignedBy?: string
}
