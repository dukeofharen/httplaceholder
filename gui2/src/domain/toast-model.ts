export interface ToastModel {
  id?: number
  title: string
  message?: string
  duration?: number
  type: 'success' | 'warning' | 'error'
}
