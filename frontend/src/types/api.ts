export interface ApiErrorResponse {
  statusCode?: number
  errorCode?: string
  message?: string
  details?: string
  path?: string
  timestamp?: string
  errors?: Record<string, string[]>
}