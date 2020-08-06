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
  onlyUploadYmlFiles: "Make sure to only upload files with the 'yml' or 'yaml' extension.",
  areYouSure: "Are you sure?",
  defaultStub: `- id: unique-stub-id
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

const tooltipResources = {
  id: "The ID of the stub. If you don't fill in an ID, an ID will be calculated when the stub is added. If you provide an ID of a stub that already exists, that stub will be overwritten with this one.",
  tenant: "The stub also has a 'tenant' field defined. This is a free text field which is optional. This field makes it possible to do operations of multiple stubs at once (e.g. delete all stubs with a specific tenant, get all stubs of a specific tenant or update all stubs of a specific tenant).",
  description: "A free text field where you can specify where the stub is for. It is optional.",
  priority: "There are cases when a request matches multiple stub. If this is the case, you can use the 'priority' element. With the priority element, you can specify which stub should be used if multiple stubs are found. The stub with the highest priority will be used. If you don't set the priority on the stub, it will be 0 by default.",
  httpMethod: "This condition checker can check the HTTP method (e.g. GET, POST, PUT, DELETE etc.).",
  urlPath: "The path condition is used to check a part of the URL path (so the part after http://... and before the query string). The condition can both check on substring and regular expressions."
};

const conditionValidationType = {
  NotSet: "NotSet",
  Valid: "Valid",
  Invalid: "Invalid",
  NotExecuted: "NotExecuted"
};

const httpMethods = [
  "GET",
  "POST",
  "PUT",
  "DELETE",
  "PATCH",
  "OPTIONS"
]

export { resources, conditionValidationType, httpMethods, tooltipResources };
