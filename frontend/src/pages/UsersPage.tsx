import { useState, useEffect, useCallback } from 'react'
import { UserTable } from '../components/users/UserTable'
import { UserForm } from '../components/users/UserForm'
import { ConfirmDialog } from '../components/ui/ConfirmDialog'
import { userService } from '../services/userService'
import { useAuth } from '../context/AuthContext'
import type { CreateUserDto, UpdateUserDto, UserDto } from '../types/user'
import { setStoredSession } from '../utils/authStorage'

const PAGE_SIZE = 20

export function UsersPage() {
  const { session } = useAuth()
  const [users, setUsers] = useState<UserDto[]>([])
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [formOpen, setFormOpen] = useState(false)
  const [editingUser, setEditingUser] = useState<UserDto | null>(null)
  const [deletingUser, setDeletingUser] = useState<UserDto | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  const fetchUsers = useCallback(async (targetPage: number) => {
    setIsLoading(true)
    setError(null)
    try {
      const response = await userService.getAll({ page: targetPage, pageSize: PAGE_SIZE })
      const result = response.data
      setUsers(result.items.filter((u) => !u.isAnonymous))
      setTotalPages(result.totalPages)
      setTotalCount(result.totalCount)
    } catch {
      setError('Failed to load users.')
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    fetchUsers(page)
  }, [fetchUsers, page])

  const handleCreate = async (data: CreateUserDto | UpdateUserDto) => {
    await userService.create(data as CreateUserDto)
    await fetchUsers(page)
  }

  const handleEdit = async (data: CreateUserDto | UpdateUserDto) => {
    if (!editingUser) return
    const res = await userService.update(editingUser.id, data as UpdateUserDto)

    if (session?.user.id === editingUser.id) {
      setStoredSession({
        ...session,
        user: {
          ...session.user,
          username: res.data.username,
          email: res.data.email,
          firstName: res.data.firstName ?? '',
          lastName: res.data.lastName ?? '',
          role: res.data.role,
        },
      })
    }

    await fetchUsers(page)
  }

  const handleDelete = async () => {
    if (!deletingUser) return
    setIsDeleting(true)
    try {
      await userService.remove(deletingUser.id)
      setDeletingUser(null)
      // If last item on page, go to previous page
      const newPage = users.length === 1 && page > 1 ? page - 1 : page
      setPage(newPage)
      await fetchUsers(newPage)
    } finally {
      setIsDeleting(false)
    }
  }

  const openEdit = (user: UserDto) => {
    setEditingUser(user)
    setFormOpen(true)
  }

  const closeForm = () => {
    setFormOpen(false)
    setEditingUser(null)
  }

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-ink">Users</h1>
          <p className="text-sm text-muted">
            Manage user accounts
            {!isLoading && totalCount > 0 && (
              <span className="ml-1 text-muted/70">({totalCount} total)</span>
            )}
          </p>
        </div>
        <button
          type="button"
          onClick={() => setFormOpen(true)}
          className="rounded-lg bg-accent px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-accent/90"
        >
          + New User
        </button>
      </div>

      {isLoading && (
        <div className="py-12 text-center text-sm text-muted">Loading users…</div>
      )}

      {error && (
        <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-600 dark:border-red-800/30 dark:bg-red-900/20 dark:text-red-400">
          {error}
          <button onClick={() => fetchUsers(page)} className="ml-2 underline">
            Retry
          </button>
        </div>
      )}

      {!isLoading && !error && (
        <UserTable
          users={users}
          onEdit={openEdit}
          onDelete={setDeletingUser}
          currentUserId={session?.user.id}
        />
      )}

      {!isLoading && totalPages > 1 && (
        <div className="flex items-center justify-center gap-3">
          <button
            type="button"
            disabled={page <= 1}
            onClick={() => setPage((p) => p - 1)}
            className="rounded-lg border border-black/10 px-3 py-1.5 text-sm text-ink transition-colors hover:bg-black/5 disabled:cursor-not-allowed disabled:opacity-40 dark:border-white/10 dark:hover:bg-white/5"
          >
            Previous
          </button>
          <span className="text-sm text-muted">
            Page {page} of {totalPages}
          </span>
          <button
            type="button"
            disabled={page >= totalPages}
            onClick={() => setPage((p) => p + 1)}
            className="rounded-lg border border-black/10 px-3 py-1.5 text-sm text-ink transition-colors hover:bg-black/5 disabled:cursor-not-allowed disabled:opacity-40 dark:border-white/10 dark:hover:bg-white/5"
          >
            Next
          </button>
        </div>
      )}

      <UserForm
        isOpen={formOpen}
        onClose={closeForm}
        onSubmit={editingUser ? handleEdit : handleCreate}
        user={editingUser}
      />

      <ConfirmDialog
        isOpen={!!deletingUser}
        onClose={() => setDeletingUser(null)}
        onConfirm={handleDelete}
        title="Delete User"
        message={`Are you sure you want to delete "${deletingUser?.username}"? This action cannot be undone.`}
        isLoading={isDeleting}
      />
    </div>
  )
}
