import { TaskPriority, TaskStatus } from '../../types/task'
import { ProjectRole } from '../../types/project'
import { UserRole } from '../../types/user'

type StatusBadgeProps = { status: TaskStatus }
type PriorityBadgeProps = { priority: TaskPriority }
type RoleBadgeProps = { role: ProjectRole | UserRole; variant?: 'project' | 'user' }

const STATUS_CONFIG: Record<TaskStatus, { label: string; className: string }> = {
  [TaskStatus.ToDo]: { label: 'To Do', className: 'bg-gray-100 text-gray-600 dark:bg-gray-700/50 dark:text-gray-300' },
  [TaskStatus.InProgress]: { label: 'In Progress', className: 'bg-blue-100 text-blue-700 dark:bg-blue-900/40 dark:text-blue-300' },
  [TaskStatus.Done]: { label: 'Done', className: 'bg-green-100 text-green-700 dark:bg-green-900/40 dark:text-green-300' },
  [TaskStatus.Blocked]: { label: 'Blocked', className: 'bg-red-100 text-red-700 dark:bg-red-900/40 dark:text-red-300' },
}

const PRIORITY_CONFIG: Record<TaskPriority, { label: string; className: string }> = {
  [TaskPriority.Low]: { label: 'Low', className: 'bg-gray-100 text-gray-500 dark:bg-gray-700/50 dark:text-gray-400' },
  [TaskPriority.Medium]: { label: 'Medium', className: 'bg-yellow-100 text-yellow-700 dark:bg-yellow-900/40 dark:text-yellow-300' },
  [TaskPriority.High]: { label: 'High', className: 'bg-orange-100 text-orange-700 dark:bg-orange-900/40 dark:text-orange-300' },
  [TaskPriority.Critical]: { label: 'Critical', className: 'bg-red-100 text-red-700 font-semibold dark:bg-red-900/40 dark:text-red-300' },
}

const PROJECT_ROLE_CONFIG: Record<ProjectRole, { label: string; className: string }> = {
  [ProjectRole.Manager]: { label: 'Manager', className: 'bg-purple-100 text-purple-700 dark:bg-purple-900/40 dark:text-purple-300' },
  [ProjectRole.Developer]: { label: 'Developer', className: 'bg-blue-100 text-blue-700 dark:bg-blue-900/40 dark:text-blue-300' },
  [ProjectRole.Member]: { label: 'Member', className: 'bg-gray-100 text-gray-600 dark:bg-gray-700/50 dark:text-gray-300' },
  [ProjectRole.Viewer]: { label: 'Viewer', className: 'bg-gray-100 text-gray-400 dark:bg-gray-700/50 dark:text-gray-500' },
}

const USER_ROLE_CONFIG: Record<UserRole, { label: string; className: string }> = {
  [UserRole.Admin]: { label: 'Admin', className: 'bg-red-100 text-red-700 dark:bg-red-900/40 dark:text-red-300' },
  [UserRole.User]: { label: 'User', className: 'bg-gray-100 text-gray-600 dark:bg-gray-700/50 dark:text-gray-300' },
}

const BASE = 'inline-flex items-center rounded-full px-2 py-0.5 text-xs'

export function StatusBadge({ status }: StatusBadgeProps) {
  const config = STATUS_CONFIG[status]
  return <span className={`${BASE} ${config.className}`}>{config.label}</span>
}

export function PriorityBadge({ priority }: PriorityBadgeProps) {
  const config = PRIORITY_CONFIG[priority]
  return <span className={`${BASE} ${config.className}`}>{config.label}</span>
}

export function RoleBadge({ role, variant = 'project' }: RoleBadgeProps) {
  const config =
    variant === 'user'
      ? USER_ROLE_CONFIG[role as UserRole]
      : PROJECT_ROLE_CONFIG[role as ProjectRole]

  if (!config) {
    return <span className={`${BASE} bg-gray-100 text-gray-400`}>Unknown</span>
  }

  return <span className={`${BASE} ${config.className}`}>{config.label}</span>
}
