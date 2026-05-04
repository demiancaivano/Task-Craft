import { useState } from 'react'
import type { FormEvent } from 'react'
import { Navigate, useNavigate } from 'react-router-dom'
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
    <section className="relative overflow-hidden rounded-[28px] border border-black/10 bg-white/85 p-6 shadow-card backdrop-blur-md before:pointer-events-none before:absolute before:inset-0 before:bg-[radial-gradient(circle_at_top_right,rgba(255,122,61,0.2),transparent_32%)] before:content-[''] md:p-10">
      <p className="m-0 text-xs uppercase tracking-[0.14em] text-accent">Authentication</p>
      <h2 className="m-0 mt-3 text-3xl font-semibold text-ink md:text-5xl">Sign in to {APP_NAME}</h2>
      <p className="mt-4 max-w-3xl text-muted">
        This form now targets the real backend endpoint at
        {' '}
        <strong>POST /api/auth/login</strong>
        {' '}
        using the contract defined in the .NET API.
      </p>

      <form className="mt-7 grid gap-4" onSubmit={handleSubmit}>
        <label className="grid gap-2">
          <span className="text-sm font-semibold text-ink">Username or email</span>
          <input
            autoComplete="username"
            name="identifier"
            type="text"
            value={identifier}
            onChange={(event) => setIdentifier(event.target.value)}
            placeholder="Enter your username or email"
            className="w-full rounded-2xl border border-black/10 bg-white/90 px-4 py-3 text-ink outline-none transition focus:border-accent/40 focus:ring-2 focus:ring-accent/25"
            required
            minLength={3}
          />
        </label>

        <label className="grid gap-2">
          <span className="text-sm font-semibold text-ink">Password</span>
          <input
            autoComplete="current-password"
            name="password"
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            placeholder="Enter your password"
            className="w-full rounded-2xl border border-black/10 bg-white/90 px-4 py-3 text-ink outline-none transition focus:border-accent/40 focus:ring-2 focus:ring-accent/25"
            required
            minLength={6}
          />
        </label>

        {errorMessage ? <p className="m-0 font-medium text-red-700">{errorMessage}</p> : null}

        <div className="mt-2 flex flex-wrap gap-3">
          <button
            type="submit"
            className="rounded-full bg-gradient-to-br from-accent to-[#ea4335] px-5 py-3 font-semibold text-white shadow-[0_16px_30px_rgba(234,67,53,0.28)] transition hover:-translate-y-0.5 disabled:cursor-wait disabled:opacity-70 disabled:shadow-none"
            disabled={isLoading}
          >
            {isLoading ? 'Signing in...' : 'Sign in'}
          </button>
        </div>
      </form>

      <div className="mt-7 grid grid-cols-1 gap-4 md:grid-cols-2">
        <article className="rounded-3xl border border-black/10 bg-panel p-5">
          <h3>Request payload</h3>
          <p className="mt-2 text-muted">The frontend sends a login identifier (username or email) and password.</p>
        </article>
        <article className="rounded-3xl border border-black/10 bg-panel p-5">
          <h3>Session storage</h3>
          <p className="mt-2 text-muted">Access and refresh tokens are persisted locally for route protection.</p>
        </article>
      </div>
    </section>
  )
}