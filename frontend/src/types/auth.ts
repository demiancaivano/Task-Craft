export interface LoginRequest {
  identifier: string
  password: string
}

export interface AuthResponse {
  userId: string
  username: string
  email: string
  firstName: string
  lastName: string
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
  }
  accessToken: string
  refreshToken: string
  accessTokenExpiration: string
  refreshTokenExpiration: string
}