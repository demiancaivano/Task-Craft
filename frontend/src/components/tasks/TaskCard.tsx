import type { TaskDto } from '../../types/task'
import { PriorityBadge, StatusBadge } from '../ui/Badge'

type TaskCardProps = {
  task: TaskDto
  onClick: (task: TaskDto) => void
  isSubtask?: boolean
}

function AssigneeAvatars({ assignees }: { assignees: { userId: string; username: string }[] }) {
  if (assignees.length === 0) return null
  const visible = assignees.slice(0, 3)
  const extra = assignees.length - visible.length
  return (
    <div className="flex items-center">
      {visible.map((a, i) => (
        <span
          key={a.userId}
          title={a.username}
          style={{ zIndex: visible.length - i }}
          className="-ml-1.5 first:ml-0 flex h-5 w-5 shrink-0 items-center justify-center rounded-full border border-white bg-accent/20 text-[9px] font-bold text-accent dark:border-surface"
        >
          {a.username.charAt(0).toUpperCase()}
        </span>
      ))}
      {extra > 0 && (
        <span className="-ml-1.5 flex h-5 w-5 shrink-0 items-center justify-center rounded-full border border-white bg-black/10 text-[9px] font-medium text-muted">
          +{extra}
        </span>
      )}
    </div>
  )
}

export function TaskCard({ task, onClick, isSubtask = false }: TaskCardProps) {
  const isOverdue = task.isOverdue && task.dueDate

  if (isSubtask) {
    return (
      <button
        type="button"
        onClick={() => onClick(task)}
        className="mt-1.5 flex w-full items-center gap-2 rounded-md border border-black/6 bg-black/2 px-2.5 py-1.5 text-left text-xs transition-all hover:border-black/15 hover:bg-black/5 focus:outline-none focus:ring-1 focus:ring-accent dark:border-white/6 dark:bg-white/2 dark:hover:border-white/15 dark:hover:bg-white/5"
      >
        <StatusBadge status={task.status} />
        <span className="flex-1 truncate font-medium text-ink/80">{task.title}</span>
        <AssigneeAvatars assignees={task.assignees ?? []} />
        {isOverdue && <span className="shrink-0 text-red-500">⚠</span>}
        <PriorityBadge priority={task.priority} />
      </button>
    )
  }

  return (
    <button
      type="button"
      onClick={() => onClick(task)}
      className="group w-full rounded-lg border border-black/8 bg-white p-3 text-left shadow-sm transition-all hover:border-black/20 hover:shadow-md focus:outline-none focus:ring-2 focus:ring-accent dark:border-white/8 dark:bg-surface dark:hover:border-white/20 dark:hover:shadow-black/30"
    >
      <div className="flex items-start justify-between gap-2">
        <p className="text-sm font-medium text-ink line-clamp-2 leading-snug group-hover:text-accent">
          {task.title}
        </p>
        <PriorityBadge priority={task.priority} />
      </div>

      <div className="mt-2.5 flex flex-wrap items-center gap-2 text-xs text-muted">
        {task.dueDate && (
          <span className={isOverdue ? 'font-medium text-red-600' : ''}>
            Due {new Date(task.dueDate).toLocaleDateString()}
            {isOverdue && ' ⚠'}
          </span>
        )}
        {task.hasSubTasks && (
          <span>{task.subTaskCount} subtask{task.subTaskCount !== 1 ? 's' : ''}</span>
        )}
        <AssigneeAvatars assignees={task.assignees ?? []} />
      </div>
    </button>
  )
}
