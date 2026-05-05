import { useState, useEffect } from 'react'
import { Modal } from '../ui/Modal'
import { UserRole } from '../../types/user'
import type { CreateUserDto, UpdateUserDto, UserDto } from '../../types/user'

type UserFormProps = {
  isOpen: boolean
  onClose: () => void
  onSubmit: (data: CreateUserDto | UpdateUserDto) => Promise<void>
  user?: UserDto | null
}

const ROLE_OPTIONS = [
  { value: UserRole.User, label: 'User' },
  { value: UserRole.Admin, label: 'Admin' },
]

export function UserForm({ isOpen, onClose, onSubmit, user }: UserFormProps) {
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [role, setRole] = useState<UserRole>(UserRole.User)
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const isEditing = !!user

  useEffect(() => {
    if (isOpen) {
      setUsername(user?.username ?? '')
      setEmail(user?.email ?? '')
      setPassword('')
      setFirstName(user?.firstName ?? '')
      setLastName(user?.lastName ?? '')
      setRole(user?.role ?? UserRole.User)
      setError(null)
    }
  }, [isOpen, user])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!username.trim() || !email.trim()) return

    setIsLoading(true)
    setError(null)
    try {
      if (isEditing) {
        const data: UpdateUserDto = {
          id: user!.id,
          username: username.trim(),
          email: email.trim(),
          firstName: firstName.trim() || undefined,
          lastName: lastName.trim() || undefined,
          role,
        }
        await onSubmit(data)
      } else {
        const data: CreateUserDto = {
          username: username.trim(),
          email: email.trim(),
          password,
          firstName: firstName.trim() || undefined,
          lastName: lastName.trim() || undefined,
          role,
        }
        await onSubmit(data)
      }
      onClose()
    } catch {
      setError('Failed to save user. Please try again.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={isEditing ? 'Edit User' : 'New User'}>
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <div className="grid grid-cols-2 gap-4">
          <div className="flex flex-col gap-1.5">
            <label htmlFor="user-username" className="text-sm font-medium text-ink">
              Username <span className="text-red-500">*</span>
            </label>
            <input
              id="user-username"
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="username"
              required
              className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
            />
          </div>

          <div className="flex flex-col gap-1.5">
            <label htmlFor="user-role" className="text-sm font-medium text-ink">
              Role
            </label>
            <select
              id="user-role"
              value={role}
              onChange={(e) => setRole(Number(e.target.value) as UserRole)}
              className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
            >
              {ROLE_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="flex flex-col gap-1.5">
          <label htmlFor="user-email" className="text-sm font-medium text-ink">
            Email <span className="text-red-500">*</span>
          </label>
          <input
            id="user-email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="user@example.com"
            required
            className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
          />
        </div>

        {!isEditing && (
          <div className="flex flex-col gap-1.5">
            <label htmlFor="user-password" className="text-sm font-medium text-ink">
              Password <span className="text-red-500">*</span>
            </label>
            <input
              id="user-password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Password"
              required={!isEditing}
              minLength={6}
              className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
            />
          </div>
        )}

        <div className="grid grid-cols-2 gap-4">
          <div className="flex flex-col gap-1.5">
            <label htmlFor="user-firstname" className="text-sm font-medium text-ink">
              First Name
            </label>
            <input
              id="user-firstname"
              type="text"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              placeholder="First name"
              className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
            />
          </div>

          <div className="flex flex-col gap-1.5">
            <label htmlFor="user-lastname" className="text-sm font-medium text-ink">
              Last Name
            </label>
            <input
              id="user-lastname"
              type="text"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
              placeholder="Last name"
              className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
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
            disabled={isLoading || !username.trim() || !email.trim()}
            className="rounded-lg bg-accent px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-accent/90 disabled:opacity-50"
          >
            {isLoading ? 'Saving…' : isEditing ? 'Save Changes' : 'Create User'}
          </button>
        </div>
      </form>
    </Modal>
  )
}
