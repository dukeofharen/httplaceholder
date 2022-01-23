import toastr from "toastr";

export function success(message) {
  toastr.success(message);
}

export function warning(message) {
  toastr.warning(message);
}

export function error(message) {
  toastr.error(message);
}
