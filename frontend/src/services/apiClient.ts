import axios from 'axios'
import type { AxiosError, InternalAxiosRequestConfig } from 'axios'
import type { AuthResponse } from '../types/auth'
import {
  clearStoredSession,
  getAccessToken,
  getRefreshToken,
  mapAuthResponseToSession,
  setStoredSession,
} from '../utils/authStorage'
import { ROUTES } from '../utils/constants'

const configuredBaseUrl = import.meta.env.VITE_API_URL?.trim()
const fallbackBaseUrl = 'https://localhost:7000/api'
const baseURL = configuredBaseUrl || fallbackBaseUrl

if (!configuredBaseUrl) {
  console.warn(
    `VITE_API_URL is not defined. Falling back to ${fallbackBaseUrl}. ` +
      'Define VITE_API_URL in frontend/.env and restart Vite.',
  )
}

export const apiClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
})

const refreshClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
})

let isRefreshing = false
let refreshSubscribers: Array<(token: string | null) => void> = []

function subscribeTokenRefresh(callback: (token: string | null) => void) {
  refreshSubscribers.push(callback)
}

function notifyTokenRefreshed(token: string | null) {
  refreshSubscribers.forEach((callback) => callback(token))
  refreshSubscribers = []
}

function isAuthEndpoint(url?: string) {
  if (!url) {
    return false
  }

  return url.includes('/auth/login') || url.includes('/auth/refresh')
}

function redirectToLogin() {
  if (typeof window !== 'undefined' && window.location.pathname !== ROUTES.login) {
    window.location.assign(ROUTES.login)
  }
}

type RetriableRequestConfig = InternalAxiosRequestConfig & {
  _retry?: boolean
}

apiClient.interceptors.request.use((config) => {
  const token = getAccessToken()

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as RetriableRequestConfig | undefined
    const statusCode = error.response?.status

    if (!originalRequest || statusCode !== 401 || originalRequest._retry || isAuthEndpoint(originalRequest.url)) {
      return Promise.reject(error)
    }

    const refreshToken = getRefreshToken()

    if (!refreshToken) {
      clearStoredSession()
      redirectToLogin()
      return Promise.reject(error)
    }

    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        subscribeTokenRefresh((token) => {
          if (!token) {
            reject(error)
            return
          }

          originalRequest.headers.Authorization = `Bearer ${token}`
          resolve(apiClient(originalRequest))
        })
      })
    }

    originalRequest._retry = true
    isRefreshing = true

    try {
      const { data } = await refreshClient.post<AuthResponse>('/auth/refresh', { refreshToken })
      const nextSession = mapAuthResponseToSession(data)

      setStoredSession(nextSession)
      notifyTokenRefreshed(nextSession.accessToken)

      originalRequest.headers.Authorization = `Bearer ${nextSession.accessToken}`

      return apiClient(originalRequest)
    } catch (refreshError) {
      clearStoredSession()
      notifyTokenRefreshed(null)
      redirectToLogin()
      return Promise.reject(refreshError)
    } finally {
      isRefreshing = false
    }
  },
)