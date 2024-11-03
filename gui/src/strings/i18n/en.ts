export const translations = {
  dayJsLocale: "en",
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
    yes: "Yes",
    no: "No",
    download: "Download",
    username: "Username",
    password: "Password",
    insert: "Insert",
    close: "Close",
    add: "Add",
    uploadInvalidFiles:
      "These files you are trying to upload have an incorrect extension: %s. The following extensions are allowed: %s",
    credentialsIncorrect: "The credentials are incorrect.",
  },
  sidebar: {
    requests: "Requests",
    stubs: "Stubs",
    addStubs: "Add stubs",
    importStubs: "Import stubs",
    scenarios: "Scenarios",
    docs: "Docs",
    apiDocs: "API Docs",
    settings: "Settings",
    logOut: "Log out",
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
    requestsDeletedSuccessfully: "The requests were deleted successfully.",
    requestBodyCopiedToClipboard:
      "Request body successfully copied to clipboard.",
  },
  request: {
    raw: "Raw",
    xml: "XML",
    json: "JSON",
    form: "Form",
    queryParameters: "Query parameters",
    executedStub: "Executed stub",
    createRequestStub: "Create stub",
    createRequestStubTitle:
      "Create a stub based on the request parameters of this request",
    exportRequest: "Export",
    exportRequestTitle: "Export the stub in a specific format",
    deleteRequest: "Delete",
    deleteRequestTitle: "Delete this request",
    requestDeletedSuccessfully: "The request was deleted successfully.",
    url: "URL",
    clientIp: "Client IP",
    correlationId: "Correlation ID",
    stubTenant: "Stub tenant (category)",
    requestTime: "Request time",
    itTookMs: "it took %s ms",
    requestBody: "Request body",
    selectExportFormat: "Select an export format...",
    curl: "cURL",
    har: "HTTP Archive (HAR)",
    copyToClipboard: "Copy command to clipboard",
    downloadExportedRequest: "Download the exported request",
    requestHeaders: "Request headers",
    responseHeaders: "Response headers",
    sentResponse: "Sent response",
    httpStatusCode: "HTTP status code",
    responseBody: "Response body",
    noResponseFound:
      'No response found for this request. Go to [Settings](%s) to enable "Store response for request".',
    responseWriterResults: "Response writer results",
    passed: "passed",
    notPassed: "not passed",
    goToStub: "Go to stub",
    noConditionCheckersFound: "No condition checkers executed for this stub.",
    stubExecutionResults: "Stub execution results",
  },
  importStubs: {
    importStubs: "Import stubs",
    uploadStubs: "Upload stubs",
    importCurlCommands: "Import cURL command(s)",
    importHar: "Import HTTP archive (HAR)",
    importOpenApi: "Import OpenAPI definition",
    howTo: "How to",
    insertExample: "Insert example",
    uploadFile: "Upload file",
    tenantPlaceholder:
      "Fill in a tenant to group the generated stubs... (if no tenant is provided, a tenant name will be generated)",
    stubIdPrefixPlaceholder:
      "Fill in a stub ID prefix here if desired... (every stub ID will be prefixed with this text)",
    stubsWillBeAdded: "The following stubs will be added.",
    saveStubs: "Save stubs",
    editStubsBeforeSaving: "Edit stubs before saving",
    stubsAddedSuccessfully: "Stubs were added successfully.",
  },
  importCurl: {
    intro:
      "Using this form, you can create stubs based on cURL commands. You can either use a cURL command you have lying around or you can copy/paste a cURL command from the developer console from your browser.",
    howToLine1:
      'You can copy/paste a cURL command from your browser. In most popular web browsers, you can do this by going to the developer tools, going to the "Network" tab and selecting the request where you would like to have the cURL request for.',
    howToLine2:
      'When copying cURL requests from a browser on Windows, make sure you select "Copy as cURL (bash)" or "Copy all as cURL (bash)" on Chrome or "Copy as cURL (POSIX)" in Firefox. The Windows formatting of cURL commands is currently not supported in HttPlaceholder.',
    howToImage1: "Example in Firefox",
    howToImage2:
      'Example in Chrome. In Chrome, you can either select "Copy as cURL" or "Copy all as cURL".',
    importCurlCommands: "Import cURL command(s)",
    noCurlStubsFound:
      "No stubs could be determined from the cURL command(s). This might mean that you did not provide valid input.",
  },
  importHar: {
    intro:
      "Using this form, you can create stubs based on an HTTP archive (or HAR). Most modern browsers allow you to download a HAR file with the request and response definitions of the recently made requests.",
    howToLine1:
      'To get the HTTP archive of the requests from your browser, you need to open the developer tools and open the "Network" tab.',
    howToLine2:
      'In Firefox, you can right click on the request in the "Network" tab and select "Copy all as HAR".',
    howToLine3:
      'In Chrome, you can also click "Copy all as HAR", but this does not copy the response contents. To get the full responses, you need to click "Save all as HAR with content" to get the full HAR.',
    howToLine4: "You can copy the full HAR file below.",
    howToImage1: "Example in Firefox",
    howToImage2: "Example in Chrome.",
    importHar: "Import HTTP archive",
  },
  importOpenApi: {
    intro:
      "Using this form, you can create stubs based on an OpenAPI (or Swagger) definition. This definition can be provided in both JSON and YAML format. Many APIs are accompanied by an OpenAPI definition, so this is a great way to create stubs for the API.",
    importOpenApiDefinition: "Import OpenAPI definition",
  },
  uploadStubs: {
    intro: "Press the button below to upload a YAML file with stubs.",
    stubsInFileAddedSuccessfully: "Stubs in file '%s' were added successfully.",
  },
  logIn: {
    logIn: "Log in",
  },
  scenarioForm: {
    scenarioName: "Scenario name",
    scenarioState: "Scenario state",
    scenarioHitCount: "Scenario hit count",
    addScenario: "Add scenario",
    updateScenario: "Update scenario",
    scenarioSetSuccessfully: "The scenario values were set successfully.",
  },
  scenarios: {
    scenarios: "Scenarios",
    description:
      "Scenarios can be used to make stubs stateful. On this page, you can manage the scenarios in HttPlaceholder. To read more about scenarios, go to [the documentation](%s).",
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
      "*Note*: this setting will be reset to its original value after restarting HttPlaceholder. To persist the setting, take a look at [the documentation](%s).",
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
      'Fill in the stub below in YAML format and click on "Save". For examples, [read the docs](%s).',
    advancedEditor: "Advanced editor",
    advancedEditorDescription:
      "Use advanced editor for editing the stub. The editor has code highlighting but is not suited for updating large stubs.",
    simpleEditor: "Simple editor",
    simpleEditorDescription:
      "Use simple editor for editing the stub. The editor has no code highlighting but is suited for updating large stubs.",
    stubNotFound: "Stub with ID %s was not found.",
    addStub: "Add stub",
    updateStub: "Update stub",
    basicAuthTitle: "Basic authentication username and password",
    insertIntoStub: "Insert into stub",
    insertThisExample: "Insert this example?",
    youHaveUnsavedChanges: "You have unsaved changes.",
    closeList: "Close list",
    filterPlaceholder: "Filter form helpers (press 'Escape' to close)...",
    addExample: "Add example",
    addGeneralStubInfo: "Add general stub info",
    addRequestCondition: "Add request condition",
    addResponseWriter: "Add response writer",
    multipleMethods: "Multiple methods",
    lineEndingAsProvided: "As provided in response body",
    lineEndingUnix: "UNIX line endings",
    lineEndingWindows: "Windows line endings",
    redirectTemporary: "Temporary redirect",
    redirectPermanent: "Permanent redirect",
    descriptionTitle: "Description",
    priorityTitle:
      "Set a stub priority (the higher the number, the higher the stub priority)",
    urlPathTitle: "URL path",
    fullPathTitle: "Full path (including query string)",
    queryStringTitle: "Query string",
    queryStringKeyPlaceholder: "Query string key",
    requestHeadersTitle: "Request headers",
    requestHeadersKeyPlaceholder: "Request header name",
    clientIpTitle:
      "Client IP (e.g. '127.0.0.1' or '127.0.0.0/30' to provide an IP range)",
    hostnameTitle: "Hostname (e.g. 'httplaceholder.com')",
    requestBodyTitle: "Request body",
    formBodyTitle: "Form body",
    formBodyKeyPlaceholder: "Posted form value key",
    minHitsTitle:
      "Minimum amount of hits (inclusive) that any stub under the same scenario should be hit",
    maxHitsTitle:
      "Maximum amount of hits (exclusive) that any stub under the same scenario should be hit",
    exactHitsTitle:
      "Exact amount of hits that any stub under the same scenario should be hit",
    scenarioStateTitle: "State the scenario should be in",
    contentTypeTitle: "Content type of response",
    extraDurationTitle: "Extra duration in milliseconds",
    responseBodyHint:
      'Select a type of response and fill in the actual response that should be returned and press "Insert".',
    responseBodySelectType: "Select a response type...",
    responseBodyBase64Hint:
      'You can upload a **file** for use in the Base64 response or click on "show text input" and insert **plain text** that will be encoded to Base64 on inserting.',
    responseBodyUploadAFile: "Upload a file",
    responseBodyShowTextInput: "Show text input",
    responseBodyEnableDynamicMode: "Enable dynamic mode",
    responseBodyPrettifyJson: "Prettify JSON",
    responseBodyMinifyJson: "Minify JSON",
    responseBodyPrettifyXml: "Prettify XML",
    responseBodyMinifyXml: "Minify XML",
    insertScenarioName: "Insert new scenario name",
    selectExistingScenario: "Select existing scenario",
    enableDisableDynamicMode: "Enable / disable dynamic mode",
    enableDisableDynamicModeHint:
      "To learn more about the dynamic mode, read more in [the documentation](%s).",
    stringCheckerInputAddRow: "Add another row",
    stringCheckerInputDeleteRow: "Delete row",
    insertNewTenantName: "Insert new tenant name",
    selectExistingTenant: "Select existing tenant",
    insertVariableHandler: "Insert variable handler",
    selectVariableHandler:
      "Select a variable handler to insert in the response...",
    selectVariableHandlerExample:
      "Select an example to insert in the response...",
    saveAsNewStub: "Save as new stub",
    resetToDefaults: "Reset to defaults?",
  },
  stubFormHelperTitles: {
    example: "Add example",
    description: "Description",
    priority: "Priority",
    disableStub: "Disable stub",
    tenant: "Tenant",
    scenario: "Scenario",
    httpMethod: "HTTP method",
    uri: "URI",
    urlPath: "URL path",
    fullPath: "Full path",
    queryString: "Query string",
    https: "HTTPS",
    headers: "Headers",
    basicAuthentication: "Basic authentication",
    requestHeaders: "Headers",
    requestBody: "Request body",
    formBody: "Form body",
    host: "Host",
    clientIp: "Client IP",
    hostname: "Hostname",
    json: "JSON",
    jsonPath: "JSONPath",
    jsonObject: "JSON object",
    jsonArray: "JSON array",
    xml: "XML",
    xpath: "XPath",
    minHits: "Scenario min hit counter",
    maxHits: "Scenario max hit counter",
    exactHits: "Scenario exact hit counter",
    scenarioState: "Scenario state check",
    statusCode: "HTTP status code",
    responseBody: "Response body",
    responseBodyPlainText: "Plain text body",
    responseBodyJson: "JSON body",
    responseBodyXml: "XML body",
    responseBodyHtml: "HTML body",
    responseBodyBase64: "Base64 (binary) body",
    responseDynamicMode: "Enable / disable dynamic mode",
    stringReplace: "String replace in response body",
    regexReplace: "Regex replace in response body",
    jsonPathReplace: "JSONPath replace in response body",
    lineEndings: "Line endings",
    image: "Image",
    responseHeaders: "Response headers",
    responseContentType: "Content type",
    redirect: "Redirect",
    clearState: "Clear scenario state",
    newScenarioState: "Set new scenario state",
    other: "Other",
    reverseProxy: "Reverse proxy",
    abortConnection: "Abort connection",
    extraDuration: "Extra duration",
  },
  stubFormHelperDescriptions: {
    example: "Pick an example from the list to start creating your stub.",
    tenant:
      "The stub also has a 'tenant' field defined. This is a free text field which is optional. This field makes it possible to do operations of multiple stubs at once (e.g. delete all stubs with a specific tenant, get all stubs of a specific tenant or update all stubs of a specific tenant).",
    scenario:
      "The stub has a 'scenario' field which can optionally be filled in. When filling in this variable, stubs that have the same scenario value can be used to have stateful behavior. It is, for example, possible to use a condition checker to see if the scenario has been hit at least an x amount of times or if the scenario is in a specific state.",
    description:
      "A free text field where you can specify where the stub is for. It is optional.",
    priority:
      "There are cases when a request matches multiple stub. If this is the case, you can use the 'priority' element. With the priority element, you can specify which stub should be used if multiple stubs are found. The stub with the highest priority will be used. If you don't set the priority on the stub, it will be 0 by default.",
    disable:
      "By setting 'enabled' to false, the stub will not be used when determining which stub should be executed for a request.",
    httpMethod:
      "This condition checker can check the HTTP method (e.g. GET, POST, PUT, DELETE etc.). You can also provide an array of multiple HTTP methods; a request with any of these HTTP methods will then succeed.",
    urlPath:
      "The path condition is used to check a part of the URL path (so the part after http://... and before the query string). The condition can both check on substring and regular expressions.",
    queryString:
      "This condition checker can check the query string in a name-value collection like way. The condition can both check on substring and regular expressions. Place a new query string condition on a new line in the form of 'key: expected_value'.",
    fullPath:
      "This condition checker looks a lot like the path checker, but this checker also checks extra URL parameters, like the query string. The condition can both check on substring and regular expressions.",
    isHttps:
      "This condition checker can be used to verify if a request uses HTTPS or not.",
    body: "This condition checker can check whether the posted body corresponds to the given rules in the stub. It is possible to add multiple conditions. Add one condition per line. The condition can both check on substring and regular expressions.",
    formBody:
      "The form value condition checker can check whether the posted form values correspond to the given rules in the stub. It is possible to add multiple conditions. The condition can both check on substring and regular expressions. Place a new form body condition on a new line in the form of 'key: expected_value'.",
    xpath:
      "Using the XPath condition checker, you can check the posted XML body to see if it contains the correct elements. It is possible to add multiple conditions. If no namespaces are set in the stub, HttPlaceholder will try to fetch the namespaces itself using a regular expression.\n\nIt is also possible to (pre)-set the XML namespaces of a posted XML body. If no namespaces are set in the stub, HttPlaceholder will try to fetch the namespaces itself using a regular expression.",
    jsonPath:
      "Using the JSONPath condition checker, you can check the posted JSON body to see if it contains the correct elements. It is possible to add multiple conditions. Add one condition per line.",
    jsonObject:
      "Using the JSON condition checker, you can check whether a posted JSON object contains the correct elements.",
    jsonArray:
      "Using the JSON condition checker, you can check whether a posted JSON array contains the correct elements.",
    basicAuthentication:
      "This condition checker can check whether the sent basic authentication matches with the data in the stub.",
    clientIp:
      "It is also possible to set a condition to check the the client IP. A condition can be set for a single IP address or a whole IP range.",
    hostname:
      "It is possible to check if a hostname in a request is correct. This condition can be used with regular expressions if needed.",
    headers:
      "This condition checker can check whether the sent headers match with the headers in the stub. The condition can both check on substring and regular expressions. Place a new header condition on a new line in the form of 'key: expected_value'.",
    statusCode:
      "Defines the HTTP status code that should be returned. Default is HTTP 200 (OK).",
    responseBody: "Provide the body that should be added to the response.",
    responseBodyPlainText: "Let the stub return a plain text response.",
    responseBodyJson: "Let the stub return a JSON response.",
    responseBodyXml: "Let the stub return an XML response.",
    responseBodyHtml: "Let the stub return an HTML response.",
    responseBodyBase64:
      "Let the stub return a binary in the form of Base64. When executing the stub, the Base64 string will be decoded and returned to the client.",
    responseDynamicMode:
      "Dynamic mode makes it possible to add variables to the response body or response headers of the stub.",
    responseHeaders:
      "Provide a set of headers that should be added to the response. Place a header on a new line in the form of 'key: value'.",
    responseContentType: "This sets the Content-Type response header.",
    extraDuration:
      "Whenever you want to simulate a busy web service, you can use the 'extraDuration' response writer. You can set the number of extra milliseconds HttPlaceholder should wait and the request will actually take that much time to complete.",
    image:
      "If you want to create placeholder images, use this response writer. With this response writer, JPEG, PNG, BMP and GIF images can be returned.",
    redirect:
      "The permanent and temporary redirect response writers are short hands for defining redirects in you stub. If you set an URL on the 'temporaryRedirect' property, HttPlaceholder will redirect the user with an HTTP 307, and when you use the 'permanentRedirect' an HTTP 301.",
    dynamicMode:
      "In order to make the responses in HttPlaceholder a bit more dynamic, the 'dynamic mode' was introduced. This makes it possible to add variables to your responses that can be parsed. As of now, these variables can be used in the response body (text only) and the response headers. The only requirement is that you set this switch to on (by default, it is set to off and the variables will not be parsed).",
    reverseProxy: "A simple reverse proxy for letting a stub call other URLs.",
    abortConnection:
      "If you want this stub to abort the connection to see how your application reacts, use this.",
    lineEndings:
      "Specify whether to enforce Windows or UNIX line endings in the response.",
    minHits:
      "Specifies the minimum amount of hits (inclusive) that any stub under the same scenario should be hit.",
    maxHits:
      "Specifies the maximum amount of hits (exclusive) that any stub under the same scenario should be hit.",
    exactHits:
      "Specifies the exact amount of hits that any stub under the same scenario should be hit.",
    scenarioState: "Specifies the state the scenario should be in.",
    clearState:
      "Specifies that when the stub is hit, the scenario (both the state and hit counter) should be reset to its original values.",
    newScenarioState:
      "Specifies the new scenario state the scenario should be in when the stub is hit.",
    stringReplace:
      "A response writer for replacing a string in the response body. Can be used with the dynamic mode.",
    regexReplace:
      "A response writer for replacing a string in the response body based on a regular expression. Can be used with the dynamic mode.",
    jsonPathReplace:
      "A response writer for replacing a value in a JSON response body based on a JSONPath expression. Can be used with the dynamic mode.",
  },
  stringChecking: {
    equals: "Equals",
    equalsDescription:
      "checks if the input is exactly equal to this string, case sensitive",
    equalsci: "Equals case insensitive",
    equalsciDescription: "same as keyword above, but case insensitive",
    notequals: "Not equals",
    notequalsDescription:
      "checks if the input is not equal to this string, case sensitive",
    notequalsci: "Not equals case insensitive",
    notequalsciDescription: "same as keyword above, but case insensitive",
    contains: "Contains",
    containsDescription:
      "checks if the input contains this string, case sensitive",
    containsci: "Contains case insensitive",
    containsciDescription: "same as keyword above, but case insensitive",
    notcontains: "Not contains",
    notcontainsDescription:
      "checks if the input does not contain this string, case sensitive",
    notcontainsci: "Not contains case insensitive",
    notcontainsciDescription: "same as keyword above, but case insensitive",
    startswith: "Starts with",
    startswithDescription:
      "checks if the input starts with this string, case sensitive",
    startswithci: "Starts with case insensitive",
    startswithciDescription: "same as keyword above, but case insensitive",
    doesnotstartwith: "Does not start with",
    doesnotstartwithDescription:
      "checks if the input does not start with this string, case sensitive",
    doesnotstartwithci: "Does not start with case insensitive",
    doesnotstartwithciDescription:
      "same as keyword above, but case insensitive",
    endswith: "Ends with",
    endswithciDescription:
      "checks if the input ends with this string, case sensitive",
    endswithci: "Ends with case insensitive",
    endswithDescription: "same as keyword above, but case insensitive",
    doesnotendwith: "Does not end with",
    doesnotendwithDescription:
      "checks if the input does not end with this string, case sensitive",
    doesnotendwithci: "Does not end with case insensitive",
    doesnotendwithciDescription: "same as keyword above, but case insensitive",
    regex: "Regular expression",
    regexDescription: "checks if the input matches this regular expression",
    regexnomatches: "Regular expression no matches",
    regexnomatchesDescription:
      "checks if the input does not match this regular expression",
    minlength: "Minimum length",
    minLengthDescription:
      "checks if the input has a minimum (inclusive) string length",
    maxlength: "Maximum length",
    maxlengthDescription:
      "checks if the input has a maximum (inclusive) string length",
    exactlength: "Exact length",
    exactlengthDescription: "checks if the input has an exact string length",
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
    disabled: "disabled",
    stubIsReadonly: "Stub is read-only",
    viewAllRequests: "View all requests made for this stub",
    duplicate: "Duplicate",
    duplicateThisStub: "Duplicate this stub",
    updateStub: "Update this stub",
    disableStub: "Disable stub",
    enableStub: "Enable stub",
    enable: "Enable",
    disable: "Disable",
    setScenario: "Set scenario",
    deleteStub: "Delete the stub",
    deleteStubWithId: "Delete stub '%s'?",
    stubLocation: "Stub location",
    stubsAddedSuccessfully: "Stubs were added successfully.",
    stubUpdatedSuccessfully: "Stub was updated successfully.",
    stubDeletedSuccessfully: "Stub was deleted successfully.",
  },
  errors: {
    errorFormattingJson: "Error occurred while formatting JSON: %s",
    errorFormattingXml: "Error occurred while formatting XML: %s",
    errorDuringParsingOfYaml: "Something went wrong while parsing the YAML: %s",
    somethingWentWrongServer:
      "Something went wrong while contacting the server.",
  },
};
