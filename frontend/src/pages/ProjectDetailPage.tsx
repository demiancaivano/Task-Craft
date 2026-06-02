import { useState, useEffect, useCallback, useRef } from 'react'
import { useParams, Link, useNavigate } from 'react-router-dom'
import { KanbanBoard } from '../components/tasks/KanbanBoard'
import { TaskForm } from '../components/tasks/TaskForm'
import { TaskDetailModal } from '../components/tasks/TaskDetailModal'
import { AssignUserModal } from '../components/tasks/AssignUserModal'
import { ProjectMembersModal } from '../components/projects/ProjectMembersModal'
import { ConfirmDialog } from '../components/ui/ConfirmDialog'
import { RoleBadge } from '../components/ui/Badge'
import { projectService } from '../services/projectService'
import { taskService } from '../services/taskService'
import { useAuth } from '../context/AuthContext'
import { ROUTES } from '../utils/constants'
import { ProjectRole } from '../types/project'
import type { ProjectDto, ProjectMemberDto } from '../types/project'
import type { TaskDto } from '../types/task'
import { TaskStatus } from '../types/task'
import {
  getGuestProject,
  getGuestTasks,
  createGuestTask,
  updateGuestTask,
  updateGuestTaskStatus,
  updateGuestTaskPriority,
  deleteGuestTask,
} from '../utils/guestStorage'

export function ProjectDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { isAuthenticated, session } = useAuth()
  const isGuest = !isAuthenticated

  const [project, setProject] = useState<ProjectDto | null>(null)
  const [tasks, setTasks] = useState<TaskDto[]>([])
  const [members, setMembers] = useState<ProjectMemberDto[]>([])
  const [userRole, setUserRole] = useState<ProjectRole | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  // Modal states
  const [selectedTask, setSelectedTask] = useState<TaskDto | null>(null)
  const [taskDetailOpen, setTaskDetailOpen] = useState(false)
  const [taskFormOpen, setTaskFormOpen] = useState(false)
  const [editingTask, setEditingTask] = useState<TaskDto | null>(null)
  const [defaultStatus, setDefaultStatus] = useState<TaskStatus>(TaskStatus.ToDo)
  const [parentTaskId, setParentTaskId] = useState<string | undefined>(undefined)
  const [deletingTask, setDeletingTask] = useState<TaskDto | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)
  const [isLeaving, setIsLeaving] = useState(false)
  const [leaveDialogOpen, setLeaveDialogOpen] = useState(false)
  const [assignModalOpen, setAssignModalOpen] = useState(false)
  const [membersModalOpen, setMembersModalOpen] = useState(false)

  const selectedTaskIdRef = useRef<string | null>(null)

  const fetchData = useCallback(async (silent = false) => {
    if (!id) return
    if (!silent) setIsLoading(true)
    setError(null)
    try {
      if (isGuest) {
        const guestProject = getGuestProject(id)
        if (!guestProject) {
          setError('Project not found.')
          return
        }
        setProject(guestProject)
        const guestTasks = getGuestTasks(id)
        setTasks(guestTasks)
        setMembers([])
        setUserRole(ProjectRole.Manager)

        if (silent && selectedTaskIdRef.current) {
          const updated = guestTasks.find((t) => t.id === selectedTaskIdRef.current)
          if (updated) setSelectedTask(updated)
        }
        return
      }

      const [projectRes, tasksRes, membersRes] = await Promise.all([
        projectService.getById(id),
        taskService.getByProject(id),
        projectService.getMembers(id),
      ])
      setProject(projectRes.data)
      setTasks(tasksRes.data)
      setMembers(membersRes.data)

      if (silent && selectedTaskIdRef.current) {
        const updated = tasksRes.data.find((t) => t.id === selectedTaskIdRef.current)
        if (updated) setSelectedTask(updated)
      }

      const currentUserId = session?.user.id
      const myMember = membersRes.data.find((m) => m.userId === currentUserId)
      if (projectRes.data.ownerId === currentUserId) {
        setUserRole(ProjectRole.Manager)
      } else if (myMember) {
        setUserRole(myMember.role)
      } else {
        setUserRole(null)
      }
    } catch {
      setError('Failed to load project.')
    } finally {
      if (!silent) setIsLoading(false)
    }
  }, [id, isGuest, session?.user.id])

  const silentRefresh = useCallback(() => fetchData(true), [fetchData])

  useEffect(() => {
    fetchData()
  }, [fetchData])

  const handleTaskClick = (task: TaskDto) => {
    selectedTaskIdRef.current = task.id
    setSelectedTask(task)
    setTaskDetailOpen(true)
  }

  const handleAddTask = (status?: TaskStatus) => {
    setEditingTask(null)
    setParentTaskId(undefined)
    setDefaultStatus(status ?? TaskStatus.ToDo)
    setTaskFormOpen(true)
  }

  const handleAddSubtask = (pTaskId: string) => {
    setEditingTask(null)
    setParentTaskId(pTaskId)
    setDefaultStatus(TaskStatus.ToDo)
    setTaskFormOpen(true)
  }

  const handleEditTask = (task: TaskDto) => {
    setEditingTask(task)
    setParentTaskId(undefined)
    setTaskFormOpen(true)
  }

  const handleTaskFormSubmit = async (data: Parameters<typeof taskService.create>[0] | Parameters<typeof taskService.update>[1]) => {
    if (!id) return
    if (isGuest) {
      if (editingTask) {
        updateGuestTask(editingTask.id, data as Parameters<typeof taskService.update>[1])
      } else {
        createGuestTask(
          { ...(data as Parameters<typeof taskService.create>[0]), projectId: id },
          project?.name ?? ''
        )
      }
      await fetchData()
      return
    }
    if (editingTask) {
      await taskService.update(editingTask.id, data as Parameters<typeof taskService.update>[1])
    } else {
      await taskService.create({ ...(data as Parameters<typeof taskService.create>[0]), projectId: id })
    }
    await fetchData()
  }

  const handleDeleteTask = async () => {
    if (!deletingTask) return
    setIsDeleting(true)
    try {
      if (isGuest) {
        deleteGuestTask(deletingTask.id)
        setDeletingTask(null)
        await fetchData()
        return
      }
      await taskService.remove(deletingTask.id)
      setDeletingTask(null)
      await fetchData()
    } finally {
      setIsDeleting(false)
    }
  }

  const handleGuestTaskUpdate = (taskId: string, updates: { status?: TaskStatus; priority?: import('../types/task').TaskPriority }) => {
    if (updates.status !== undefined) updateGuestTaskStatus(taskId, updates.status)
    if (updates.priority !== undefined) updateGuestTaskPriority(taskId, updates.priority)
    silentRefresh()
  }

  const handleLeaveProject = async () => {
    if (!id || isGuest || userRole === ProjectRole.Manager) return

    setIsLeaving(true)
    try {
      await projectService.leave(id)
      navigate(ROUTES.projects, {
        state: { toastMessage: `You left "${project?.name ?? 'the project'}" successfully.` },
      })
    } catch {
      setError('Failed to leave project.')
    } finally {
      setIsLeaving(false)
      setLeaveDialogOpen(false)
    }
  }

  if (isLoading) {
    return <div className="py-12 text-center text-sm text-muted">Loading project…</div>
  }

  if (error || !project) {
    return (
      <div className="py-12 text-center text-sm text-red-600">
        {error ?? 'Project not found.'}
        <button onClick={() => fetchData()} className="ml-2 underline">
          Retry
        </button>
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-6">
      {/* Header */}
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <div className="mb-1 flex items-center gap-2 text-sm text-muted">
            <Link to={ROUTES.projects} className="hover:text-accent hover:underline">
              Projects
            </Link>
            <span>/</span>
            <span className="text-ink">{project.name}</span>
          </div>
          <h1 className="text-2xl font-semibold tracking-tight text-ink">{project.name}</h1>
          {project.description && (
            <p className="mt-1 text-sm text-muted">{project.description}</p>
          )}
          <div className="mt-2 flex flex-wrap items-center gap-3 text-xs text-muted">
            {userRole !== null && <RoleBadge role={userRole} />}
            {!isGuest && (
              <span>Manager: <span className="font-medium text-ink">{members.find((m) => m.userId === project.ownerId)?.username ?? project.ownerUsername ?? '—'}</span></span>
            )}
            {!isGuest && (
              <span>{members.length} member{members.length !== 1 ? 's' : ''}</span>
            )}
            <span>{tasks.filter((t) => !t.parentTaskId).length} task{tasks.filter((t) => !t.parentTaskId).length !== 1 ? 's' : ''}{tasks.filter((t) => t.parentTaskId).length > 0 ? `, ${tasks.filter((t) => t.parentTaskId).length} subtask${tasks.filter((t) => t.parentTaskId).length !== 1 ? 's' : ''}` : ''}</span>
          </div>
        </div>

        <div className="flex gap-2">
          {!isGuest && userRole === ProjectRole.Manager && (
            <button
              type="button"
              onClick={() => setMembersModalOpen(true)}
              className="rounded-lg border border-black/10 px-3 py-2 text-sm font-medium text-ink transition-colors hover:bg-black/5"
            >
              Members
            </button>
          )}
          {!isGuest && userRole !== null && userRole !== ProjectRole.Manager && (
            <button
              type="button"
              onClick={() => setLeaveDialogOpen(true)}
              className="rounded-lg border border-red-200 px-3 py-2 text-sm font-medium text-red-600 transition-colors hover:bg-red-50"
            >
              Leave Project
            </button>
          )}
          {(isGuest || (userRole !== null && userRole <= ProjectRole.Developer)) && (
            <button
              type="button"
              onClick={() => handleAddTask()}
              className="rounded-lg bg-accent px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-accent/90"
            >
              + Add Task
            </button>
          )}
        </div>
      </div>

      {/* Kanban board */}
      <KanbanBoard tasks={tasks} onTaskClick={handleTaskClick} onAddTask={handleAddTask} userRole={userRole} />

      {/* Modals */}
      <TaskDetailModal
        task={selectedTask}
        isOpen={taskDetailOpen}
        onClose={() => setTaskDetailOpen(false)}
        onEdit={handleEditTask}
        onDelete={(task) => { setTaskDetailOpen(false); setDeletingTask(task) }}
        onAddSubtask={handleAddSubtask}
        onAssign={(task) => { setSelectedTask(task); setAssignModalOpen(true) }}
        onRefresh={silentRefresh}
        userRole={userRole}
        isGuest={isGuest}
        guestSubtasks={isGuest ? tasks.filter((t) => t.parentTaskId === selectedTask?.id) : undefined}
        onGuestTaskUpdate={isGuest ? handleGuestTaskUpdate : undefined}
      />

      <TaskForm
        isOpen={taskFormOpen}
        onClose={() => { setTaskFormOpen(false); setEditingTask(null) }}
        onSubmit={handleTaskFormSubmit}
        projectId={id!}
        task={editingTask}
        defaultStatus={defaultStatus}
        parentTaskId={parentTaskId}
      />

      {!isGuest && (
        <AssignUserModal
          isOpen={assignModalOpen}
          onClose={() => setAssignModalOpen(false)}
          task={selectedTask}
          projectId={id!}
          onAssigned={fetchData}
        />
      )}

      {!isGuest && (
        <ProjectMembersModal
          isOpen={membersModalOpen}
          onClose={() => setMembersModalOpen(false)}
          projectId={id!}
        />
      )}

      <ConfirmDialog
        isOpen={!!deletingTask}
        onClose={() => setDeletingTask(null)}
        onConfirm={handleDeleteTask}
        title="Delete Task"
        message={`Are you sure you want to delete "${deletingTask?.title}"?`}
        isLoading={isDeleting}
      />

      <ConfirmDialog
        isOpen={leaveDialogOpen}
        onClose={() => setLeaveDialogOpen(false)}
        onConfirm={handleLeaveProject}
        title="Leave Project"
        message={`Are you sure you want to leave "${project.name}"?`}
        isLoading={isLeaving}
      />
    </div>
  )
}

