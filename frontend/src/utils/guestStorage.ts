import type { ProjectDto } from '../types/project'
import type { CreateTaskDto, TaskDto, UpdateTaskDto } from '../types/task'
import { TaskStatus, TaskPriority } from '../types/task'
import type { CreateProjectDto, UpdateProjectDto } from '../types/project'

const GUEST_PROJECTS_KEY = 'tc_guest_projects'
const GUEST_TASKS_KEY = 'tc_guest_tasks'

function readProjects(): ProjectDto[] {
  try {
    return JSON.parse(localStorage.getItem(GUEST_PROJECTS_KEY) ?? '[]')
  } catch {
    return []
  }
}

function writeProjects(projects: ProjectDto[]): void {
  localStorage.setItem(GUEST_PROJECTS_KEY, JSON.stringify(projects))
}

function readTasks(): TaskDto[] {
  try {
    return JSON.parse(localStorage.getItem(GUEST_TASKS_KEY) ?? '[]')
  } catch {
    return []
  }
}

function writeTasks(tasks: TaskDto[]): void {
  localStorage.setItem(GUEST_TASKS_KEY, JSON.stringify(tasks))
}

export function getGuestProjects(): ProjectDto[] {
  return readProjects()
}

export function getGuestProject(id: string): ProjectDto | undefined {
  return readProjects().find((p) => p.id === id)
}

export function createGuestProject(data: CreateProjectDto): ProjectDto {
  const project: ProjectDto = {
    id: crypto.randomUUID(),
    name: data.name,
    description: data.description ?? null,
    ownerId: 'guest',
    ownerUsername: 'Guest',
    memberCount: 1,
    taskCount: 0,
    subTaskCount: 0,
    currentUserRole: 'Manager',
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
    createdBy: null,
    updatedBy: null,
  }
  writeProjects([...readProjects(), project])
  return project
}

export function updateGuestProject(id: string, data: UpdateProjectDto): void {
  writeProjects(
    readProjects().map((p) =>
      p.id === id
        ? { ...p, name: data.name, description: data.description ?? null, updatedAt: new Date().toISOString() }
        : p
    )
  )
}

export function deleteGuestProject(id: string): void {
  writeProjects(readProjects().filter((p) => p.id !== id))
  writeTasks(readTasks().filter((t) => t.projectId !== id))
}

export function getGuestTasks(projectId: string): TaskDto[] {
  return readTasks().filter((t) => t.projectId === projectId)
}

export function createGuestTask(data: CreateTaskDto, projectName: string): TaskDto {
  const now = new Date().toISOString()
  const tasks = readTasks()

  const parentTask = data.parentTaskId ? tasks.find((t) => t.id === data.parentTaskId) : undefined

  const task: TaskDto = {
    id: crypto.randomUUID(),
    title: data.title,
    description: data.description ?? null,
    status: data.status ?? TaskStatus.ToDo,
    priority: data.priority ?? TaskPriority.Medium,
    startDate: data.startDate ?? null,
    dueDate: data.dueDate ?? null,
    projectId: data.projectId,
    projectName,
    parentTaskId: data.parentTaskId ?? null,
    parentTaskTitle: parentTask?.title ?? null,
    assigneeCount: 0,
    assignees: [],
    subTaskCount: 0,
    commentCount: 0,
    isOverdue: data.dueDate ? new Date(data.dueDate) < new Date() : false,
    hasSubTasks: false,
    createdAt: now,
    updatedAt: now,
    createdBy: null,
    updatedBy: null,
  }

  let updated = [...tasks, task]

  // Increment parent subTaskCount
  if (data.parentTaskId) {
    updated = updated.map((t) =>
      t.id === data.parentTaskId ? { ...t, subTaskCount: t.subTaskCount + 1, hasSubTasks: true } : t
    )
  }

  writeTasks(updated)

  // Increment project task/subtask count
  const projects = readProjects()
  writeProjects(
    projects.map((p) =>
      p.id === data.projectId
        ? {
            ...p,
            taskCount: data.parentTaskId ? p.taskCount : p.taskCount + 1,
            subTaskCount: data.parentTaskId ? p.subTaskCount + 1 : p.subTaskCount,
          }
        : p
    )
  )

  return task
}

export function updateGuestTask(id: string, data: UpdateTaskDto): void {
  writeTasks(
    readTasks().map((t) =>
      t.id === id
        ? {
            ...t,
            title: data.title,
            description: data.description ?? null,
            status: data.status,
            priority: data.priority,
            startDate: data.startDate ?? null,
            dueDate: data.dueDate ?? null,
            isOverdue: data.dueDate ? new Date(data.dueDate) < new Date() : false,
            updatedAt: new Date().toISOString(),
          }
        : t
    )
  )
}

export function updateGuestTaskStatus(id: string, newStatus: TaskStatus): void {
  writeTasks(
    readTasks().map((t) =>
      t.id === id ? { ...t, status: newStatus, updatedAt: new Date().toISOString() } : t
    )
  )
}

export function updateGuestTaskPriority(id: string, newPriority: TaskPriority): void {
  writeTasks(
    readTasks().map((t) =>
      t.id === id ? { ...t, priority: newPriority, updatedAt: new Date().toISOString() } : t
    )
  )
}

export function deleteGuestTask(id: string): void {
  const tasks = readTasks()
  const task = tasks.find((t) => t.id === id)
  if (!task) return

  const subtaskIds = tasks.filter((t) => t.parentTaskId === id).map((t) => t.id)
  const remaining = tasks.filter((t) => t.id !== id && !subtaskIds.includes(t.id))
  writeTasks(remaining)

  // Update parent subTaskCount
  if (task.parentTaskId) {
    writeTasks(
      remaining.map((t) =>
        t.id === task.parentTaskId
          ? { ...t, subTaskCount: Math.max(0, t.subTaskCount - 1), hasSubTasks: t.subTaskCount - 1 > 0 }
          : t
      )
    )
  }

  // Update project counts
  const projects = readProjects()
  writeProjects(
    projects.map((p) =>
      p.id === task.projectId
        ? {
            ...p,
            taskCount: task.parentTaskId ? p.taskCount : Math.max(0, p.taskCount - 1),
            subTaskCount: task.parentTaskId ? Math.max(0, p.subTaskCount - 1) : p.subTaskCount,
          }
        : p
    )
  )
}

export function clearGuestData(): void {
  localStorage.removeItem(GUEST_PROJECTS_KEY)
  localStorage.removeItem(GUEST_TASKS_KEY)
}

export function hasGuestData(): boolean {
  return readProjects().length > 0
}
