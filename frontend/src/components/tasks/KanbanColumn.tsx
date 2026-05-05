import type { TaskDto } from '../../types/task'
import { TaskCard } from './TaskCard'
import { ProjectRole } from '../../types/project'
import type { ProjectRole as ProjectRoleType } from '../../types/project'

type KanbanColumnProps = {
  title: string
  tasks: TaskDto[]
  allTasks: TaskDto[]
  colorClass: string
  onTaskClick: (task: TaskDto) => void
  onAddTask?: () => void
  userRole: ProjectRoleType | null
}

export function KanbanColumn({ title, tasks, allTasks, colorClass, onTaskClick, onAddTask, userRole }: KanbanColumnProps) {
  const canAddTask = userRole !== null && userRole <= ProjectRole.Developer

  const subtasksOf = (parentId: string) =>
    allTasks.filter((t) => t.parentTaskId === parentId)

  return (
    <div className="flex w-72 shrink-0 flex-col gap-3">
      {/* Column header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <span className={`h-2.5 w-2.5 rounded-full ${colorClass}`} aria-hidden="true" />
          <span className="text-sm font-semibold text-ink">{title}</span>
          <span className="rounded-full bg-black/8 px-1.5 py-0.5 text-xs font-medium text-muted dark:bg-white/10">
            {tasks.length}
          </span>
        </div>
        {onAddTask && canAddTask && (
          <button
            type="button"
            onClick={onAddTask}
            className="rounded p-1 text-muted transition-colors hover:bg-black/5 hover:text-ink dark:hover:bg-white/8"
            aria-label={`Add task to ${title}`}
          >
            +
          </button>
        )}
      </div>

      {/* Task list */}
      <div className="flex flex-col gap-2">
        {tasks.map((task) => (
          <div key={task.id}>
            <TaskCard task={task} onClick={onTaskClick} />
            {subtasksOf(task.id).map((sub) => (
              <TaskCard key={sub.id} task={sub} onClick={onTaskClick} isSubtask />
            ))}
          </div>
        ))}
        {tasks.length === 0 && (
          <div className="rounded-lg border border-dashed border-black/12 py-6 text-center text-xs text-muted dark:border-white/12">
            No tasks
          </div>
        )}
      </div>
    </div>
  )
}
