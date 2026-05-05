import { useState, useEffect } from 'react'
import { Modal } from '../ui/Modal'
import type { CreateProjectDto, ProjectDto, UpdateProjectDto } from '../../types/project'

type ProjectFormProps = {
  isOpen: boolean
  onClose: () => void
  onSubmit: (data: CreateProjectDto | UpdateProjectDto) => Promise<void>
  project?: ProjectDto | null
}

export function ProjectForm({ isOpen, onClose, onSubmit, project }: ProjectFormProps) {
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const isEditing = !!project

  useEffect(() => {
    if (isOpen) {
      setName(project?.name ?? '')
      setDescription(project?.description ?? '')
      setError(null)
    }
  }, [isOpen, project])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!name.trim()) return

    setIsLoading(true)
    setError(null)
    try {
      await onSubmit({ name: name.trim(), description: description.trim() || undefined })
      onClose()
    } catch {
      setError('Failed to save project. Please try again.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEditing ? 'Edit Project' : 'New Project'}
    >
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <div className="flex flex-col gap-1.5">
          <label htmlFor="project-name" className="text-sm font-medium text-ink">
            Name <span className="text-red-500">*</span>
          </label>
          <input
            id="project-name"
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Project name"
            maxLength={200}
            required
            className="w-full rounded-lg border border-black/15 px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent dark:border-white/15 dark:bg-surface"
          />
        </div>

        <div className="flex flex-col gap-1.5">
          <label htmlFor="project-description" className="text-sm font-medium text-ink">
            Description
          </label>
          <textarea
            id="project-description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Optional description"
            rows={3}
            maxLength={1000}
            className="w-full resize-none rounded-lg border border-black/15 px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent dark:border-white/15 dark:bg-surface"
          />
        </div>

        {error && <p className="text-xs text-red-600">{error}</p>}

        <div className="flex justify-end gap-2 pt-1">
          <button
            type="button"
            onClick={onClose}
            disabled={isLoading}
            className="rounded-lg border border-black/10 px-4 py-2 text-sm font-medium text-ink transition-colors hover:bg-black/5 disabled:opacity-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={isLoading || !name.trim()}
            className="rounded-lg bg-accent px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-accent/90 disabled:opacity-50"
          >
            {isLoading ? 'Saving…' : isEditing ? 'Save Changes' : 'Create Project'}
          </button>
        </div>
      </form>
    </Modal>
  )
}
