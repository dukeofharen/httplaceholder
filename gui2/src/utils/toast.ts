import type { ToastModel } from '@/domain/toast-model.ts'

function showToast(toast: ToastModel) {
  document.dispatchEvent(new CustomEvent('toast', { detail: toast }))
}

export function success(message: string): void {
  showToast({
    type: 'success',
    title: message,
  })
}

export function warning(message: string): void {
  showToast({
    type: 'warning',
    title: message,
  })
}

export function error(message: string): void {
  showToast({
    type: 'error',
    title: message,
  })
}
