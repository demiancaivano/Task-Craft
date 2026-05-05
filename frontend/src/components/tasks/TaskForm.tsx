import { useState, useEffect } from 'react'
import { Modal } from '../ui/Modal'
import { TaskPriority, TaskStatus } from '../../types/task'
import type { CreateTaskDto, TaskDto, UpdateTaskDto } from '../../types/task'

type TaskFormProps = {
  isOpen: boolean
  onClose: () => void
  onSubmit: (data: CreateTaskDto | UpdateTaskDto) => Promise<void>
  projectId: string
  task?: TaskDto | null
  defaultStatus?: TaskStatus
  parentTaskId?: string
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

export function TaskForm({
  isOpen,
  onClose,
  onSubmit,
  projectId,
  task,
  defaultStatus = TaskStatus.ToDo,
  parentTaskId,
}: TaskFormProps) {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [status, setStatus] = useState<TaskStatus>(defaultStatus)
  const [priority, setPriority] = useState<TaskPriority>(TaskPriority.Medium)
  const [startDate, setStartDate] = useState('')
  const [dueDate, setDueDate] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const isEditing = !!task

  useEffect(() => {
    if (isOpen) {
      setTitle(task?.title ?? '')
      setDescription(task?.description ?? '')
      setStatus(task?.status ?? defaultStatus)
      setPriority(task?.priority ?? TaskPriority.Medium)
      setStartDate(task?.startDate ? task.startDate.split('T')[0] : '')
      setDueDate(task?.dueDate ? task.dueDate.split('T')[0] : '')
      setError(null)
    }
  }, [isOpen, task, defaultStatus])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!title.trim()) return

    setIsLoading(true)
    setError(null)
    try {
      if (isEditing && task) {
        const data: UpdateTaskDto = {
          id: task.id,
          title: title.trim(),
          description: description.trim() || undefined,
          status,
          priority,
          startDate: startDate || undefined,
          dueDate: dueDate || undefined,
        }
        await onSubmit(data)
      } else {
        const data: CreateTaskDto = {
          title: title.trim(),
          description: description.trim() || undefined,
          status,
          priority,
          startDate: startDate || undefined,
          dueDate: dueDate || undefined,
          projectId,
          parentTaskId: parentTaskId || undefined,
        }
        await onSubmit(data)
      }
      onClose()
    } catch {
      setError('Failed to save task. Please try again.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEditing ? 'Edit Task' : parentTaskId ? 'New Subtask' : 'New Task'}
      size="lg"
    >
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <div className="flex flex-col gap-1.5">
          <label htmlFor="task-title" className="text-sm font-medium text-ink">
            Title <span className="text-red-500">*</span>
          </label>
          <input
            id="task-title"
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Task title"
            minLength={3}
            maxLength={300}
            required
            className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
          />
        </div>

        <div className="flex flex-col gap-1.5">
          <label htmlFor="task-description" className="text-sm font-medium text-ink">
            Description
          </label>
          <textarea
            id="task-description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Optional description"
            rows={3}
            maxLength={2000}
            className="w-full resize-none rounded-lg border border-black/15 px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div className="flex flex-col gap-1.5">
            <label htmlFor="task-status" className="text-sm font-medium text-ink">
              Status
            </label>
            <select
              id="task-status"
              value={status}
              onChange={(e) => setStatus(Number(e.target.value) as TaskStatus)}
              className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
            >
              {STATUS_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>

          <div className="flex flex-col gap-1.5">
            <label htmlFor="task-priority" className="text-sm font-medium text-ink">
              Priority
            </label>
            <select
              id="task-priority"
              value={priority}
              onChange={(e) => setPriority(Number(e.target.value) as TaskPriority)}
              className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
            >
              {PRIORITY_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div className="flex flex-col gap-1.5">
            <label htmlFor="task-start-date" className="text-sm font-medium text-ink">
              Start Date
            </label>
            <input
              id="task-start-date"
              type="date"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
            />
          </div>

          <div className="flex flex-col gap-1.5">
            <label htmlFor="task-due-date" className="text-sm font-medium text-ink">
              Due Date
            </label>
            <input
              id="task-due-date"
              type="date"
              value={dueDate}
              onChange={(e) => setDueDate(e.target.value)}
              className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
            />
          </div>
        </div>

        {error && <p className="text-xs text-red-600">{error}</p>}

        <div className="flex justify-end gap-2 pt-1">
          <button
            type="button"
            onClick={onClose}
            disabled={isLoading}
            className="rounded-lg border border-black/10 px-4 py-2 text-sm font-medium text-ink transition-colors hover:bg-black/5 disabled:opacity-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={isLoading || !title.trim()}
            className="rounded-lg bg-accent px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-accent/90 disabled:opacity-50"
          >
            {isLoading ? 'Saving…' : isEditing ? 'Save Changes' : 'Create Task'}
          </button>
        </div>
      </form>
    </Modal>
  )
}
