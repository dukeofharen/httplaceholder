import { toastError } from "@/utils/toastUtil";
import router from "@/router";
import { resources } from "@/shared/resources";
import store from "@/store";
import { mutationNames } from "@/store/storeConstants";
import { routeNames } from "@/router/routerConstants";

export default function handleError(error) {
  if (error && error.response) {
    const status = error.response.status;
    if (status !== 401) {
      toastError(resources.somethingWentWrongServer);
    } else {
      store.commit(mutationNames.userTokenMutation, null);
      router.push({ name: routeNames.login });
    }

    return Promise.reject(error);
  }
}
