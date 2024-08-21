import { curl } from "./examples/curl";
import { har } from "./examples/har";
import { openapi } from "./examples/openapi";
import { getUiRootUrl } from "@/utils/config";

export const resources = {
  somethingWentWrongServer: "Something went wrong while contacting the server.",
  requestsDeletedSuccessfully: "The requests were deleted successfully.",
  requestDeletedSuccessfully: "The request was deleted successfully.",
  stubDeletedSuccessfully: "Stub was deleted successfully.",
  stubUpdatedSuccessfully: "Stub was updated successfully.",
  stubsInFileAddedSuccessfully: "Stubs in file '%s' were added successfully.",
  scenarioSetSuccessfully: "The scenario values were set successfully.",
  errorDuringParsingOfYaml: "Something went wrong while parsing the YAML: %s",
  requestBodyCopiedToClipboard:
    "Request body successfully copied to clipboard.",
  uploadInvalidFiles:
    "These files you are trying to upload have an incorrect extension: %s. The following extensions are allowed: %s",
  credentialsIncorrect: "The credentials are incorrect.",
  defaultStub: `id: unique-stub-id
description: A description for the stub.
conditions:
  method: GET
response:
  text: OK!`,
  exampleCurlInput: curl,
  exampleHarInput: har,
  exampleOpenApiInput: openapi,
};

export function renderDocLink(hashTag?: string) {
  let docsUrl = `${getUiRootUrl()}/docs/index.html`;
  if (hashTag) {
    docsUrl += `#${hashTag}`;
  }

  return docsUrl;
}
