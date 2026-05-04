import { Link } from 'react-router-dom'
import { ROUTES } from '../utils/constants'

export function NotFoundPage() {
  return (
    <section className="relative overflow-hidden rounded-[28px] border border-black/10 bg-white/85 p-6 shadow-card backdrop-blur-md before:pointer-events-none before:absolute before:inset-0 before:bg-[radial-gradient(circle_at_top_right,rgba(255,122,61,0.2),transparent_32%)] before:content-[''] md:p-10">
      <p className="m-0 text-xs uppercase tracking-[0.14em] text-accent">404</p>
      <h2 className="m-0 mt-3 text-3xl font-semibold text-ink md:text-5xl">Route not found</h2>
      <p className="mt-4 max-w-3xl text-muted">
        The requested route does not exist in this frontend version.
      </p>

      <div className="mt-7 flex flex-wrap gap-3">
        <Link
          className="rounded-full bg-gradient-to-br from-accent to-[#ea4335] px-5 py-3 font-semibold text-white shadow-[0_16px_30px_rgba(234,67,53,0.28)] transition hover:-translate-y-0.5"
          to={ROUTES.login}
        >
          Back to login
        </Link>
      </div>
    </section>
  )
}