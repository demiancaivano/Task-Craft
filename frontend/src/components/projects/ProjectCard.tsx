import { Link } from 'react-router-dom'
import type { ProjectDto } from '../../types/project'
import { ProjectRole } from '../../types/project'
import { RoleBadge } from '../ui/Badge'
import { ROUTES } from '../../utils/constants'

const ROLE_LABEL_MAP: Record<string, ProjectRole> = {
  Manager: ProjectRole.Manager,
  Developer: ProjectRole.Developer,
  Member: ProjectRole.Member,
  Viewer: ProjectRole.Viewer,
}

type ProjectCardProps = {
  project: ProjectDto
  onEdit: (project: ProjectDto) => void
  onDelete: (project: ProjectDto) => void
}

export function ProjectCard({ project, onEdit, onDelete }: ProjectCardProps) {
  return (
    <div className="flex flex-col gap-3 rounded-xl border border-black/8 bg-white p-5 transition-shadow hover:shadow-md dark:border-white/8 dark:bg-surface dark:hover:shadow-black/30">
      <div className="flex items-start justify-between gap-2">
        <Link
          to={ROUTES.projectDetail(project.id)}
          className="text-base font-semibold text-ink hover:text-accent"
        >
          {project.name}
        </Link>
        <div className="flex shrink-0 items-center gap-2">
          {project.currentUserRole && ROLE_LABEL_MAP[project.currentUserRole] !== undefined && (
            <RoleBadge role={ROLE_LABEL_MAP[project.currentUserRole]} />
          )}
          <div className="flex shrink-0 gap-1">
            <button
              type="button"
              onClick={() => onEdit(project)}
              className="rounded-md px-2 py-1 text-xs text-muted transition-colors hover:bg-black/5 hover:text-ink dark:hover:bg-white/8"
            >
              Edit
            </button>
            <button
              type="button"
              onClick={() => onDelete(project)}
              className="rounded-md px-2 py-1 text-xs text-muted transition-colors hover:bg-red-50 hover:text-red-600 dark:hover:bg-red-900/20 dark:hover:text-red-400"
            >
              Delete
            </button>
          </div>
        </div>
      </div>
      {project.description && (
        <p className="line-clamp-2 text-sm text-muted">{project.description}</p>
      )}

      <div className="flex items-center gap-4 border-t border-black/5 pt-3 text-xs text-muted dark:border-white/5">
        <span>
          Manager: <span className="font-medium text-ink">{project.ownerUsername}</span>
        </span>
        <span>{project.memberCount} member{project.memberCount !== 1 ? 's' : ''}</span>
        <span>
          {project.taskCount} task{project.taskCount !== 1 ? 's' : ''}
          {project.subTaskCount > 0 && `, ${project.subTaskCount} subtask${project.subTaskCount !== 1 ? 's' : ''}`}
        </span>
      </div>
    </div>
  )
}
