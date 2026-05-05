import { useState, useEffect, useCallback } from 'react'
import { Modal } from '../ui/Modal'
import { StatusBadge, PriorityBadge } from '../ui/Badge'
import { taskService } from '../../services/taskService'
import { ProjectRole } from '../../types/project'
import type { ProjectRole as ProjectRoleType } from '../../types/project'
import type { TaskAssignmentDto, TaskDto, UpdateTaskPriorityDto, UpdateTaskStatusDto } from '../../types/task'
import { TaskPriority, TaskStatus } from '../../types/task'

type TaskDetailModalProps = {
  task: TaskDto | null
  isOpen: boolean
  onClose: () => void
  onEdit: (task: TaskDto) => void
  onDelete: (task: TaskDto) => void
  onAddSubtask: (parentTaskId: string) => void
  onAssign: (task: TaskDto) => void
  onRefresh: () => void
  userRole: ProjectRoleType | null
  isGuest?: boolean
  guestSubtasks?: TaskDto[]
  onGuestTaskUpdate?: (taskId: string, updates: { status?: TaskStatus; priority?: TaskPriority }) => void
}

const STATUS_OPTIONS = [
  { value: TaskStatus.ToDo, label: 'To Do' },
  { value: TaskStatus.InProgress, label: 'In Progress' },
  { value: TaskStatus.Done, label: 'Done' },
  { value: TaskStatus.Blocked, label: 'Blocked' },
]

const PRIORITY_OPTIONS = [
  { value: TaskPriority.Low, label: 'Low' },
  { value: TaskPriority.Medium, label: 'Medium' },
  { value: TaskPriority.High, label: 'High' },
  { value: TaskPriority.Critical, label: 'Critical' },
]

export function TaskDetailModal({
  task,
  isOpen,
  onClose,
  onEdit,
  onDelete,
  onAddSubtask,
  onAssign,
  onRefresh,
  userRole,
  isGuest = false,
  guestSubtasks,
  onGuestTaskUpdate,
}: TaskDetailModalProps) {
  const [subtasks, setSubtasks] = useState<TaskDto[]>([])
  const [assignments, setAssignments] = useState<TaskAssignmentDto[]>([])
  const [isUpdating, setIsUpdating] = useState(false)

  const canDevelop = isGuest || (userRole !== null && userRole <= ProjectRole.Developer)
  const canMember = isGuest || (userRole !== null && userRole <= ProjectRole.Member)
  const isManager = isGuest || userRole === ProjectRole.Manager

  const fetchDetails = useCallback(async () => {
    if (!task) return
    if (isGuest) {
      setSubtasks(guestSubtasks ?? [])
      setAssignments([])
      return
    }
    const [subtasksRes, assignmentsRes] = await Promise.all([
      task.hasSubTasks ? taskService.getSubTasks(task.id) : Promise.resolve({ data: [] }),
      taskService.getAssignments(task.id),
    ])
    setSubtasks(subtasksRes.data)
    setAssignments(assignmentsRes.data)
  }, [task, isGuest, guestSubtasks])

  useEffect(() => {
    if (isOpen && task) {
      fetchDetails()
    }
  }, [isOpen, task, fetchDetails])

  const handleStatusChange = async (newStatus: TaskStatus) => {
    if (!task) return
    setIsUpdating(true)
    try {
      if (isGuest && onGuestTaskUpdate) {
        onGuestTaskUpdate(task.id, { status: newStatus })
      } else {
        const payload: UpdateTaskStatusDto = { taskId: task.id, newStatus }
        await taskService.updateStatus(task.id, payload)
        onRefresh()
      }
    } finally {
      setIsUpdating(false)
    }
  }

  const handlePriorityChange = async (newPriority: TaskPriority) => {
    if (!task) return
    setIsUpdating(true)
    try {
      if (isGuest && onGuestTaskUpdate) {
        onGuestTaskUpdate(task.id, { priority: newPriority })
      } else {
        const payload: UpdateTaskPriorityDto = { taskId: task.id, newPriority }
        await taskService.updatePriority(task.id, payload)
        onRefresh()
      }
    } finally {
      setIsUpdating(false)
    }
  }

  if (!task) return null

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={task.title} size="lg">
      <div className="flex flex-col gap-5 overflow-y-auto">
        {/* Status & Priority controls */}
        <div className="flex flex-wrap gap-4">
          {canMember && (
            <div className="flex flex-col gap-1.5">
              <span className="text-xs font-medium uppercase tracking-wide text-muted">Status</span>
              <select
                value={task.status}
                onChange={(e) => handleStatusChange(Number(e.target.value) as TaskStatus)}
                disabled={isUpdating}
                className="rounded-lg border border-black/15 px-2 py-1.5 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent disabled:opacity-50"
              >
                {STATUS_OPTIONS.map((opt) => (
                  <option key={opt.value} value={opt.value}>
                    {opt.label}
                  </option>
                ))}
              </select>
            </div>
          )}

          {canDevelop && (
            <div className="flex flex-col gap-1.5">
              <span className="text-xs font-medium uppercase tracking-wide text-muted">Priority</span>
              <select
                value={task.priority}
                onChange={(e) => handlePriorityChange(Number(e.target.value) as TaskPriority)}
                disabled={isUpdating}
                className="rounded-lg border border-black/15 px-2 py-1.5 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent disabled:opacity-50"
              >
                {PRIORITY_OPTIONS.map((opt) => (
                  <option key={opt.value} value={opt.value}>
                    {opt.label}
                  </option>
                ))}
              </select>
            </div>
          )}

          <div className="flex flex-col gap-1.5">
            <span className="text-xs font-medium uppercase tracking-wide text-muted">Current</span>
            <div className="flex items-center gap-1.5 pt-1">
              <StatusBadge status={task.status} />
              <PriorityBadge priority={task.priority} />
            </div>
          </div>
        </div>

        {/* Description */}
        {task.description && (
          <div>
            <span className="text-xs font-medium uppercase tracking-wide text-muted">Description</span>
            <p className="mt-1.5 text-sm text-ink">{task.description}</p>
          </div>
        )}

        {/* Dates */}
        {(task.startDate || task.dueDate) && (
          <div className="flex gap-6 text-sm">
            {task.startDate && (
              <div>
                <span className="text-xs font-medium uppercase tracking-wide text-muted">Start</span>
                <p className="mt-0.5 text-ink">{new Date(task.startDate).toLocaleDateString()}</p>
              </div>
            )}
            {task.dueDate && (
              <div>
                <span className="text-xs font-medium uppercase tracking-wide text-muted">Due</span>
                <p className={`mt-0.5 ${task.isOverdue ? 'font-medium text-red-600' : 'text-ink'}`}>
                  {new Date(task.dueDate).toLocaleDateString()}
                  {task.isOverdue && ' (overdue)'}
                </p>
              </div>
            )}
          </div>
        )}

        {/* Parent task */}
        {task.parentTaskTitle && (
          <div className="text-sm text-muted">
            Subtask of: <span className="font-medium text-ink">{task.parentTaskTitle}</span>
          </div>
        )}

        {/* Assignees */}
        {!isGuest && (
          <div>
            <div className="flex items-center justify-between">
              <span className="text-xs font-medium uppercase tracking-wide text-muted">Assignees</span>
              {canDevelop && (
                <button
                  type="button"
                  onClick={() => onAssign(task)}
                  className="text-xs text-accent hover:underline"
                >
                  + Assign user
                </button>
              )}
            </div>
            {assignments.length === 0 ? (
              <p className="mt-1.5 text-xs text-muted">No assignees yet.</p>
            ) : (
              <ul className="mt-1.5 flex flex-col gap-1">
                {assignments.map((a) => (
                  <li key={a.userId} className="flex items-center gap-2 text-sm text-ink">
                    <span className="flex h-6 w-6 items-center justify-center rounded-full bg-accent/15 text-[10px] font-bold text-accent">
                      {a.username.charAt(0).toUpperCase()}
                    </span>
                    {a.username}
                    <span className="text-muted">({a.email})</span>
                  </li>
                ))}
              </ul>
            )}
          </div>
        )}

        {/* Subtasks */}
        <div>
          <div className="flex items-center justify-between">
            <span className="text-xs font-medium uppercase tracking-wide text-muted">
              Subtasks ({subtasks.length})
            </span>
            {canDevelop && (
              <button
                type="button"
                onClick={() => { onClose(); onAddSubtask(task.id) }}
                className="text-xs text-accent hover:underline"
              >
                + Add subtask
              </button>
            )}
          </div>
          {subtasks.length === 0 ? (
            <p className="mt-1.5 text-xs text-muted">No subtasks.</p>
          ) : (
            <ul className="mt-1.5 flex flex-col gap-1">
              {subtasks.map((st) => (
                <li key={st.id} className="flex items-center gap-2 rounded-md bg-black/3 px-2 py-1.5 text-sm">
                  <StatusBadge status={st.status} />
                  <span className="text-ink">{st.title}</span>
                </li>
              ))}
            </ul>
          )}
        </div>

        {/* Actions */}
        {(isManager || canDevelop) && (
          <div className="flex justify-between border-t border-black/8 pt-3">
            {isManager && (
              <button
                type="button"
                onClick={() => { onClose(); onDelete(task) }}
                className="rounded-lg px-3 py-1.5 text-sm font-medium text-red-600 transition-colors hover:bg-red-50"
              >
                Delete task
              </button>
            )}
            {!isManager && <span />}
            {canDevelop && (
              <button
                type="button"
                onClick={() => { onClose(); onEdit(task) }}
                className="rounded-lg bg-accent px-4 py-1.5 text-sm font-medium text-white transition-colors hover:bg-accent/90"
              >
                Edit task
              </button>
            )}
          </div>
        )}
      </div>
    </Modal>
  )
}
