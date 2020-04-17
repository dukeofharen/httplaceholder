import {toastError} from "@/utils/toastUtil";
import router from "@/router";

export default function handleError(error) {
    if (error && error.response) {
        const status = error.response.status;
        if (status !== 401) {
            toastError(resources.somethingWentWrongServer);
        } else {
            router.push({name: "login"});
        }

        return Promise.reject(error);
    }
}