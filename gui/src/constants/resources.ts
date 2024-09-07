import { getUiRootUrl } from "@/utils/config";

export const resources = {
  somethingWentWrongServer: "Something went wrong while contacting the server.",
  requestsDeletedSuccessfully: "The requests were deleted successfully.",

  stubDeletedSuccessfully: "Stub was deleted successfully.",
  stubUpdatedSuccessfully: "Stub was updated successfully.",
  scenarioSetSuccessfully: "The scenario values were set successfully.",
  errorDuringParsingOfYaml: "Something went wrong while parsing the YAML: %s",
  requestBodyCopiedToClipboard:
    "Request body successfully copied to clipboard.",
  uploadInvalidFiles:
    "These files you are trying to upload have an incorrect extension: %s. The following extensions are allowed: %s",
  credentialsIncorrect: "The credentials are incorrect.",
};

export function renderDocLink(hashTag?: string) {
  let docsUrl = `${getUiRootUrl()}/docs/index.html`;
  if (hashTag) {
    docsUrl += `#${hashTag}`;
  }

  return docsUrl;
}
