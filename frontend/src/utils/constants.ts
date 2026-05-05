export const APP_NAME = 'TaskCraft'

export const ROUTES = {
  login: '/login',
  register: '/register',
  dashboard: '/dashboard',
  projects: '/projects',
  projectDetail: (id: string) => `/projects/${id}`,
  profile: '/profile',
  users: '/users',
} as const

export const AUTH_MESSAGES = {
  invalidCredentials: 'Invalid username or password.',
} as const