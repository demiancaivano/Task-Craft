export const UserRole = {
  Admin: 1,
  User: 2,
} as const
export type UserRole = (typeof UserRole)[keyof typeof UserRole]

export interface UserDto {
  id: string
  username: string
  email: string
  firstName: string | null
  lastName: string | null
  role: UserRole
  isAnonymous: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateUserDto {
  username: string
  email: string
  password: string
  firstName?: string
  lastName?: string
  role?: UserRole
}

export interface UpdateUserDto {
  id: string
  username: string
  email: string
  firstName?: string
  lastName?: string
  role?: UserRole
  newPassword?: string
}
