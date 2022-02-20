const elementDescriptions = {
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
    "This condition checker can check the HTTP method (e.g. GET, POST, PUT, DELETE etc.).",
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
};

const httpMethods = [
  "GET",
  "POST",
  "PUT",
  "DELETE",
  "PATCH",
  "OPTIONS",
  "HEAD",
];

const httpStatusCodes = [
  {
    code: 100,
    name: "Continue",
  },
  {
    code: 101,
    name: "Switching Protocols",
  },
  {
    code: 102,
    name: "Processing",
  },
  {
    code: 200,
    name: "OK",
  },
  {
    code: 201,
    name: "Created",
  },
  {
    code: 202,
    name: "Accepted",
  },
  {
    code: 203,
    name: "Non-authoritative Information",
  },
  {
    code: 204,
    name: "No Content",
  },
  {
    code: 205,
    name: "Reset Content",
  },
  {
    code: 206,
    name: "Partial Content",
  },
  {
    code: 207,
    name: "Multi-Status",
  },
  {
    code: 208,
    name: "Already Reported",
  },
  {
    code: 226,
    name: "IM Used",
  },
  {
    code: 300,
    name: "Multiple Choices",
  },
  {
    code: 301,
    name: "Moved Permanently",
  },
  {
    code: 302,
    name: "Found",
  },
  {
    code: 303,
    name: "See Other",
  },
  {
    code: 304,
    name: "Not Modified",
  },
  {
    code: 305,
    name: "Use Proxy",
  },
  {
    code: 307,
    name: "Temporary Redirect",
  },
  {
    code: 308,
    name: "Permanent Redirect",
  },
  {
    code: 400,
    name: "Bad Request",
  },
  {
    code: 401,
    name: "Unauthorized",
  },
  {
    code: 402,
    name: "Payment Required",
  },
  {
    code: 403,
    name: "Forbidden",
  },
  {
    code: 404,
    name: "Not Found",
  },
  {
    code: 405,
    name: "Method Not Allowed",
  },
  {
    code: 406,
    name: "Not Acceptable",
  },
  {
    code: 407,
    name: "Proxy Authentication Required",
  },
  {
    code: 408,
    name: "Request Timeout",
  },
  {
    code: 409,
    name: "Conflict",
  },
  {
    code: 410,
    name: "Gone",
  },
  {
    code: 411,
    name: "Length Required",
  },
  {
    code: 412,
    name: "Precondition Failed",
  },
  {
    code: 413,
    name: "Payload Too Large",
  },
  {
    code: 414,
    name: "Request-URI Too Long",
  },
  {
    code: 415,
    name: "Unsupported Media Type",
  },
  {
    code: 416,
    name: "Requested range Not Satisfiable",
  },
  {
    code: 417,
    name: "Expectation Failed",
  },
  {
    code: 418,
    name: "I'm A Teapot",
  },
  {
    code: 421,
    name: "Misdirected Request",
  },
  {
    code: 422,
    name: "Unprocessable Entity",
  },
  {
    code: 423,
    name: "Locked",
  },
  {
    code: 424,
    name: "Failed Dependency",
  },
  {
    code: 426,
    name: "Upgrade Required",
  },
  {
    code: 428,
    name: "Precondition Required",
  },
  {
    code: 429,
    name: "Too Many Requests",
  },
  {
    code: 431,
    name: "Request Header Fields Too Large",
  },
  {
    code: 444,
    name: "Connection Closed Without Response",
  },
  {
    code: 451,
    name: "Unavailable For Legal Reasons",
  },
  {
    code: 499,
    name: "Client Closed Request",
  },
  {
    code: 500,
    name: "Internal Server Error",
  },
  {
    code: 501,
    name: "Not Implemented",
  },
  {
    code: 502,
    name: "Bad Gateway",
  },
  {
    code: 503,
    name: "Service Unavailable",
  },
  {
    code: 504,
    name: "Gateway Timeout",
  },
  {
    code: 505,
    name: "HTTP Version Not Supported",
  },
  {
    code: 506,
    name: "Variant Also Negotiates",
  },
  {
    code: 507,
    name: "Insufficient Storage",
  },
  {
    code: 508,
    name: "Loop Detected",
  },
  {
    code: 510,
    name: "Not Extended",
  },
  {
    code: 511,
    name: "Network Authentication Required",
  },
  {
    code: 599,
    name: "Network Connect Timeout Error",
  },
];

const formHelperKeys = {
  tenant: "tenant",
  httpMethod: "httpMethod",
  statusCode: "statusCode",
  responseBody: "responseBody",
  responseBodyPlainText: "responseBodyPlainText",
  responseBodyJson: "responseBodyJson",
  responseBodyXml: "responseBodyXml",
  responseBodyHtml: "responseBodyHtml",
  responseBodyBase64: "responseBodyBase64",
  redirect: "redirect",
  lineEndings: "lineEndings",
  scenario: "scenario",
};

// TODO remove this afterwards
const responseBodyTypes = {
  text: "Text",
  json: "JSON",
  xml: "XML",
  html: "HTML",
  base64: "Base64",
};

// TODO delete this after migrating to enum
const lineEndingTypes = {
  windows: "windows",
  unix: "unix",
};

const stubFormHelpers = [
  {
    title: "Add general information",
    isMainItem: true,
  },
  {
    title: "Description",
    subTitle: elementDescriptions.description,
    defaultValueMutation: (store) => store.setDefaultDescription(),
  },
  {
    title: "Priority",
    subTitle: elementDescriptions.priority,
    defaultValueMutation: (store) => store.setDefaultPriority(),
  },
  {
    title: "Disable stub",
    subTitle: elementDescriptions.disable,
    defaultValueMutation: (store) => store.setStubDisabled(),
  },
  {
    title: "Tenant",
    subTitle: elementDescriptions.tenant,
    formHelperToOpen: formHelperKeys.tenant,
  },
  {
    title: "Scenario",
    subTitle: elementDescriptions.scenario,
    formHelperToOpen: formHelperKeys.scenario,
  },
  {
    title: "Add request condition",
    isMainItem: true,
  },
  {
    title: "HTTP method",
    subTitle: elementDescriptions.httpMethod,
    formHelperToOpen: formHelperKeys.httpMethod,
  },
  {
    title: "URL path",
    subTitle: elementDescriptions.urlPath,
    defaultValueMutation: (store) => store.setDefaultPath(),
  },
  {
    title: "Full path",
    subTitle: elementDescriptions.fullPath,
    defaultValueMutation: (store) => store.setDefaultFullPath(),
  },
  {
    title: "Query string",
    subTitle: elementDescriptions.queryString,
    defaultValueMutation: (store) => store.setDefaultQuery(),
  },
  {
    title: "HTTPS",
    subTitle: elementDescriptions.isHttps,
    defaultValueMutation: (store) => store.setDefaultIsHttps(),
  },
  {
    title: "Basic authentication",
    subTitle: elementDescriptions.basicAuthentication,
    defaultValueMutation: (store) => store.setDefaultBasicAuth(),
  },
  {
    title: "Headers",
    subTitle: elementDescriptions.headers,
    defaultValueMutation: (store) => store.setDefaultRequestHeaders(),
  },
  {
    title: "Request body",
    subTitle: elementDescriptions.body,
    defaultValueMutation: (store) => store.setDefaultRequestBody(),
  },
  {
    title: "Form body",
    subTitle: elementDescriptions.formBody,
    defaultValueMutation: (store) => store.setDefaultFormBody(),
  },
  {
    title: "Client IP",
    subTitle: elementDescriptions.clientIp,
    defaultValueMutation: (store) => store.setDefaultClientIp(),
  },
  {
    title: "Hostname",
    subTitle: elementDescriptions.hostname,
    defaultValueMutation: (store) => store.setDefaultHostname(),
  },
  {
    title: "JSONPath",
    subTitle: elementDescriptions.jsonPath,
    defaultValueMutation: (store) => store.setDefaultJsonPath(),
  },
  {
    title: "JSON object",
    subTitle: elementDescriptions.jsonObject,
    defaultValueMutation: (store) => store.setDefaultJsonObject(),
  },
  {
    title: "JSON array",
    subTitle: elementDescriptions.jsonArray,
    defaultValueMutation: (store) => store.setDefaultJsonArray(),
  },
  {
    title: "XPath",
    subTitle: elementDescriptions.xpath,
    defaultValueMutation: (store) => store.setDefaultXPath(),
  },
  {
    title: "Scenario min hit counter",
    subTitle: elementDescriptions.minHits,
    defaultValueMutation: (store) => store.setDefaultMinHits(),
  },
  {
    title: "Scenario max hit counter",
    subTitle: elementDescriptions.maxHits,
    defaultValueMutation: (store) => store.setDefaultMaxHits(),
  },
  {
    title: "Scenario exact hit counter",
    subTitle: elementDescriptions.exactHits,
    defaultValueMutation: (store) => store.setDefaultExactHits(),
  },
  {
    title: "Scenario state check",
    subTitle: elementDescriptions.scenarioState,
    defaultValueMutation: (store) => store.setDefaultScenarioState(),
  },
  {
    title: "Add response definition",
    isMainItem: true,
  },
  {
    title: "HTTP status code",
    subTitle: elementDescriptions.statusCode,
    formHelperToOpen: formHelperKeys.statusCode,
  },
  {
    title: "Response body",
    subTitle: elementDescriptions.responseBody,
    formHelperToOpen: formHelperKeys.responseBody,
  },
  {
    title: "Plain text body",
    subTitle: elementDescriptions.responseBodyPlainText,
    formHelperToOpen: formHelperKeys.responseBodyPlainText,
  },
  {
    title: "JSON body",
    subTitle: elementDescriptions.responseBodyJson,
    formHelperToOpen: formHelperKeys.responseBodyJson,
  },
  {
    title: "XML body",
    subTitle: elementDescriptions.responseBodyXml,
    formHelperToOpen: formHelperKeys.responseBodyXml,
  },
  {
    title: "HTML body",
    subTitle: elementDescriptions.responseBodyHtml,
    formHelperToOpen: formHelperKeys.responseBodyHtml,
  },
  {
    title: "Base64 (binary) body",
    subTitle: elementDescriptions.responseBodyBase64,
    formHelperToOpen: formHelperKeys.responseBodyBase64,
  },
  {
    title: "Response headers",
    subTitle: elementDescriptions.responseHeaders,
    defaultValueMutation: (store) => store.setDefaultResponseHeaders(),
  },
  {
    title: "Content type",
    subTitle: elementDescriptions.responseContentType,
    defaultValueMutation: (store) => store.setDefaultResponseContentType(),
  },
  {
    title: "Extra duration",
    subTitle: elementDescriptions.extraDuration,
    defaultValueMutation: (store) => store.setDefaultExtraDuration(),
  },
  {
    title: "Image",
    subTitle: elementDescriptions.image,
    defaultValueMutation: (store) => store.setDefaultImage(),
  },
  {
    title: "Redirect",
    subTitle: elementDescriptions.redirect,
    formHelperToOpen: formHelperKeys.redirect,
  },
  {
    title: "Line endings",
    subTitle: elementDescriptions.lineEndings,
    formHelperToOpen: formHelperKeys.lineEndings,
  },
  {
    title: "Reverse proxy",
    subTitle: elementDescriptions.reverseProxy,
    defaultValueMutation: (store) => store.setDefaultReverseProxy(),
  },
  {
    title: "Clear scenario state",
    subTitle: elementDescriptions.clearState,
    defaultValueMutation: (store) => store.setClearState(),
  },
  {
    title: "Set new scenario state",
    subTitle: elementDescriptions.newScenarioState,
    defaultValueMutation: (store) => store.setDefaultNewScenarioState(),
  },
];

export {
  elementDescriptions,
  httpMethods,
  httpStatusCodes,
  defaultValues,
  formHelperKeys,
  responseBodyTypes,
  lineEndingTypes,
  stubFormHelpers,
};
