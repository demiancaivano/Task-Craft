import { useState, useEffect } from 'react'
import { Modal } from '../ui/Modal'
import { projectService } from '../../services/projectService'
import { taskService } from '../../services/taskService'
import type { ProjectMemberDto } from '../../types/project'
import type { TaskDto } from '../../types/task'
import { useAuth } from '../../context/AuthContext'

type AssignUserModalProps = {
  isOpen: boolean
  onClose: () => void
  task: TaskDto | null
  projectId: string
  onAssigned: () => void
}

export function AssignUserModal({ isOpen, onClose, task, projectId, onAssigned }: AssignUserModalProps) {
  const { session } = useAuth()
  const [members, setMembers] = useState<ProjectMemberDto[]>([])
  const [assignedUserIds, setAssignedUserIds] = useState<Set<string>>(new Set())
  const [isLoading, setIsLoading] = useState(false)
  const [isAssigning, setIsAssigning] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!isOpen || !task) return
    setIsLoading(true)
    setError(null)
    Promise.all([
      projectService.getMembers(projectId),
      taskService.getAssignments(task.id),
    ])
      .then(([membersRes, assignmentsRes]) => {
        setMembers(membersRes.data)
        setAssignedUserIds(new Set(assignmentsRes.data.map((a) => a.userId)))
      })
      .catch(() => setError('Failed to load members.'))
      .finally(() => setIsLoading(false))
  }, [isOpen, task, projectId])

  const handleAssign = async (userId: string) => {
    if (!task) return
    setIsAssigning(userId)
    setError(null)
    try {
      await taskService.assign(task.id, {
        taskId: task.id,
        userId,
        assignedBy: session?.user.id,
      })
      onAssigned()
      onClose()
    } catch {
      setError('Failed to assign user.')
    } finally {
      setIsAssigning(null)
    }
  }

  const available = members.filter((m) => !assignedUserIds.has(m.userId))

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Assign Member" size="sm">
      {isLoading && <p className="py-4 text-center text-sm text-muted">Loading members…</p>}
      {error && <p className="mb-3 text-xs text-red-600">{error}</p>}

      {!isLoading && (
        <ul className="flex flex-col gap-1 overflow-y-auto">
          {available.map((member) => (
            <li key={member.userId}>
              <button
                type="button"
                disabled={!!isAssigning}
                onClick={() => handleAssign(member.userId)}
                className="flex w-full items-center gap-3 rounded-lg px-3 py-2 text-left text-sm transition-colors hover:bg-black/5 disabled:opacity-50 dark:hover:bg-white/8"
              >
                <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-accent/15 text-[11px] font-bold text-accent">
                  {member.username.charAt(0).toUpperCase()}
                </span>
                <div className="min-w-0">
                  <p className="truncate font-medium text-ink">{member.username}</p>
                  <p className="truncate text-xs text-muted">{member.email}</p>
                </div>
                {isAssigning === member.userId && (
                  <span className="ml-auto text-xs text-muted">Assigning…</span>
                )}
              </button>
            </li>
          ))}
          {available.length === 0 && !isLoading && (
            <p className="py-4 text-center text-xs text-muted">
              All project members are already assigned.
            </p>
          )}
        </ul>
      )}
    </Modal>
  )
}
