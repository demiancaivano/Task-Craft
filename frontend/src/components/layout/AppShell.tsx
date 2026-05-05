import { NavLink, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import { useTheme } from '../../context/ThemeContext'
import { APP_NAME, ROUTES } from '../../utils/constants'
import { UserRole } from '../../types/user'

type AppShellProps = {
  children: React.ReactNode
}

const AUTH_NAV_ITEMS = [
  { label: 'Dashboard', to: ROUTES.dashboard, icon: '⊞', adminOnly: false },
  { label: 'Projects', to: ROUTES.projects, icon: '◫', adminOnly: false },
  { label: 'My Profile', to: ROUTES.profile, icon: '◉', adminOnly: false },
  { label: 'Users', to: ROUTES.users, icon: '◎', adminOnly: true },
] as const

const GUEST_NAV_ITEMS = [
  { label: 'Projects', to: ROUTES.projects, icon: '◫' },
] as const

export function AppShell({ children }: AppShellProps) {
  const { isAuthenticated, session, logout } = useAuth()
  const { theme, toggleTheme } = useTheme()
  const navigate = useNavigate()
  const location = useLocation()
  const isAuthPage =
    location.pathname === ROUTES.login || location.pathname === ROUTES.register
  const isAdmin = session?.user.role === UserRole.Admin
  const authNavItems = AUTH_NAV_ITEMS.filter((item) => !item.adminOnly || isAdmin)

  const handleLogout = async () => {
    await logout()
    navigate(ROUTES.login)
  }

  if (isAuthPage) {
    return (
      <div className="flex min-h-screen items-center justify-center p-4">
        <div className="w-full max-w-md">{children}</div>
      </div>
    )
  }

  return (
    <div className="flex min-h-screen">
      {/* Sidebar */}
      <aside className="sticky top-0 flex h-screen w-60 flex-shrink-0 flex-col gap-6 overflow-y-auto border-r border-black/8 bg-white px-4 py-6 dark:border-white/8 dark:bg-[#13131c]">
        <div className="flex items-center justify-between px-2">
          <div className="flex flex-col gap-0.5">
            <p className="text-[10px] uppercase tracking-widest text-muted">Workspace</p>
            <span className="text-xl font-semibold tracking-tight text-ink">{APP_NAME}</span>
          </div>
          <button
            type="button"
            onClick={toggleTheme}
            aria-label={theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'}
            className="rounded-lg p-1.5 text-muted transition-colors hover:bg-black/5 hover:text-ink dark:hover:bg-white/8 dark:hover:text-ink"
          >
            {theme === 'dark' ? (
              /* Sun icon */
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                <circle cx="12" cy="12" r="4" />
                <path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M6.34 17.66l-1.41 1.41M19.07 4.93l-1.41 1.41" />
              </svg>
            ) : (
              /* Moon icon */
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
              </svg>
            )}
          </button>
        </div>

        <nav className="flex flex-col gap-1">
          {isAuthenticated
            ? authNavItems.map(({ label, to, icon }) => (
                <NavLink
                  key={to}
                  to={to}
                  className={({ isActive }) =>
                    `flex items-center gap-2.5 rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
                      isActive
                        ? 'bg-accent/10 text-accent'
                        : 'text-muted hover:bg-black/5 hover:text-ink dark:hover:bg-white/8'
                    }`
                  }
                >
                  <span aria-hidden="true">{icon}</span>
                  {label}
                </NavLink>
              ))
            : GUEST_NAV_ITEMS.map(({ label, to, icon }) => (
                <NavLink
                  key={to}
                  to={to}
                  className={({ isActive }) =>
                    `flex items-center gap-2.5 rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
                      isActive
                        ? 'bg-accent/10 text-accent'
                        : 'text-muted hover:bg-black/5 hover:text-ink dark:hover:bg-white/8'
                    }`
                  }
                >
                  <span aria-hidden="true">{icon}</span>
                  {label}
                </NavLink>
              ))}
        </nav>

        {/* Bottom section */}
        <div className="mt-auto flex flex-col gap-2">
          {isAuthenticated ? (
            <>
              <NavLink
                to={ROUTES.profile}
                className="flex flex-col gap-1 rounded-lg border border-black/8 px-3 py-3 transition-colors hover:bg-black/5 dark:border-white/8 dark:hover:bg-white/5"
              >
                <div className="flex items-center gap-2 text-xs text-muted" aria-live="polite">
                  <span className="h-2 w-2 rounded-full bg-green-500" aria-hidden="true" />
                  Signed in
                </div>
                {session && (
                  <p className="truncate text-sm font-medium text-ink">{session.user.username}</p>
                )}
              </NavLink>

              <button
                type="button"
                onClick={handleLogout}
                className="flex w-full items-center gap-2.5 rounded-lg px-3 py-2 text-sm font-medium text-muted transition-colors hover:bg-red-50 hover:text-red-600 dark:hover:bg-red-900/20 dark:hover:text-red-400"
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                  <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
                  <polyline points="16 17 21 12 16 7" />
                  <line x1="21" y1="12" x2="9" y2="12" />
                </svg>
                Sign out
              </button>
            </>
          ) : (
            <div className="flex flex-col gap-2 rounded-lg border border-black/8 p-3 dark:border-white/8">
              <p className="text-xs text-muted">
                You're in <span className="font-medium text-ink">guest mode</span>. Sign up to sync and collaborate.
              </p>
              <NavLink
                to={ROUTES.register}
                className="rounded-lg bg-accent px-3 py-1.5 text-center text-xs font-medium text-white transition-colors hover:bg-accent/90"
              >
                Create account
              </NavLink>
              <NavLink
                to={ROUTES.login}
                className="rounded-lg border border-black/10 px-3 py-1.5 text-center text-xs font-medium text-ink transition-colors hover:bg-black/5 dark:border-white/10 dark:hover:bg-white/5"
              >
                Sign in
              </NavLink>
            </div>
          )}
        </div>
      </aside>

      {/* Main content */}
      <div className="flex flex-1 flex-col overflow-hidden">
        <main className="flex-1 overflow-y-auto px-6 py-6">{children}</main>
      </div>
    </div>
  )
}
