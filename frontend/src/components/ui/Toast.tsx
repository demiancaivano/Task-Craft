import { useEffect } from 'react'

type ToastProps = {
  message: string
  onClose: () => void
  durationMs?: number
}

export function Toast({ message, onClose, durationMs = 3200 }: ToastProps) {
  useEffect(() => {
    const timer = window.setTimeout(onClose, durationMs)
    return () => window.clearTimeout(timer)
  }, [durationMs, onClose])

  return (
    <div className="fixed right-4 top-4 z-50 rounded-lg border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-800 shadow-lg dark:border-emerald-800/40 dark:bg-emerald-900/30 dark:text-emerald-200">
      <div className="flex items-start gap-3">
        <p>{message}</p>
        <button
          type="button"
          onClick={onClose}
          className="font-medium text-emerald-700 transition-colors hover:text-emerald-900 dark:text-emerald-300 dark:hover:text-emerald-100"
          aria-label="Close notification"
        >
          Close
        </button>
      </div>
    </div>
  )
}
