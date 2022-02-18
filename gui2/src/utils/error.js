import { error as errorToast } from "@/utils/toast";
import { resources } from "@/constants/resources";

export function handleHttpError(error) {
  let result;
  if (error.status === 401) {
    // Let the application handle the 401s itself.
    return;
  }

  if (error.status === 400) {
    if (Array.isArray(error.body)) {
      result = "<ul>";
      for (const item of error.body) {
        result += `<li>${item}</li>`;
      }

      result += "</ul>";
    } else if (typeof error.body === "string") {
      result = error.body;
    }
  }

  if (result) {
    errorToast(result);
  } else {
    console.log(error);
    errorToast(resources.somethingWentWrongServer);
  }
}
