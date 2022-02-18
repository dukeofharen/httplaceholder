import toastr from "toastr";

export function success(message: string) {
  toastr.success(message);
}

export function warning(message: string) {
  toastr.warning(message);
}

export function error(message: string) {
  toastr.error(message);
}
