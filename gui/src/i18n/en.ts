export const translations = {
  general: {
    requests: "Requests",
    refresh: "Refresh",
    reset: "Reset",
    tenant: "Tenant",
    stubId: "Stub ID",
    selectStubTenantCategory: "Select stub tenant / category name...",
    required: "required",
    optional: "optional",
    save: "Save",
    update: "Update",
    delete: "Delete",
  },
  requests: {
    reloadAllRequests: "Load all requests",
    deleteAllRequests: "Delete all requests",
    deleteAllRequestsQuestion: "Delete all requests?",
    requestsCantBeRecovered: "The requests can't be recovered.",
    filterPlaceholder: "Filter on stub ID, request ID or URL...",
    filterLabel: "Stub ID / req.ID / URL",
    loadMoreRequests: "Load more requests",
    noRequestsYet:
      "No requests have been made to HttPlaceholder yet. Perform HTTP requests and you will see the requests appearing on this page.",
  },
  importStubs: {
    importStubs: "Import stubs",
    uploadStubs: "Upload stubs",
    importCurlCommands: "Import cURL command(s)",
    importHar: "Import HTTP archive (HAR)",
    importOpenApi: "Import OpenAPI definition",
  },
  logIn: {
    logIn: "Log in",
    username: "Username",
    password: "Password",
  },
  scenarioForm: {
    scenarioName: "Scenario name",
    scenarioState: "Scenario state",
    scenarioHitCount: "Scenario hit count",
    addScenario: "Add scenario",
    updateScenario: "Update scenario",
  },
  scenarios: {
    scenarios: "Scenarios",
    description:
      'Scenarios can be used to make stubs stateful. On this page, you can manage the scenarios in HttPlaceholder. To read more about scenarios, go to <a href="%s" target="_blank">the documentation</a>.',
    addScenario: "Add scenario",
    clearAllScenarios: "Clear all scenarios",
    clearAllScenariosQuestion: "Clear all scenarios?",
    scenariosCantBeRecovered: "The scenarios can't be recovered.",
    state: "State",
    hitCount: "Hit count",
    scenariosDeletedSuccessfully: "The scenarios were deleted successfully.",
    scenarioDeletedSuccessfully: "The scenario was deleted successfully.",
  },
  settings: {
    settings: "Settings",
    features: "Features",
    darkTheme: "Dark theme",
    persistSearchFilters: "Persist search filters on stubs and request screens",
    storeResponseForRequest: "Store response for request",
    storeResponseForRequestDescription:
      '<strong>Note</strong>: this setting will be reset to its original value after restarting HttPlaceholder. To persist the setting, take a look at <a href="%s" target="_blank">the documentation</a>.',
    defaultNumberOfRequests:
      "Default number of requests on the request page (set to 0 to disable request paging)",
    httplaceholderConfiguration: "HttPlaceholder configuration",
    httplaceholderConfigurationDescription:
      "HttPlaceholder was started with the following settings. The settings are read-only and can only be set when starting the application.",
    metadata: "Metadata",
    version: "Version",
    runtime: "Runtime",
  },
  stubForm: {
    description:
      'Fill in the stub below in YAML format and click on "Save". For examples, <a href="%s" target="_blank" class="break-word">read the docs</a>.',
    advancedEditor: "Advanced editor",
    advancedEditorDescription:
      "Use advanced editor for editing the stub. The editor has code highlighting but is not suited for updating large stubs.",
    simpleEditor: "Simple editor",
    simpleEditorDescription:
      "Use simple editor for editing the stub. The editor has no code highlighting but is suited for updating large stubs.",
    stubNotFound: "Stub with ID %s was not found.",
    addStub: "Add stub",
    updateStub: "Update stub",
  },
  stubs: {
    stubs: "Stubs",
    addStubs: "Add stubs",
    downloadStubs: "Download stubs as YAML",
    downloadStubsDescription: "Download the (filtered) stubs as YAML file.",
    importStubs: "Import stubs",
    deleteAllStubs: "Delete all stubs",
    deleteAllStubsQuestion: "Delete all stubs?",
    stubsCantBeRecovered: "The stubs can't be recovered.",
    disableStubs: "Disable stubs",
    disableStubsDescription: "Disable the current selection of stubs",
    disableStubsQuestion: "Disable the current filtered stubs?",
    disableStubsModalBody:
      "Only the stubs currently visible in the list will be disabled.",
    enableStubs: "Enable stubs",
    enableStubsQuestion: "Enable the current filtered stubs?",
    enableStubsModalBody:
      "Only the stubs currently visible in the list will be enabled.",
    deleteSelectedStubs: "Delete stubs",
    deleteSelectedStubsQuestion: "Delete the current filtered stubs?",
    deleteSelectedStubsModalBody:
      "The stubs can't be recovered. Only the stubs currently visible in the list will be deleted.",
    filterPlaceholder: "Filter on stub ID...",
    stubsDeletedSuccessfully: "All stubs deleted successfully.",
    stubEnabledSuccessfully: "Stub with ID '%s' was enabled successfully.",
    stubDisabledSuccessfully: "Stub with ID '%s' was disabled successfully.",
    filteredStubsDeletedSuccessfully: "Stubs were deleted successfully.",
    downloadStubsHeader:
      "# This .yml file was created with HttPlaceholder. For more information, go to http://httplaceholder.com.",
  },
};
