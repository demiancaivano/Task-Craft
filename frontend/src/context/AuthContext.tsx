import { createContext, useContext, useEffect, useState } from 'react'
import { authService } from '../services/authService'
import type { ApiErrorResponse } from '../types/api'
import type { AuthSession, LoginRequest, RegisterRequest } from '../types/auth'
import {
  AUTH_SESSION_CHANGED_EVENT,
  clearStoredSession,
  getStoredSession,
  mapAuthResponseToSession,
  setStoredSession,
} from '../utils/authStorage'
import { migrateGuestData } from '../utils/guestMigration'

type AuthContextValue = {
  session: AuthSession | null
  isAuthenticated: boolean
  isLoading: boolean
  login: (credentials: LoginRequest) => Promise<void>
  register: (data: RegisterRequest) => Promise<void>
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

type AuthProviderProps = {
  children: React.ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [session, setSession] = useState<AuthSession | null>(() => getStoredSession())
  const [isLoading, setIsLoading] = useState(false)

  useEffect(() => {
    const syncSession = () => {
      setSession(getStoredSession())
    }

    window.addEventListener(AUTH_SESSION_CHANGED_EVENT, syncSession)
    window.addEventListener('storage', syncSession)

    return () => {
      window.removeEventListener(AUTH_SESSION_CHANGED_EVENT, syncSession)
      window.removeEventListener('storage', syncSession)
    }
  }, [])

  const login = async (credentials: LoginRequest) => {
    setIsLoading(true)

    try {
      const response = await authService.login(credentials)
      const nextSession = mapAuthResponseToSession(response)
      setStoredSession(nextSession)
      setSession(nextSession)
    } catch (error) {
      const apiError = error as { response?: { data?: ApiErrorResponse & { title?: string } } }
      const body = apiError.response?.data

      if (body?.errors && Object.keys(body.errors).length > 0) {
        const messages = Object.values(body.errors).flat().join(' ')
        throw new Error(messages)
      }

      throw new Error(body?.message ?? body?.title ?? 'Login failed')
    } finally {
      setIsLoading(false)
    }
  }

  const register = async (data: RegisterRequest) => {
    setIsLoading(true)

    try {
      const response = await authService.register(data)
      const nextSession = mapAuthResponseToSession(response)
      setStoredSession(nextSession)
      setSession(nextSession)
      // Migrate any guest data to the new account (fire and forget)
      migrateGuestData().catch(() => {})
    } catch (error) {
      const apiError = error as { response?: { data?: ApiErrorResponse & { title?: string } } }
      const body = apiError.response?.data

      // FluentValidation returns { errors: { Field: ["msg1", "msg2"] } }
      if (body?.errors && Object.keys(body.errors).length > 0) {
        const messages = Object.values(body.errors).flat().join(' ')
        throw new Error(messages)
      }

      throw new Error(body?.message ?? body?.title ?? 'Registration failed')
    } finally {
      setIsLoading(false)
    }
  }

  const logout = async () => {
    const currentSession = getStoredSession()

    setIsLoading(true)

    try {
      if (currentSession?.refreshToken) {
        await authService.revoke({ refreshToken: currentSession.refreshToken })
      }
    } finally {
      clearStoredSession()
      setSession(null)
      setIsLoading(false)
    }
  }

  return (
    <AuthContext.Provider
      value={{
        session,
        isAuthenticated: Boolean(session?.accessToken),
        isLoading,
        login,
        register,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used within AuthProvider')
  }

  return context
}