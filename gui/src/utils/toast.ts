import toastr from 'toastr'

export function success(message: string): void {
  toastr.success(message)
}

export function warning(message: string): void {
  toastr.warning(message)
}

export function error(message: string): void {
  toastr.error(message)
}
