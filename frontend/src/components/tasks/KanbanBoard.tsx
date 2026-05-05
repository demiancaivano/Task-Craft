import type { TaskDto } from '../../types/task'
import { TaskStatus } from '../../types/task'
import { KanbanColumn } from './KanbanColumn'
import type { ProjectRole } from '../../types/project'

type KanbanBoardProps = {
  tasks: TaskDto[]
  onTaskClick: (task: TaskDto) => void
  onAddTask: (defaultStatus?: TaskStatus) => void
  userRole: ProjectRole | null
}

const COLUMNS: { status: TaskStatus; title: string; colorClass: string }[] = [
  { status: TaskStatus.ToDo, title: 'To Do', colorClass: 'bg-gray-400' },
  { status: TaskStatus.InProgress, title: 'In Progress', colorClass: 'bg-blue-500' },
  { status: TaskStatus.Done, title: 'Done', colorClass: 'bg-green-500' },
  { status: TaskStatus.Blocked, title: 'Blocked', colorClass: 'bg-red-500' },
]

export function KanbanBoard({ tasks, onTaskClick, onAddTask, userRole }: KanbanBoardProps) {
  const rootTasksByStatus = (status: TaskStatus) =>
    tasks.filter((t) => t.status === status && !t.parentTaskId)

  return (
    <div className="flex gap-5 overflow-x-auto pb-4">
      {COLUMNS.map(({ status, title, colorClass }) => (
        <KanbanColumn
          key={status}
          title={title}
          tasks={rootTasksByStatus(status)}
          allTasks={tasks}
          colorClass={colorClass}
          onTaskClick={onTaskClick}
          onAddTask={() => onAddTask(status)}
          userRole={userRole}
        />
      ))}
    </div>
  )
}
