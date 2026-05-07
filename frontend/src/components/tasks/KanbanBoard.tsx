import { useState } from 'react'
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

const COLUMNS: { status: TaskStatus; title: string; colorClass: string; dotClass: string }[] = [
  { status: TaskStatus.ToDo,       title: 'To Do',       colorClass: 'bg-gray-400',  dotClass: 'bg-gray-400' },
  { status: TaskStatus.InProgress, title: 'In Progress', colorClass: 'bg-blue-500',  dotClass: 'bg-blue-500' },
  { status: TaskStatus.Done,       title: 'Done',        colorClass: 'bg-green-500', dotClass: 'bg-green-500' },
  { status: TaskStatus.Blocked,    title: 'Blocked',     colorClass: 'bg-red-500',   dotClass: 'bg-red-500' },
]

export function KanbanBoard({ tasks, onTaskClick, onAddTask, userRole }: KanbanBoardProps) {
  const [activeTab, setActiveTab] = useState<TaskStatus>(TaskStatus.ToDo)

  const rootTasksByStatus = (status: TaskStatus) =>
    tasks.filter((t) => t.status === status && !t.parentTaskId)

  const activeColumn = COLUMNS.find((c) => c.status === activeTab)!

  return (
    <>
      {/* Mobile: tab bar + single column */}
      <div className="md:hidden">
        <div className="flex gap-1 rounded-xl border border-black/8 bg-black/3 p-1 dark:border-white/8 dark:bg-white/5">
          {COLUMNS.map(({ status, title, dotClass }) => {
            const count = rootTasksByStatus(status).length
            const isActive = activeTab === status
            return (
              <button
                key={status}
                type="button"
                onClick={() => setActiveTab(status)}
                className={`flex flex-1 flex-col items-center gap-0.5 rounded-lg px-2 py-2 text-xs font-medium transition-colors ${
                  isActive
                    ? 'bg-white shadow-sm text-ink dark:bg-surface dark:text-ink'
                    : 'text-muted hover:text-ink'
                }`}
              >
                <span className={`h-1.5 w-1.5 rounded-full ${dotClass}`} aria-hidden="true" />
                <span className="hidden xs:inline">{title}</span>
                <span className="xs:hidden">{title.split(' ')[0]}</span>
                <span className={`rounded-full px-1.5 py-px text-[10px] font-semibold ${
                  isActive ? 'bg-accent/10 text-accent' : 'bg-black/8 text-muted dark:bg-white/10'
                }`}>
                  {count}
                </span>
              </button>
            )
          })}
        </div>

        <div className="mt-3">
          <KanbanColumn
            title={activeColumn.title}
            tasks={rootTasksByStatus(activeTab)}
            allTasks={tasks}
            colorClass={activeColumn.colorClass}
            onTaskClick={onTaskClick}
            onAddTask={() => onAddTask(activeTab)}
            userRole={userRole}
            hideTitleBar
          />
        </div>
      </div>

      {/* Desktop: horizontal kanban */}
      <div className="hidden md:block">
        <div className="-mx-6 overflow-x-auto px-6 pb-4">
          <div className="flex gap-5" style={{ minWidth: 'max-content' }}>
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
        </div>
      </div>
    </>
  )
}
