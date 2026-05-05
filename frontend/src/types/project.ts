export const ProjectRole = {
  Manager: 1,
  Developer: 2,
  Member: 3,
  Viewer: 4,
} as const
export type ProjectRole = (typeof ProjectRole)[keyof typeof ProjectRole]

export interface ProjectDto {
  id: string
  name: string
  description: string | null
  ownerId: string
  ownerUsername: string | null
  memberCount: number
  taskCount: number
  subTaskCount: number
  currentUserRole: string | null
  createdAt: string
  updatedAt: string
  createdBy: string | null
  updatedBy: string | null
}

export interface ProjectMemberDto {
  userId: string
  username: string
  email: string
  firstName: string | null
  lastName: string | null
  role: ProjectRole
  joinedAt: string
}

export interface CreateProjectDto {
  name: string
  description?: string
}

export interface UpdateProjectDto {
  id: string
  name: string
  description?: string
}

export interface AddProjectMemberDto {
  userId: string
  role: ProjectRole
}
