import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { ROUTES } from '../utils/constants'

export function DashboardPage() {
  const navigate = useNavigate()
  const { logout, session } = useAuth()

  const handleLogout = async () => {
    await logout()
    navigate(ROUTES.login)
  }

  return (
    <div className="grid gap-6">
      <section className="relative overflow-hidden rounded-[28px] border border-black/10 bg-white/85 p-6 shadow-card backdrop-blur-md before:pointer-events-none before:absolute before:inset-0 before:bg-[radial-gradient(circle_at_top_right,rgba(255,122,61,0.2),transparent_32%)] before:content-[''] md:p-10">
        <p className="m-0 text-xs uppercase tracking-[0.14em] text-accent">Dashboard</p>
        <h2 className="m-0 mt-3 text-3xl font-semibold text-ink md:text-5xl">Authenticated area</h2>
        <p className="mt-4 max-w-3xl text-muted">
          This route is now protected by the session stored after login.
          The authenticated user is <strong>{session?.user.username}</strong>.
        </p>

        <div className="mt-7 grid grid-cols-1 gap-4 md:grid-cols-3">
          <article className="rounded-3xl border border-black/10 bg-panel p-5">
            <h3>User</h3>
            <p className="mt-2 text-muted">{session?.user.firstName} {session?.user.lastName}</p>
          </article>
          <article className="rounded-3xl border border-black/10 bg-panel p-5">
            <h3>Email</h3>
            <p className="mt-2 text-muted">{session?.user.email}</p>
          </article>
          <article className="rounded-3xl border border-black/10 bg-panel p-5">
            <h3>Token expiry</h3>
            <p className="mt-2 text-muted">{new Date(session?.accessTokenExpiration ?? '').toLocaleString()}</p>
          </article>
        </div>

        <div className="mt-7 flex flex-wrap gap-3">
          <button
            type="button"
            className="rounded-full bg-black/10 px-5 py-3 font-semibold text-ink transition hover:-translate-y-0.5"
            onClick={handleLogout}
          >
            Sign out
          </button>
        </div>
      </section>
    </div>
  )
}