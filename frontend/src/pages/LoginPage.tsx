import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { AUTH_MESSAGES, APP_NAME, ROUTES } from '../utils/constants'

export function LoginPage() {
  const navigate = useNavigate()
  const { isAuthenticated, isLoading, login } = useAuth()
  const [identifier, setIdentifier] = useState('')
  const [password, setPassword] = useState('')
  const [errorMessage, setErrorMessage] = useState('')

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setErrorMessage('')

    try {
      await login({ identifier, password })
      navigate(ROUTES.dashboard)
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : AUTH_MESSAGES.invalidCredentials)
    }
  }

  if (isAuthenticated) {
    return <Navigate to={ROUTES.dashboard} replace />
  }

  return (
    <div className="rounded-2xl border border-black/10 bg-white/90 p-8 shadow-card backdrop-blur-md dark:border-white/10 dark:bg-surface/90">
      <div className="mb-6">
        <p className="text-xs font-semibold uppercase tracking-widest text-accent">{APP_NAME}</p>
        <h1 className="mt-1 text-2xl font-semibold text-ink">Sign in</h1>
        <p className="mt-1 text-sm text-muted">Welcome back. Enter your credentials below.</p>
      </div>

      <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
        <label className="flex flex-col gap-1.5">
          <span className="text-sm font-medium text-ink">Username or email</span>
          <input
            autoComplete="username"
            name="identifier"
            type="text"
            value={identifier}
            onChange={(e) => setIdentifier(e.target.value)}
            placeholder="Enter your username or email"
            className="rounded-xl border border-black/10 bg-white px-4 py-2.5 text-sm text-ink outline-none transition focus:border-accent/40 focus:ring-2 focus:ring-accent/20 dark:border-white/10 dark:bg-surface"
            required
            minLength={3}
          />
        </label>

        <label className="flex flex-col gap-1.5">
          <span className="text-sm font-medium text-ink">Password</span>
          <input
            autoComplete="current-password"
            name="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Enter your password"
            className="rounded-xl border border-black/10 bg-white px-4 py-2.5 text-sm text-ink outline-none transition focus:border-accent/40 focus:ring-2 focus:ring-accent/20 dark:border-white/10 dark:bg-surface"
            required
            minLength={6}
          />
        </label>

        {errorMessage && (
          <p className="text-sm font-medium text-red-600">{errorMessage}</p>
        )}

        <button
          type="submit"
          className="mt-1 rounded-xl bg-accent px-4 py-2.5 text-sm font-semibold text-white shadow transition hover:brightness-110 disabled:cursor-wait disabled:opacity-70"
          disabled={isLoading}
        >
          {isLoading ? 'Signing in...' : 'Sign in'}
        </button>
      </form>

      <p className="mt-6 text-center text-sm text-muted">
        Don't have an account?{' '}
        <Link to={ROUTES.register} className="font-medium text-accent hover:underline">
          Create one
        </Link>
      </p>

      <div className="mt-4 flex items-center gap-3">
        <div className="h-px flex-1 bg-black/8" />
        <span className="text-xs text-muted">or</span>
        <div className="h-px flex-1 bg-black/8" />
      </div>

      <Link
        to={ROUTES.projects}
        className="mt-4 block rounded-xl border border-black/10 px-4 py-2.5 text-center text-sm font-medium text-muted transition hover:bg-black/5 hover:text-ink"
      >
        Continue as guest →
      </Link>
    </div>
  )
}