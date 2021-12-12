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
  exampleCurlInput: `curl 'https://api.site.com/api/v1/users' \\
  -X 'PUT' \\
  -H 'authority: api.site.com' \\
  -H 'sec-ch-ua: " Not A;Brand";v="99", "Chromium";v="96", "Google Chrome";v="96"' \\
  -H 'accept: application/json, text/plain, */*' \\
  -H 'content-type: application/json;charset=UTF-8' \\
  -H 'authorization: Bearer VERYLONGSTRING \\
  -H 'sec-ch-ua-mobile: ?0' \\
  -H 'user-agent: Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36' \\
  -H 'sec-ch-ua-platform: "Linux"' \\
  -H 'origin: https://site.com' \\
  -H 'sec-fetch-site: same-site' \\
  -H 'sec-fetch-mode: cors' \\
  -H 'sec-fetch-dest: empty' \\
  -H 'accept-language: en-US,en;q=0.9,nl;q=0.8' \\
  --data-raw $'{"id":1,"created":"2015-10-21T14:39:55","updated":"2021-11-26T22:10:52","userName":"d","firstName":"d\\'","lastName":"h","street":"Road","number":"6","postalCode":"1234AB","city":"Amsterdam","phone":"0612345678","email":"info@example.com","placeId":1,"newsletter":false,"driversLicenseNumber":"112233","emailRepeat":"info@example.com"}' \\
  --compressed`,
};
