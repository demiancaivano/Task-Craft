export interface LoginRequest {
  identifier: string
  password: string
}

export interface RegisterRequest {
  username: string
  email: string
  password: string
  firstName?: string
  lastName?: string
}

export interface AuthResponse {
  userId: string
  username: string
  email: string
  firstName: string
  lastName: string
  role: number
  accessToken: string
  refreshToken: string
  accessTokenExpiration: string
  refreshTokenExpiration: string
}

export interface RefreshTokenRequest {
  refreshToken: string
}

export interface ValidateTokenResponse {
  isValid: boolean
}

export interface AuthSession {
  user: {
    id: string
    username: string
    email: string
    firstName: string
    lastName: string
    role: number
  }
  accessToken: string
  refreshToken: string
  accessTokenExpiration: string
  refreshTokenExpiration: string
}