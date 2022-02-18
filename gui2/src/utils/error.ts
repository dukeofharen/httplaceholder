import { error as errorToast } from "@/utils/toast";
import { resources } from "@/constants/resources";
import type { HttpError } from "@/utils/api";

export function handleHttpError(error: any) {
  let result;
  if (!error.status) {
    // Not an HTTP error.
    return;
  }

  const httpError = <HttpError>error;
  if (httpError.status === 401) {
    // Let the application handle the 401s itself.
    return;
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
    errorToast(resources.somethingWentWrongServer);
  }
}
