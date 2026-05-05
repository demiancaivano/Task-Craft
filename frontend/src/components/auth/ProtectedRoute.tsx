import { Navigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import { ROUTES } from '../../utils/constants'

type ProtectedRouteProps = {
  children: React.ReactNode
  requiredRole?: number
}

export function ProtectedRoute({ children, requiredRole }: ProtectedRouteProps) {
  const { isAuthenticated, session } = useAuth()

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.login} replace />
  }

  if (requiredRole !== undefined && session?.user.role !== requiredRole) {
    return <Navigate to={ROUTES.dashboard} replace />
  }

  return children
}