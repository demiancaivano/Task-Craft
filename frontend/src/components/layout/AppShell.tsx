import { useState, useEffect } from 'react'
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
  const [mobileOpen, setMobileOpen] = useState(false)
  const isAuthPage =
    location.pathname === ROUTES.login || location.pathname === ROUTES.register
  const isAdmin = session?.user.role === UserRole.Admin
  const authNavItems = AUTH_NAV_ITEMS.filter((item) => !item.adminOnly || isAdmin)

  useEffect(() => {
    setMobileOpen(false)
  }, [location.pathname])

  const handleLogout = async () => {
    await logout()
    navigate(ROUTES.login)
  }

  const navLinkClass = ({ isActive }: { isActive: boolean }) =>
    `flex items-center gap-2.5 rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
      isActive
        ? 'bg-accent/10 text-accent'
        : 'text-muted hover:bg-black/5 hover:text-ink dark:hover:bg-white/8'
    }`

  const ThemeButton = () => (
    <button
      type="button"
      onClick={toggleTheme}
      aria-label={theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'}
      className="rounded-lg p-1.5 text-muted transition-colors hover:bg-black/5 hover:text-ink dark:hover:bg-white/8 dark:hover:text-ink"
    >
      {theme === 'dark' ? (
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
          <circle cx="12" cy="12" r="4" />
          <path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M6.34 17.66l-1.41 1.41M19.07 4.93l-1.41 1.41" />
        </svg>
      ) : (
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
          <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
        </svg>
      )}
    </button>
  )

  const renderSidebarContent = (onLinkClick?: () => void) => (
    <>
      <div className="flex items-center justify-between px-2">
        <div className="flex flex-col gap-0.5">
          <p className="text-[10px] uppercase tracking-widest text-muted">Workspace</p>
          <span className="text-xl font-semibold tracking-tight text-ink">{APP_NAME}</span>
        </div>
        <ThemeButton />
      </div>

      <nav className="flex flex-col gap-1">
        {isAuthenticated
          ? authNavItems.map(({ label, to, icon }) => (
              <NavLink key={to} to={to} className={navLinkClass} onClick={onLinkClick}>
                <span aria-hidden="true">{icon}</span>
                {label}
              </NavLink>
            ))
          : GUEST_NAV_ITEMS.map(({ label, to, icon }) => (
              <NavLink key={to} to={to} className={navLinkClass} onClick={onLinkClick}>
                <span aria-hidden="true">{icon}</span>
                {label}
              </NavLink>
            ))}
      </nav>

      <div className="mt-auto flex flex-col gap-2">
        {isAuthenticated ? (
          <>
            <NavLink
              to={ROUTES.profile}
              onClick={onLinkClick}
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
              onClick={async () => { onLinkClick?.(); await handleLogout() }}
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
              onClick={onLinkClick}
              className="rounded-lg bg-accent px-3 py-1.5 text-center text-xs font-medium text-white transition-colors hover:bg-accent/90"
            >
              Create account
            </NavLink>
            <NavLink
              to={ROUTES.login}
              onClick={onLinkClick}
              className="rounded-lg border border-black/10 px-3 py-1.5 text-center text-xs font-medium text-ink transition-colors hover:bg-black/5 dark:border-white/10 dark:hover:bg-white/5"
            >
              Sign in
            </NavLink>
          </div>
        )}
      </div>
    </>
  )

  if (isAuthPage) {
    return (
      <div className="flex min-h-screen items-center justify-center p-4">
        <div className="w-full max-w-md">{children}</div>
      </div>
    )
  }

  return (
    <div className="flex min-h-screen">
      {/* Desktop sidebar */}
      <aside className="sticky top-0 hidden h-screen w-60 flex-shrink-0 flex-col gap-6 overflow-y-auto border-r border-black/8 bg-white px-4 py-6 dark:border-white/8 dark:bg-[#13131c] md:flex">
        {renderSidebarContent()}
      </aside>

      {/* Mobile drawer overlay */}
      {mobileOpen && (
        <div className="fixed inset-0 z-40 md:hidden">
          <div
            className="absolute inset-0 bg-black/50 backdrop-blur-sm"
            onClick={() => setMobileOpen(false)}
            aria-hidden="true"
          />
          <aside className="absolute left-0 top-0 flex h-full w-64 flex-col gap-6 overflow-y-auto bg-white px-4 py-6 shadow-xl dark:bg-[#13131c]">
            {renderSidebarContent(() => setMobileOpen(false))}
          </aside>
        </div>
      )}

      {/* Main content */}
      <div className="flex min-w-0 flex-1 flex-col overflow-hidden">
        {/* Mobile top bar */}
        <header className="flex shrink-0 items-center justify-between border-b border-black/8 bg-white px-4 py-3 dark:border-white/8 dark:bg-[#13131c] md:hidden">
          <span className="text-lg font-semibold tracking-tight text-ink">{APP_NAME}</span>
          <div className="flex items-center gap-1">
            <ThemeButton />
            <button
              type="button"
              onClick={() => setMobileOpen(true)}
              aria-label="Open navigation"
              className="rounded-lg p-1.5 text-muted transition-colors hover:bg-black/5 hover:text-ink dark:hover:bg-white/8"
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                <line x1="3" y1="6" x2="21" y2="6" />
                <line x1="3" y1="12" x2="21" y2="12" />
                <line x1="3" y1="18" x2="21" y2="18" />
              </svg>
            </button>
          </div>
        </header>

        <main className="flex-1 overflow-y-auto px-4 py-4 md:px-6 md:py-6">{children}</main>
      </div>
    </div>
  )
}
