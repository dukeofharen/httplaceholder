import { error as errorToast } from "@/utils/toast";
import type { HttpError } from "@/utils/api";
import { translate } from "@/utils/translate";

export function handleHttpError(error: any): boolean {
  let result;
  if (!error.status) {
    // Not an HTTP error.
    return false;
  }

  const httpError = <HttpError>error;
  if (httpError.status === 401) {
    // Let the application handle the 401s itself.
    return false;
  }

  if (httpError.status === 400) {
    if (Array.isArray(httpError.body)) {
      result = "<ul>";
      for (const item of httpError.body) {
        result += `<li>${item}</li>`;
      }

      result += "</ul>";
    } else if (typeof httpError.body === "string") {
      result = httpError.body;
    }
  }

  if (result) {
    errorToast(result);
  } else {
    console.log(error);
    errorToast(translate("errors.somethingWentWrongServer"));
  }

  return true;
}
