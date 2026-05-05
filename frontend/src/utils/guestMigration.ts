import { projectService } from '../services/projectService'
import { taskService } from '../services/taskService'
import { getGuestProjects, getGuestTasks, clearGuestData } from './guestStorage'

export async function migrateGuestData(): Promise<void> {
  const projects = getGuestProjects()
  if (projects.length === 0) return

  for (const guestProject of projects) {
    try {
      const res = await projectService.create({
        name: guestProject.name,
        description: guestProject.description ?? undefined,
      })
      const newProjectId = res.data.id

      // Migrate top-level tasks only (subtasks not migrated in this version)
      const topLevelTasks = getGuestTasks(guestProject.id).filter((t) => !t.parentTaskId)
      for (const task of topLevelTasks) {
        await taskService.create({
          title: task.title,
          description: task.description ?? undefined,
          status: task.status,
          priority: task.priority,
          startDate: task.startDate ?? undefined,
          dueDate: task.dueDate ?? undefined,
          projectId: newProjectId,
        })
      }
    } catch {
      // If one project fails, continue with the rest
    }
  }

  clearGuestData()
}
