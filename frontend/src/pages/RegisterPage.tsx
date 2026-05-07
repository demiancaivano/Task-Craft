import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { APP_NAME, ROUTES } from '../utils/constants'

export function RegisterPage() {
  const navigate = useNavigate()
  const { isAuthenticated, isLoading, register } = useAuth()
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [errorMessage, setErrorMessage] = useState('')

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setErrorMessage('')

    try {
      await register({
        username,
        email,
        password,
        firstName: firstName.trim(),
        lastName: lastName.trim(),
      })
      navigate(ROUTES.dashboard)
    } catch (error) {
      setErrorMessage(
        error instanceof Error ? error.message : 'Registration failed. Please try again.',
      )
    }
  }

  if (isAuthenticated) {
    return <Navigate to={ROUTES.dashboard} replace />
  }

  return (
    <div className="rounded-2xl border border-black/10 bg-white/90 p-8 shadow-card backdrop-blur-md dark:border-white/10 dark:bg-surface/90">
      <div className="mb-6">
        <p className="text-xs font-semibold uppercase tracking-widest text-accent">{APP_NAME}</p>
        <h1 className="mt-1 text-2xl font-semibold text-ink">Create account</h1>
        <p className="mt-1 text-sm text-muted">Fill in the details below to get started.</p>
      </div>

      <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-3">
          <label className="flex flex-col gap-1.5">
            <span className="text-sm font-medium text-ink">First name</span>
            <input
              name="firstName"
              type="text"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              placeholder="John"
              className="rounded-xl border border-black/10 bg-white px-4 py-2.5 text-sm text-ink outline-none transition focus:border-accent/40 focus:ring-2 focus:ring-accent/20 dark:border-white/10 dark:bg-surface"
              required
            />
          </label>

          <label className="flex flex-col gap-1.5">
            <span className="text-sm font-medium text-ink">Last name</span>
            <input
              name="lastName"
              type="text"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
              placeholder="Doe"
              className="rounded-xl border border-black/10 bg-white px-4 py-2.5 text-sm text-ink outline-none transition focus:border-accent/40 focus:ring-2 focus:ring-accent/20 dark:border-white/10 dark:bg-surface"
              required
            />
          </label>
        </div>

        <label className="flex flex-col gap-1.5">
          <span className="text-sm font-medium text-ink">Username</span>
          <input
            autoComplete="username"
            name="username"
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Choose a username"
            className="rounded-xl border border-black/10 bg-white px-4 py-2.5 text-sm text-ink outline-none transition focus:border-accent/40 focus:ring-2 focus:ring-accent/20 dark:border-white/10 dark:bg-surface"
            required
            minLength={3}
          />
        </label>

        <label className="flex flex-col gap-1.5">
          <span className="text-sm font-medium text-ink">Email</span>
          <input
            autoComplete="email"
            name="email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="your@email.com"
            className="rounded-xl border border-black/10 bg-white px-4 py-2.5 text-sm text-ink outline-none transition focus:border-accent/40 focus:ring-2 focus:ring-accent/20 dark:border-white/10 dark:bg-surface"
            required
          />
        </label>

        <label className="flex flex-col gap-1.5">
          <span className="text-sm font-medium text-ink">Password</span>
          <input
            autoComplete="new-password"
            name="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="At least 8 characters"
            className="rounded-xl border border-black/10 bg-white px-4 py-2.5 text-sm text-ink outline-none transition focus:border-accent/40 focus:ring-2 focus:ring-accent/20 dark:border-white/10 dark:bg-surface"
            required
            minLength={8}
          />
          <p className="text-xs text-muted">Must include uppercase, lowercase, number and special character.</p>
        </label>

        {errorMessage && (
          <p className="text-sm font-medium text-red-600">{errorMessage}</p>
        )}

        <button
          type="submit"
          className="mt-1 rounded-xl bg-accent px-4 py-2.5 text-sm font-semibold text-white shadow transition hover:brightness-110 disabled:cursor-wait disabled:opacity-70"
          disabled={isLoading}
        >
          {isLoading ? 'Creating account...' : 'Create account'}
        </button>
      </form>

      <p className="mt-6 text-center text-sm text-muted">
        Already have an account?{' '}
        <Link to={ROUTES.login} className="font-medium text-accent hover:underline">
          Sign in
        </Link>
      </p>
    </div>
  )
}
