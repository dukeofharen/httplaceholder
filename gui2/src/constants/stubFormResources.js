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

// TODO delete this afterwards
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
