export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface ApiErrorResponse {
  statusCode?: number
  errorCode?: string
  message?: string
  details?: string
  path?: string
  timestamp?: string
  errors?: Record<string, string[]>
}