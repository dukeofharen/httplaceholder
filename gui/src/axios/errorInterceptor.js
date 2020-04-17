import {toastError} from "@/utils/toastUtil";

export default function handleError(error) {
    const status = error.response.status;
    if (status !== 401) {
        toastError(resources.somethingWentWrongServer);
    }
}