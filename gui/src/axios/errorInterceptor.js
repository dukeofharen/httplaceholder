import {toastError} from "@/utils/toastUtil";
import router from "@/router";
import {resources} from "@/shared/resources";
import store from "@/store";
import {routeNames} from "@/router/routerConstants";

export default function handleError(error) {
  if (error) {
    if (error.response) {
      const status = error.response.status;
      if (status === 401) {
        store.commit("users/storeUserToken", null);
        router.push({name: routeNames.login});
      } else if (status === 400 && error.response.data instanceof Array) {
        const validationErrors = error.response.data.map(e => `- ${e}`);
        const msg = validationErrors.join("<br />");
        toastError(msg);
      } else if (status >= 500) {
        toastError(resources.somethingWentWrongServer);
      }
    } else {
      toastError(resources.somethingWentWrongServer);
    }

    return Promise.reject(error);
  }
}
