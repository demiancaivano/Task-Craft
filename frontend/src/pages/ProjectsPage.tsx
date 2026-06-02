import { useState, useEffect, useCallback } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { ProjectCard } from '../components/projects/ProjectCard'
import { ProjectForm } from '../components/projects/ProjectForm'
import { ConfirmDialog } from '../components/ui/ConfirmDialog'
import { Toast } from '../components/ui/Toast'
import { projectService } from '../services/projectService'
import { useAuth } from '../context/AuthContext'
import {
  getGuestProjects,
  createGuestProject,
  updateGuestProject,
  deleteGuestProject,
} from '../utils/guestStorage'
import type { CreateProjectDto, ProjectDto, UpdateProjectDto } from '../types/project'

export function ProjectsPage() {
  const location = useLocation()
  const navigate = useNavigate()
  const { isAuthenticated } = useAuth()
  const [projects, setProjects] = useState<ProjectDto[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [formOpen, setFormOpen] = useState(false)
  const [editingProject, setEditingProject] = useState<ProjectDto | null>(null)
  const [deletingProject, setDeletingProject] = useState<ProjectDto | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)
  const [leavingProject, setLeavingProject] = useState<ProjectDto | null>(null)
  const [isLeaving, setIsLeaving] = useState(false)
  const [toastMessage, setToastMessage] = useState<string | null>(null)

  useEffect(() => {
    const state = location.state as { toastMessage?: string } | null
    if (!state?.toastMessage) return

    setToastMessage(state.toastMessage)
    navigate(location.pathname, { replace: true, state: {} })
  }, [location.pathname, location.state, navigate])

  const fetchProjects = useCallback(async () => {
    setIsLoading(true)
    setError(null)
    try {
      if (!isAuthenticated) {
        setProjects(getGuestProjects())
        return
      }
      const response = await projectService.getAll()
      setProjects(response.data)
    } catch {
      setError('Failed to load projects.')
    } finally {
      setIsLoading(false)
    }
  }, [isAuthenticated])

  useEffect(() => {
    fetchProjects()
  }, [fetchProjects])

  const handleCreate = async (data: CreateProjectDto | UpdateProjectDto) => {
    if (!isAuthenticated) {
      createGuestProject(data as CreateProjectDto)
      setProjects(getGuestProjects())
      return
    }
    await projectService.create(data as CreateProjectDto)
    await fetchProjects()
  }

  const handleEdit = async (data: CreateProjectDto | UpdateProjectDto) => {
    if (!editingProject) return
    if (!isAuthenticated) {
      updateGuestProject(editingProject.id, { ...(data as UpdateProjectDto), id: editingProject.id })
      setProjects(getGuestProjects())
      return
    }
    await projectService.update(editingProject.id, { ...(data as UpdateProjectDto), id: editingProject.id })
    await fetchProjects()
  }

  const handleDelete = async () => {
    if (!deletingProject) return
    setIsDeleting(true)
    try {
      if (!isAuthenticated) {
        deleteGuestProject(deletingProject.id)
        setDeletingProject(null)
        setProjects(getGuestProjects())
        return
      }
      await projectService.remove(deletingProject.id)
      setDeletingProject(null)
      await fetchProjects()
    } finally {
      setIsDeleting(false)
    }
  }

  const handleLeave = async () => {
    if (!leavingProject || !isAuthenticated) return

    setIsLeaving(true)
    try {
      await projectService.leave(leavingProject.id)
      setToastMessage(`You left "${leavingProject.name}" successfully.`)
      setLeavingProject(null)
      await fetchProjects()
    } catch {
      setError('Failed to leave project.')
    } finally {
      setIsLeaving(false)
    }
  }

  const openEdit = (project: ProjectDto) => {
    setEditingProject(project)
    setFormOpen(true)
  }

  const closeForm = () => {
    setFormOpen(false)
    setEditingProject(null)
  }

  return (
    <div className="flex flex-col gap-6">
      {toastMessage && (
        <Toast message={toastMessage} onClose={() => setToastMessage(null)} />
      )}

      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-ink">Projects</h1>
          <p className="text-sm text-muted">Manage your task management projects</p>
        </div>
        <button
          type="button"
          onClick={() => setFormOpen(true)}
          className="rounded-lg bg-accent px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-accent/90"
        >
          + New Project
        </button>
      </div>

      {isLoading && (
        <div className="py-12 text-center text-sm text-muted">Loading projects…</div>
      )}

      {error && (
        <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-600">
          {error}
          <button onClick={fetchProjects} className="ml-2 underline">
            Retry
          </button>
        </div>
      )}

      {!isLoading && !error && projects.length === 0 && (
        <div className="rounded-xl border border-dashed border-black/15 py-16 text-center">
          <p className="text-sm text-muted">No projects yet.</p>
          <button
            type="button"
            onClick={() => setFormOpen(true)}
            className="mt-3 text-sm font-medium text-accent hover:underline"
          >
            Create your first project
          </button>
        </div>
      )}

      {!isLoading && projects.length > 0 && (
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
          {projects.map((project) => (
            <ProjectCard
              key={project.id}
              project={project}
              onEdit={openEdit}
              onDelete={setDeletingProject}
              onLeave={setLeavingProject}
            />
          ))}
        </div>
      )}

      <ProjectForm
        isOpen={formOpen}
        onClose={closeForm}
        onSubmit={editingProject ? handleEdit : handleCreate}
        project={editingProject}
      />

      <ConfirmDialog
        isOpen={!!deletingProject}
        onClose={() => setDeletingProject(null)}
        onConfirm={handleDelete}
        title="Delete Project"
        message={`Are you sure you want to delete "${deletingProject?.name}"? This action cannot be undone.`}
        isLoading={isDeleting}
      />

      <ConfirmDialog
        isOpen={!!leavingProject}
        onClose={() => setLeavingProject(null)}
        onConfirm={handleLeave}
        title="Leave Project"
        message={`Are you sure you want to leave "${leavingProject?.name}"?`}
        isLoading={isLeaving}
      />
    </div>
  )
}
