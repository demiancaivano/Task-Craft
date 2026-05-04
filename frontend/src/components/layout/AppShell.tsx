import { useAuth } from '../../context/AuthContext'
import { APP_NAME } from '../../utils/constants'

type AppShellProps = {
  children: React.ReactNode
}

export function AppShell({ children }: AppShellProps) {
  const { isAuthenticated, session } = useAuth()

  return (
    <div className="min-h-screen px-5 py-6 md:px-8 md:py-8">
      <div className="mx-auto w-full max-w-6xl">
        <header className="mb-8 flex flex-col items-start gap-4 md:flex-row md:items-center md:justify-between">
          <div className="flex flex-col gap-1">
            <p className="m-0 text-xs uppercase tracking-[0.14em] text-muted">Task management workspace</p>
            <h1 className="m-0 text-4xl font-semibold leading-[0.94] tracking-tight text-ink md:text-6xl">
              {APP_NAME}
            </h1>
          </div>

          <div
            className="inline-flex items-center gap-2 rounded-full border border-black/10 bg-white/70 px-4 py-2 text-sm text-muted backdrop-blur-md"
            aria-live="polite"
          >
            <span
              className="h-2.5 w-2.5 rounded-full bg-accent shadow-[0_0_0_6px_rgba(255,122,61,0.16)]"
              aria-hidden="true"
            ></span>
            {isAuthenticated
              ? `Signed in as ${session?.user.username ?? 'user'}`
              : 'Not authenticated'}
          </div>
        </header>

        <main>{children}</main>
      </div>
    </div>
  )
}