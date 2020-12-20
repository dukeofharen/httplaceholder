const resources = {
  somethingWentWrongServer: "Something went wrong while contacting the server.",
  credentialsIncorrect: "The credentials are incorrect.",
  requestsDeletedSuccessfully: "The requests were deleted successfully.",
  stubAddedSuccessfully: "Stub with ID '{0}' was added successfully.",
  stubUpdatedSuccessfully: "Stub with ID '{0}' was updated successfully.",
  stubDeletedSuccessfully: "Stub with ID '{0}' was deleted successfully.",
  stubsDeletedSuccessfully: "All stubs deleted successfully.",
  stubNotAdded: "Stub with ID '{0}' could not be added.",
  stubNotAddedGeneric: "Stub could not be added.",
  stubAlreadyAdded: "Stub with ID '{0}' is already added.",
  onlyOneStubAtATime: "You can only update one stub at a time.",
  onlyUploadYmlFiles:
    "Make sure to only upload files with the 'yml' or 'yaml' extension.",
  uploadInvalidFiles:
    "These files you are trying to upload have an incorrect extension: {0}.",
  requestNotFoundAnymore:
    "The request could not be found (anymore). Perhaps it's already cleaned up.",
  areYouSure: "Are you sure?",
  defaultStub: `id: unique-stub-id
description: A description for the stub.
conditions:
  method: GET
  url:
    path: /stub
response:
  text: OK!`,
  downloadStubsHeader:
    "# This .yml file was created with HttPlaceholder. For more information, go to http://httplaceholder.com."
};

const conditionValidationType = {
  NotSet: "NotSet",
  Valid: "Valid",
  Invalid: "Invalid",
  NotExecuted: "NotExecuted"
};

export { resources, conditionValidationType };
