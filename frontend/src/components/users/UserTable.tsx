import { RoleBadge } from '../ui/Badge'
import { UserRole } from '../../types/user'
import type { UserDto } from '../../types/user'

type UserTableProps = {
  users: UserDto[]
  onEdit: (user: UserDto) => void
  onDelete: (user: UserDto) => void
  currentUserId?: string
}

export function UserTable({ users, onEdit, onDelete, currentUserId }: UserTableProps) {
  if (users.length === 0) {
    return (
      <div className="rounded-xl border border-dashed border-black/15 py-16 text-center text-sm text-muted dark:border-white/15">
        No users found.
      </div>
    )
  }

  return (
    <div className="overflow-hidden rounded-xl border border-black/8 dark:border-white/8">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b border-black/8 bg-black/3 text-left dark:border-white/8 dark:bg-white/3">
            <th className="px-4 py-3 font-medium text-muted">Username</th>
            <th className="px-4 py-3 font-medium text-muted">Email</th>
            <th className="px-4 py-3 font-medium text-muted">Name</th>
            <th className="px-4 py-3 font-medium text-muted">Role</th>
            <th className="px-4 py-3 font-medium text-muted">Joined</th>
            <th className="px-4 py-3" />
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr
              key={user.id}
              className="border-b border-black/5 last:border-b-0 hover:bg-black/2 dark:border-white/5 dark:hover:bg-white/3"
            >
              <td className="px-4 py-3 font-medium text-ink">
                {user.username}
                {user.id === currentUserId && (
                  <span className="ml-1.5 text-xs text-muted">(you)</span>
                )}
              </td>
              <td className="px-4 py-3 text-muted">{user.email}</td>
              <td className="px-4 py-3 text-ink">
                {[user.firstName, user.lastName].filter(Boolean).join(' ') || '—'}
              </td>
              <td className="px-4 py-3">
                <RoleBadge role={user.role} variant="user" />
              </td>
              <td className="px-4 py-3 text-muted">
                {new Date(user.createdAt).toLocaleDateString()}
              </td>
              <td className="px-4 py-3">
                <div className="flex items-center justify-end gap-1">
                  <button
                    type="button"
                    onClick={() => onEdit(user)}
                    className="rounded-md px-2 py-1 text-xs text-muted transition-colors hover:bg-black/5 hover:text-ink dark:hover:bg-white/8"
                  >
                    Edit
                  </button>
                  {user.role !== UserRole.Admin && (
                    <button
                      type="button"
                      onClick={() => onDelete(user)}
                      className="rounded-md px-2 py-1 text-xs text-muted transition-colors hover:bg-red-50 hover:text-red-600 dark:hover:bg-red-900/20 dark:hover:text-red-400"
                    >
                      Delete
                    </button>
                  )}
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
