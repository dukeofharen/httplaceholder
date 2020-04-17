import toastr from "toastr";

export function toastInfo(message) {
    toastr.info(message);
}

export function toastWarning(message) {
    toastr.warning(message);
}

export function toastError(message) {
    toastr.error(message);
}

export function toastSuccess(message) {
    toastr.success(message);
}