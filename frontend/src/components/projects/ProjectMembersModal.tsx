import { useState, useEffect, useCallback } from 'react'
import { Modal } from '../ui/Modal'
import { RoleBadge } from '../ui/Badge'
import { projectService } from '../../services/projectService'
import { userService } from '../../services/userService'
import { ProjectRole } from '../../types/project'
import type { ProjectMemberDto } from '../../types/project'
import type { UserDto } from '../../types/user'

type ProjectMembersModalProps = {
  isOpen: boolean
  onClose: () => void
  projectId: string
}

const ROLE_OPTIONS = [
  { value: ProjectRole.Manager, label: 'Manager' },
  { value: ProjectRole.Developer, label: 'Developer' },
  { value: ProjectRole.Member, label: 'Member' },
  { value: ProjectRole.Viewer, label: 'Viewer' },
]

export function ProjectMembersModal({ isOpen, onClose, projectId }: ProjectMembersModalProps) {
  const [members, setMembers] = useState<ProjectMemberDto[]>([])
  const [users, setUsers] = useState<UserDto[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [selectedUserId, setSelectedUserId] = useState('')
  const [selectedRole, setSelectedRole] = useState<ProjectRole>(ProjectRole.Member)
  const [isAdding, setIsAdding] = useState(false)
  const [isRemoving, setIsRemoving] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    setError(null)
    try {
      const [membersRes, usersRes] = await Promise.all([
        projectService.getMembers(projectId),
        userService.getAll(),
      ])
      setMembers(membersRes.data)
      setUsers(usersRes.data)
    } catch {
      setError('Failed to load members.')
    } finally {
      setIsLoading(false)
    }
  }, [projectId])

  useEffect(() => {
    if (isOpen) fetchData()
  }, [isOpen, fetchData])

  const memberIds = new Set(members.map((m) => m.userId))
  const availableUsers = users.filter((u) => !memberIds.has(u.id))

  const handleAdd = async () => {
    if (!selectedUserId) return
    setIsAdding(true)
    setError(null)
    try {
      await projectService.addMember(projectId, { userId: selectedUserId, role: selectedRole })
      setSelectedUserId('')
      await fetchData()
    } catch {
      setError('Failed to add member.')
    } finally {
      setIsAdding(false)
    }
  }

  const handleRemove = async (userId: string) => {
    setIsRemoving(userId)
    setError(null)
    try {
      await projectService.removeMember(projectId, userId)
      await fetchData()
    } catch {
      setError('Failed to remove member.')
    } finally {
      setIsRemoving(null)
    }
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Project Members" size="md">
      {/* Add member — never scrolls away */}
      <div className="shrink-0 pb-4">
        <span className="text-xs font-medium uppercase tracking-wide text-muted">Add Member</span>

        {error && <p className="mt-2 text-xs text-red-600">{error}</p>}

        <div className="mt-2 flex flex-col gap-2">
          <select
            value={selectedUserId}
            onChange={(e) => setSelectedUserId(e.target.value)}
            disabled={isAdding || availableUsers.length === 0}
            className="w-full rounded-lg border border-black/15 px-2 py-1.5 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent disabled:opacity-50"
          >
            <option value="">Select user…</option>
            {availableUsers.map((u) => (
              <option key={u.id} value={u.id}>
                {u.username} ({u.email})
              </option>
            ))}
          </select>

          <div className="flex gap-2">
            <select
              value={selectedRole}
              onChange={(e) => setSelectedRole(Number(e.target.value) as ProjectRole)}
              disabled={isAdding}
              className="flex-1 rounded-lg border border-black/15 px-2 py-1.5 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent disabled:opacity-50"
            >
              {ROLE_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
            <button
              type="button"
              onClick={handleAdd}
              disabled={!selectedUserId || isAdding}
              className="rounded-lg bg-accent px-4 py-1.5 text-sm font-medium text-white transition-colors hover:bg-accent/90 disabled:opacity-50"
            >
              {isAdding ? '…' : 'Add'}
            </button>
          </div>

          {availableUsers.length === 0 && !isLoading && (
            <p className="text-xs text-muted">All users are already members.</p>
          )}
        </div>
      </div>

      {/* Divider */}
      <div className="shrink-0 border-t border-black/8" />

      {/* Members list — scrollable */}
      <div className="flex min-h-0 flex-1 flex-col gap-1 overflow-y-auto pt-4">
        <span className="shrink-0 text-xs font-medium uppercase tracking-wide text-muted">
          Members ({members.length})
        </span>
        {isLoading ? (
          <p className="py-4 text-center text-sm text-muted">Loading…</p>
        ) : members.length === 0 ? (
          <p className="text-xs text-muted">No members yet.</p>
        ) : (
          <ul className="flex flex-col gap-1">
            {members.map((member) => (
              <li key={member.userId} className="flex items-center gap-3 rounded-lg px-2 py-2">
                <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-accent/15 text-[11px] font-bold text-accent">
                  {member.username.charAt(0).toUpperCase()}
                </span>
                <div className="min-w-0 flex-1">
                  <p className="truncate text-sm font-medium text-ink">{member.username}</p>
                  <p className="truncate text-xs text-muted">{member.email}</p>
                </div>
                <RoleBadge role={member.role} variant="project" />
                <button
                  type="button"
                  onClick={() => handleRemove(member.userId)}
                  disabled={!!isRemoving}
                  className="ml-1 rounded p-1 text-xs text-muted transition-colors hover:bg-red-50 hover:text-red-600 disabled:opacity-50"
                  aria-label={`Remove ${member.username}`}
                >
                  {isRemoving === member.userId ? '…' : '✕'}
                </button>
          </li>
            ))}
          </ul>
        )}
      </div>
    </Modal>
  )
}
