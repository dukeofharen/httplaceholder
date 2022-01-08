import { curl } from "./examples/curl";
import { har } from "./examples/har";
import { openapi } from "./examples/openapi";

export const resources = {
  somethingWentWrongServer: "Something went wrong while contacting the server.",
  requestsDeletedSuccessfully: "The requests were deleted successfully.",
  requestDeletedSuccessfully: "The request was deleted successfully.",
  stubDeletedSuccessfully: "Stub was deleted successfully.",
  stubsDeletedSuccessfully: "All stubs deleted successfully.",
  stubsAddedSuccessfully: "Stubs were added successfully.",
  stubUpdatedSuccessfully: "Stub was updated successfully.",
  stubsDisabledSuccessfully: "Stubs were disabled successfully.",
  stubsEnabledSuccessfully: "Stubs were enabled successfully.",
  filteredStubsDeletedSuccessfully: "Stubs were deleted successfully.",
  stubNotFound: "Stub with ID {0} was not found.",
  stubsInFileAddedSuccessfully: "Stubs in file '{0}' were added successfully.",
  scenarioSetSuccessfully: "The scenario values were set successfully.",
  scenariosDeletedSuccessfully: "The scenarios were deleted successfully.",
  scenarioDeletedSuccessfully: "The scenario was deleted successfully.",
  noCurlStubsFound:
    "No stubs could be determined from the cURL command(s). This might mean that you did not provide valid input.",
  errorDuringParsingOfYaml: "Something went wrong while parsing the YAML: {0}",
  requestBodyCopiedToClipboard:
    "Request body successfully copied to clipboard.",
  uploadInvalidFiles:
    "These files you are trying to upload have an incorrect extension: {0}.",
  onlyUploadYmlFiles:
    "Make sure to only upload files with the 'yml' or 'yaml' extension.",
  credentialsIncorrect: "The credentials are incorrect.",
  defaultStub: `id: unique-stub-id
description: A description for the stub.
conditions:
  method: GET
response:
  text: OK!`,
  downloadStubsHeader:
    "# This .yml file was created with HttPlaceholder. For more information, go to http://httplaceholder.com.",
  exampleCurlInput: curl,
  exampleHarInput: har,
  exampleOpenApiInput: openapi,
};
