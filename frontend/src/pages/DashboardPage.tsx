import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { projectService } from '../services/projectService'
import { taskService } from '../services/taskService'
import { ROUTES } from '../utils/constants'
import { StatusBadge, PriorityBadge } from '../components/ui/Badge'
import type { ProjectDto } from '../types/project'
import type { TaskDto } from '../types/task'
import { TaskStatus } from '../types/task'

function getGreeting() {
  const hour = new Date().getHours()
  if (hour < 12) return 'Good morning'
  if (hour < 18) return 'Good afternoon'
  return 'Good evening'
}

function isThisWeek(dateStr: string) {
  const date = new Date(dateStr)
  const now = new Date()
  const weekAgo = new Date(now)
  weekAgo.setDate(now.getDate() - 7)
  return date >= weekAgo && date <= now
}

export function DashboardPage() {
  const { session } = useAuth()
  const userId = session?.user.id ?? ''
  const displayName = session?.user.firstName || session?.user.username || 'there'

  const [projects, setProjects] = useState<ProjectDto[]>([])
  const [tasks, setTasks] = useState<TaskDto[]>([])
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    if (!userId) return
    Promise.all([
      projectService.getByUser(userId),
      taskService.getByUser(userId),
    ])
      .then(([projectsRes, tasksRes]) => {
        setProjects(projectsRes.data)
        setTasks(tasksRes.data)
      })
      .finally(() => setIsLoading(false))
  }, [userId])

  // Computed stats
  const myTasks = tasks.filter((t) => !t.parentTaskId)
  const overdueTasks = myTasks.filter((t) => t.isOverdue && t.status !== TaskStatus.Done)
  const doneThisWeek = myTasks.filter((t) => t.status === TaskStatus.Done && isThisWeek(t.updatedAt))
  const pendingTasks = myTasks
    .filter((t) => t.status !== TaskStatus.Done)
    .sort((a, b) => {
      if (a.isOverdue && !b.isOverdue) return -1
      if (!a.isOverdue && b.isOverdue) return 1
      if (a.dueDate && b.dueDate) return new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime()
      if (a.dueDate) return -1
      if (b.dueDate) return 1
      return 0
    })
    .slice(0, 6)

  const statusCounts = {
    [TaskStatus.ToDo]: myTasks.filter((t) => t.status === TaskStatus.ToDo).length,
    [TaskStatus.InProgress]: myTasks.filter((t) => t.status === TaskStatus.InProgress).length,
    [TaskStatus.Done]: myTasks.filter((t) => t.status === TaskStatus.Done).length,
    [TaskStatus.Blocked]: myTasks.filter((t) => t.status === TaskStatus.Blocked).length,
  }

  const recentProjects = [...projects]
    .sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime())
    .slice(0, 4)

  const recentActivity = [...tasks]
    .sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime())
    .slice(0, 5)

  const statCards = [
    { label: 'My Projects', value: projects.length, color: 'text-accent' },
    { label: 'My Tasks', value: myTasks.length, color: 'text-ink' },
    { label: 'Overdue', value: overdueTasks.length, color: overdueTasks.length > 0 ? 'text-red-600' : 'text-ink' },
    { label: 'Done this week', value: doneThisWeek.length, color: 'text-green-600' },
  ]

  return (
    <div className="flex flex-col gap-6">
      {/* Greeting */}
      <div>
        <p className="text-xs font-semibold uppercase tracking-widest text-accent">Dashboard</p>
        <h1 className="mt-1 text-2xl font-semibold text-ink">
          {getGreeting()}, {displayName}
        </h1>
        <p className="text-sm text-muted">
          {new Date().toLocaleDateString('en-US', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
        </p>
      </div>

      {isLoading ? (
        <div className="py-16 text-center text-sm text-muted">Loading dashboard…</div>
      ) : (
        <>
          {/* Stat cards */}
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
            {statCards.map((card) => (
              <div key={card.label} className="rounded-xl border border-black/8 bg-white p-4 dark:border-white/8 dark:bg-surface">
                <p className="text-xs text-muted">{card.label}</p>
                <p className={`mt-1 text-3xl font-semibold ${card.color}`}>{card.value}</p>
              </div>
            ))}
          </div>

          {/* Status breakdown */}
          <div className="rounded-xl border border-black/8 bg-white p-4 dark:border-white/8 dark:bg-surface">
            <p className="mb-3 text-xs font-medium uppercase tracking-wide text-muted">Tasks by status</p>
            <div className="grid grid-cols-4 gap-3 text-center">
              {[
                { label: 'To Do', count: statusCounts[TaskStatus.ToDo], color: 'bg-gray-100 text-gray-600 dark:bg-gray-700/40 dark:text-gray-300' },
                { label: 'In Progress', count: statusCounts[TaskStatus.InProgress], color: 'bg-blue-50 text-blue-600 dark:bg-blue-900/30 dark:text-blue-300' },
                { label: 'Done', count: statusCounts[TaskStatus.Done], color: 'bg-green-50 text-green-600 dark:bg-green-900/30 dark:text-green-300' },
                { label: 'Blocked', count: statusCounts[TaskStatus.Blocked], color: 'bg-red-50 text-red-600 dark:bg-red-900/30 dark:text-red-300' },
              ].map((s) => (
                <div key={s.label} className={`rounded-lg px-2 py-3 ${s.color}`}>
                  <p className="text-2xl font-semibold">{s.count}</p>
                  <p className="mt-0.5 text-xs">{s.label}</p>
                </div>
              ))}
            </div>
          </div>

          {/* Two columns: projects + pending tasks */}
          <div className="grid gap-4 lg:grid-cols-2">
            {/* Recent projects */}
            <div className="rounded-xl border border-black/8 bg-white p-4 dark:border-white/8 dark:bg-surface">
              <div className="mb-3 flex items-center justify-between">
                <p className="text-xs font-medium uppercase tracking-wide text-muted">Recent projects</p>
                <Link to={ROUTES.projects} className="text-xs text-accent hover:underline">
                  View all →
                </Link>
              </div>
              {recentProjects.length === 0 ? (
                <p className="py-4 text-center text-sm text-muted">No projects yet.</p>
              ) : (
                <ul className="flex flex-col gap-2">
                  {recentProjects.map((p) => (
                    <li key={p.id}>
                      <Link
                        to={`${ROUTES.projects}/${p.id}`}
                        className="flex items-center justify-between rounded-lg px-3 py-2.5 transition-colors hover:bg-black/4 dark:hover:bg-white/5"
                      >
                        <div>
                          <p className="text-sm font-medium text-ink">{p.name}</p>
                          <p className="text-xs text-muted">{p.taskCount} task{p.taskCount !== 1 ? 's' : ''} · {p.memberCount} member{p.memberCount !== 1 ? 's' : ''}</p>
                        </div>
                        <span className="text-xs text-muted">→</span>
                      </Link>
                    </li>
                  ))}
                </ul>
              )}
            </div>

            {/* Pending tasks */}
            <div className="rounded-xl border border-black/8 bg-white p-4 dark:border-white/8 dark:bg-surface">
              <div className="mb-3 flex items-center justify-between">
                <p className="text-xs font-medium uppercase tracking-wide text-muted">Pending tasks</p>
                <span className="text-xs text-muted">{pendingTasks.length > 0 ? `${pendingTasks.length} shown` : ''}</span>
              </div>
              {pendingTasks.length === 0 ? (
                <p className="py-4 text-center text-sm text-muted">No pending tasks. 🎉</p>
              ) : (
                <ul className="flex flex-col gap-2">
                  {pendingTasks.map((t) => (
                    <li key={t.id} className="flex items-start gap-2 rounded-lg px-3 py-2.5">
                      <div className="flex-1 min-w-0">
                        <p className={`truncate text-sm font-medium ${t.isOverdue ? 'text-red-600' : 'text-ink'}`}>
                          {t.isOverdue && <span className="mr-1 text-xs">⚠</span>}{t.title}
                        </p>
                        <p className="truncate text-xs text-muted">
                          {t.projectName}
                          {t.dueDate && ` · due ${new Date(t.dueDate).toLocaleDateString()}`}
                        </p>
                      </div>
                      <div className="flex-shrink-0">
                        <StatusBadge status={t.status} />
                      </div>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          </div>

          {/* Recent activity */}
          <div className="rounded-xl border border-black/8 bg-white p-4 dark:border-white/8 dark:bg-surface">
            <p className="mb-3 text-xs font-medium uppercase tracking-wide text-muted">Recent activity</p>
            {recentActivity.length === 0 ? (
              <p className="py-4 text-center text-sm text-muted">No recent activity.</p>
            ) : (
              <ul className="flex flex-col divide-y divide-black/5 dark:divide-white/5">
                {recentActivity.map((t) => (
                  <li key={t.id} className="flex items-center gap-3 py-2.5 divide-white/5">
                    <div className="flex-1 min-w-0">
                      <p className="truncate text-sm text-ink">{t.title}</p>
                      <p className="truncate text-xs text-muted">{t.projectName}</p>
                    </div>
                    <div className="flex flex-shrink-0 items-center gap-2">
                      <StatusBadge status={t.status} />
                      <PriorityBadge priority={t.priority} />
                    </div>
                    <span className="flex-shrink-0 text-xs text-muted">
                      {new Date(t.updatedAt).toLocaleDateString()}
                    </span>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </>
      )}
    </div>
  )
}
