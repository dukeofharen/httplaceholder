import {formValidationMessages} from "@/shared/stubFormResources";

export function getStubFormValidation(state) {
  const validationMessages = [];
  if (state.stubForm.queryStrings && !state.stubForm.stub.conditions.url.query) {
    validationMessages.push(formValidationMessages.queryStringIncorrect);
  }

  if (state.stubForm.formBody && !state.stubForm.stub.conditions.form) {
    validationMessages.push(formValidationMessages.formBodyIncorrect);
  }

  if (state.stubForm.headers && !state.stubForm.stub.conditions.headers) {
    validationMessages.push(formValidationMessages.headersIncorrect);
  }

  if (isNaN(state.stubForm.stub.priority)) {
    validationMessages.push(formValidationMessages.priorityNotInteger);
  }

  if (state.stubForm.xpathNamespaces && !state.stubForm.stub.conditions.xpath) {
    validationMessages.push(formValidationMessages.xpathNotFilledIn);
  }

  if (
    !state.stubForm.stub.conditions.basicAuthentication.username && state.stubForm.stub.conditions.basicAuthentication.password ||
    state.stubForm.stub.conditions.basicAuthentication.username && !state.stubForm.stub.conditions.basicAuthentication.password) {
    validationMessages.push(formValidationMessages.basicAuthInvalid);
  }

  const statusCode = state.stubForm.stub.response.statusCode;
  const parsedStatusCode = parseInt(statusCode);
  if (statusCode !== null && (isNaN(parsedStatusCode) || parsedStatusCode < 100 || parsedStatusCode >= 600)) {
    validationMessages.push(formValidationMessages.fillInCorrectStatusCode);
  }

  if (state.stubForm.responseHeaders && !state.stubForm.stub.response.headers) {
    validationMessages.push(formValidationMessages.headersIncorrect);
  }

  const extraDuration = state.stubForm.stub.response.extraDuration;
  const parsedExtraDuration = parseInt(extraDuration);
  if (extraDuration !== null && extraDuration !== undefined && (isNaN(parsedExtraDuration) || parsedExtraDuration <= 0)) {
    validationMessages.push(formValidationMessages.extraDurationInvalid);
  }

  if (state.stubForm.stub.response.permanentRedirect && state.stubForm.stub.response.temporaryRedirect) {
    validationMessages.push(formValidationMessages.fillInOneTypeOfRedirect);
  }

  return validationMessages;
}

export function getStubForSaving(state) {
  return state.stubForm.stub;
}
