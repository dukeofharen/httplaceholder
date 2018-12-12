const resources = {
    somethingWentWrongServer: "Something went wrong while contacting the server.",
    credentialsIncorrect: "The credentials are incorrect.",
    requestsDeletedSuccessfully: "The requests were deleted successfully.",
    stubAddedSuccessfully: "Stub with ID '{0}' was added successfully.",
    stubUpdatedSuccessfully: "Stub with ID '{0}' was updated successfully.",
    stubDeletedSuccessfully: "Stub with ID '{0}' was deleted successfully.",
    stubNotAdded: "Stub with ID '{0}' could not be added.",
    stubAlreadyAdded: "Stub with ID '{0}' is already added.",
    areYouSure: "Are you sure?",
    defaultStub:
`- id: unique-stub-id
  conditions:
    method: GET
    url:
      path: /stub
  response:
    text: OK!`
};

export default resources