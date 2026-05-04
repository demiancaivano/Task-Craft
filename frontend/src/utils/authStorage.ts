import type { AuthResponse, AuthSession } from '../types/auth'

const AUTH_STORAGE_KEY = 'taskcraft.auth.session'
export const AUTH_SESSION_CHANGED_EVENT = 'taskcraft-auth-session-changed'

function isBrowser() {
  return typeof window !== 'undefined'
}

export function mapAuthResponseToSession(response: AuthResponse): AuthSession {
  return {
    user: {
      id: response.userId,
      username: response.username,
      email: response.email,
      firstName: response.firstName,
      lastName: response.lastName,
    },
    accessToken: response.accessToken,
    refreshToken: response.refreshToken,
    accessTokenExpiration: response.accessTokenExpiration,
    refreshTokenExpiration: response.refreshTokenExpiration,
  }
}

export function getStoredSession(): AuthSession | null {
  if (!isBrowser()) {
    return null
  }

  const rawSession = window.localStorage.getItem(AUTH_STORAGE_KEY)

  if (!rawSession) {
    return null
  }

  try {
    return JSON.parse(rawSession) as AuthSession
  } catch {
    window.localStorage.removeItem(AUTH_STORAGE_KEY)
    return null
  }
}

export function setStoredSession(session: AuthSession) {
  if (!isBrowser()) {
    return
  }

  window.localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(session))
  window.dispatchEvent(new Event(AUTH_SESSION_CHANGED_EVENT))
}

export function clearStoredSession() {
  if (!isBrowser()) {
    return
  }

  window.localStorage.removeItem(AUTH_STORAGE_KEY)
  window.dispatchEvent(new Event(AUTH_SESSION_CHANGED_EVENT))
}

export function getAccessToken() {
  return getStoredSession()?.accessToken ?? null
}

export function getRefreshToken() {
  return getStoredSession()?.refreshToken ?? null
}