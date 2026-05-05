import { useState, useEffect, type FormEvent } from 'react'
import { useAuth } from '../context/AuthContext'
import { userService } from '../services/userService'
import { RoleBadge } from '../components/ui/Badge'
import { UserRole } from '../types/user'
import type { UserDto } from '../types/user'

const INPUT_CLASS =
  'w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent dark:border-white/15 dark:bg-surface'

const BTN_PRIMARY =
  'rounded-lg bg-accent px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-accent/90 disabled:opacity-50'

export function MyProfilePage() {
  const { session } = useAuth()
  const [user, setUser] = useState<UserDto | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [loadError, setLoadError] = useState(false)

  // Personal info form
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [isSavingInfo, setIsSavingInfo] = useState(false)
  const [infoSuccess, setInfoSuccess] = useState(false)
  const [infoError, setInfoError] = useState<string | null>(null)

  // Password form
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [isChangingPassword, setIsChangingPassword] = useState(false)
  const [passwordSuccess, setPasswordSuccess] = useState(false)
  const [passwordError, setPasswordError] = useState<string | null>(null)

  useEffect(() => {
    if (!session) return
    setIsLoading(true)
    userService
      .getById(session.user.id)
      .then((res) => {
        const u = res.data
        setUser(u)
        setUsername(u.username)
        setEmail(u.email)
        setFirstName(u.firstName ?? '')
        setLastName(u.lastName ?? '')
      })
      .catch(() => setLoadError(true))
      .finally(() => setIsLoading(false))
  }, [session])

  const handleSaveInfo = async (e: FormEvent) => {
    e.preventDefault()
    if (!user) return
    setIsSavingInfo(true)
    setInfoError(null)
    setInfoSuccess(false)
    try {
      const res = await userService.update(user.id, {
        id: user.id,
        username: username.trim(),
        email: email.trim(),
        firstName: firstName.trim() || undefined,
        lastName: lastName.trim() || undefined,
        role: user.role,
      })
      setUser(res.data)
      setInfoSuccess(true)
      setTimeout(() => setInfoSuccess(false), 3000)
    } catch {
      setInfoError('Failed to save changes. Please try again.')
    } finally {
      setIsSavingInfo(false)
    }
  }

  const handleChangePassword = async (e: FormEvent) => {
    e.preventDefault()
    if (!user) return
    setPasswordError(null)
    setPasswordSuccess(false)

    if (newPassword.length < 6) {
      setPasswordError('Password must be at least 6 characters.')
      return
    }
    if (newPassword !== confirmPassword) {
      setPasswordError('Passwords do not match.')
      return
    }

    setIsChangingPassword(true)
    try {
      await userService.update(user.id, {
        id: user.id,
        username: user.username,
        email: user.email,
        firstName: user.firstName ?? undefined,
        lastName: user.lastName ?? undefined,
        role: user.role,
        newPassword,
      })
      setNewPassword('')
      setConfirmPassword('')
      setPasswordSuccess(true)
      setTimeout(() => setPasswordSuccess(false), 3000)
    } catch {
      setPasswordError('Failed to update password. Please try again.')
    } finally {
      setIsChangingPassword(false)
    }
  }

  const initials = user
    ? `${user.firstName?.[0] ?? user.username[0]}${user.lastName?.[0] ?? ''}`.toUpperCase()
    : '?'

  const memberSince = user
    ? new Date(user.createdAt).toLocaleDateString('en-US', { month: 'long', year: 'numeric' })
    : ''

  return (
    <div className="mx-auto flex max-w-2xl flex-col gap-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight text-ink">My Profile</h1>
        <p className="text-sm text-muted">Manage your personal information and password</p>
      </div>

      {isLoading && (
        <p className="text-sm text-muted">Loading...</p>
      )}

      {!isLoading && loadError && (
        <p className="text-sm text-red-500">Failed to load profile data.</p>
      )}

      {!isLoading && user && (
        <>
          {/* Profile summary card */}
          <div className="flex items-center gap-4 rounded-xl border border-black/10 bg-white p-5 dark:border-white/10 dark:bg-surface">
            <div className="flex h-16 w-16 shrink-0 items-center justify-center rounded-full bg-accent/15 text-2xl font-bold text-accent">
              {initials}
            </div>
            <div className="flex min-w-0 flex-col gap-1">
              <div className="flex items-center gap-2">
                <span className="truncate text-lg font-semibold text-ink">{user.username}</span>
                <RoleBadge role={user.role === UserRole.Admin ? 1 : 2} variant="user" />
              </div>
              <span className="truncate text-sm text-muted">{user.email}</span>
              <span className="text-xs text-muted">Member since {memberSince}</span>
            </div>
          </div>

          {/* Personal info form */}
          <div className="rounded-xl border border-black/10 bg-white p-5 dark:border-white/10 dark:bg-surface">
            <h2 className="mb-4 text-base font-semibold text-ink">Personal Information</h2>
            <form onSubmit={handleSaveInfo} className="flex flex-col gap-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-sm font-medium text-ink">
                    Username <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="text"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                    className={INPUT_CLASS}
                  />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-sm font-medium text-ink">
                    Email <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                    className={INPUT_CLASS}
                  />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-sm font-medium text-ink">First Name</label>
                  <input
                    type="text"
                    value={firstName}
                    onChange={(e) => setFirstName(e.target.value)}
                    placeholder="First name"
                    className={INPUT_CLASS}
                  />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-sm font-medium text-ink">Last Name</label>
                  <input
                    type="text"
                    value={lastName}
                    onChange={(e) => setLastName(e.target.value)}
                    placeholder="Last name"
                    className={INPUT_CLASS}
                  />
                </div>
              </div>

              {infoError && <p className="text-sm text-red-500">{infoError}</p>}
              {infoSuccess && <p className="text-sm text-green-600">Changes saved successfully.</p>}

              <div className="flex justify-end">
                <button type="submit" disabled={isSavingInfo} className={BTN_PRIMARY}>
                  {isSavingInfo ? 'Saving...' : 'Save Changes'}
                </button>
              </div>
            </form>
          </div>

          {/* Change password */}
          <div className="rounded-xl border border-black/10 bg-white p-5 dark:border-white/10 dark:bg-surface">
            <h2 className="mb-1 text-base font-semibold text-ink">Change Password</h2>
            <p className="mb-4 text-sm text-muted">Leave blank to keep your current password.</p>
            <form onSubmit={handleChangePassword} className="flex flex-col gap-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="flex flex-col gap-1.5">
                  <label className="text-sm font-medium text-ink">New Password</label>
                  <input
                    type="password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    placeholder="Min. 6 characters"
                    required
                    className={INPUT_CLASS}
                  />
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-sm font-medium text-ink">Confirm Password</label>
                  <input
                    type="password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    placeholder="Repeat new password"
                    required
                    className={INPUT_CLASS}
                  />
                </div>
              </div>

              {passwordError && <p className="text-sm text-red-500">{passwordError}</p>}
              {passwordSuccess && <p className="text-sm text-green-600">Password updated successfully.</p>}

              <div className="flex justify-end">
                <button type="submit" disabled={isChangingPassword} className={BTN_PRIMARY}>
                  {isChangingPassword ? 'Updating...' : 'Update Password'}
                </button>
              </div>
            </form>
          </div>
        </>
      )}
    </div>
  )
}
