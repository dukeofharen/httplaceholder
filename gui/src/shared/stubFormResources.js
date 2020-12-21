const tooltipResources = {
  id:
    "The ID of the stub. If you don't fill in an ID, an ID will be calculated when the stub is added. If you provide an ID of a stub that already exists, that stub will be overwritten with this one.",
  tenant:
    "The stub also has a 'tenant' field defined. This is a free text field which is optional. This field makes it possible to do operations of multiple stubs at once (e.g. delete all stubs with a specific tenant, get all stubs of a specific tenant or update all stubs of a specific tenant).",
  description:
    "A free text field where you can specify where the stub is for. It is optional.",
  priority:
    "There are cases when a request matches multiple stub. If this is the case, you can use the 'priority' element. With the priority element, you can specify which stub should be used if multiple stubs are found. The stub with the highest priority will be used. If you don't set the priority on the stub, it will be 0 by default.",
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
  body:
    "This condition checker can check whether the posted body corresponds to the given rules in the stub. It is possible to add multiple conditions. Add one condition per line. Add The condition can both check on substring and regular expressions.",
  formBody:
    "The form value condition checker can check whether the posted form values correspond to the given rules in the stub. It is possible to add multiple conditions. The condition can both check on substring and regular expressions. Place a new form body condition on a new line in the form of 'key: expected_value'.",
  xpath:
    "Using the XPath condition checker, you can check the posted XML body to see if it contains the correct elements. It is possible to add multiple conditions. If no namespaces are set in the stub, HttPlaceholder will try to fetch the namespaces itself using a regular expression.\n\nIt is also possible to (pre)-set the XML namespaces of a posted XML body. If no namespaces are set in the stub, HttPlaceholder will try to fetch the namespaces itself using a regular expression.",
  xpathNamespaces:
    "Fill in the XML namespaces here. If no namespaces are set in the stub, HttPlaceholder will try to fetch the namespaces itself using a regular expression. Place a new namespace on a new line in the form of 'key: expected_value'.",
  jsonPath:
    "Using the JSONPath condition checker, you can check the posted JSON body to see if it contains the correct elements. It is possible to add multiple conditions. Add one condition per line.",
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
  responseBodyType:
    "Here you can select the type of response you would like to return.",
  responseBody: "Provide the body that should be added to the response.",
  responseHeaders:
    "Provide a set of headers that should be added to the response. Place a header on a new line in the form of 'key: value'.",
  extraDuration:
    "Whenever you want to simulate a busy web service, you can use the 'extraDuration' response writer. You can set the number of extra milliseconds HttPlaceholder should wait and the request will actually take that much time to complete.",
  redirect:
    "The permanent and temporary redirect response writers are short hands for defining redirects in you stub. If you set an URL on the 'temporaryRedirect' property, HttPlaceholder will redirect the user with an HTTP 307, and when you use the 'permanentRedirect' an HTTP 301.",
  dynamicMode:
    "In order to make the responses in HttPlaceholder a bit more dynamic, the 'dynamic mode' was introduced. This makes it possible to add variables to your responses that can be parsed. As of now, these variables can be used in the response body (text only) and the response headers. The only requirement is that you set this switch to on (by default, it is set to off and the variables will not be parsed).",
  selectVariableHandler:
    "When clicking this button, you will be able to insert a variable handler. This variable handler will be parsed when the stub is executed.",
  base64Upload:
    "When clicking this button, you will be able to select a file from your PC. This file will be base64 encoded and added to the response body.",
  reverseProxyUrl: "The URL HttPlaceholder should send the request to.",
  appendQueryString:
    "Whether the query string of the request to HttPlaceholder should be appended to the request that will be sent to the proxy URL.",
  appendPath:
    "Whether the path of the request to HttPlaceholder (so the string after https://.../ and before the query string) should be appended to the request that will be sent to the proxy URL.",
  replaceRootUrl:
    "Whether the content returned by the proxy request should have its URLs replaced by the HttPlaceholder root URL. Both the response body and the HTTP headers will have its URLs replaced."
};

const isHttpsValues = {
  onlyHttps: 0,
  onlyHttp: 1,
  httpAndHttps: 2
};

const httpMethods = ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS", "HEAD"];

const httpStatusCodes = [
  {
    code: 100,
    name: "Continue"
  },
  {
    code: 101,
    name: "Switching Protocols"
  },
  {
    code: 102,
    name: "Processing"
  },
  {
    code: 200,
    name: "OK"
  },
  {
    code: 201,
    name: "Created"
  },
  {
    code: 202,
    name: "Accepted"
  },
  {
    code: 203,
    name: "Non-authoritative Information"
  },
  {
    code: 204,
    name: "No Content"
  },
  {
    code: 205,
    name: "Reset Content"
  },
  {
    code: 206,
    name: "Partial Content"
  },
  {
    code: 207,
    name: "Multi-Status"
  },
  {
    code: 208,
    name: "Already Reported"
  },
  {
    code: 226,
    name: "IM Used"
  },
  {
    code: 300,
    name: "Multiple Choices"
  },
  {
    code: 301,
    name: "Moved Permanently"
  },
  {
    code: 302,
    name: "Found"
  },
  {
    code: 303,
    name: "See Other"
  },
  {
    code: 304,
    name: "Not Modified"
  },
  {
    code: 305,
    name: "Use Proxy"
  },
  {
    code: 307,
    name: "Temporary Redirect"
  },
  {
    code: 308,
    name: "Permanent Redirect"
  },
  {
    code: 400,
    name: "Bad Request"
  },
  {
    code: 401,
    name: "Unauthorized"
  },
  {
    code: 402,
    name: "Payment Required"
  },
  {
    code: 403,
    name: "Forbidden"
  },
  {
    code: 404,
    name: "Not Found"
  },
  {
    code: 405,
    name: "Method Not Allowed"
  },
  {
    code: 406,
    name: "Not Acceptable"
  },
  {
    code: 407,
    name: "Proxy Authentication Required"
  },
  {
    code: 408,
    name: "Request Timeout"
  },
  {
    code: 409,
    name: "Conflict"
  },
  {
    code: 410,
    name: "Gone"
  },
  {
    code: 411,
    name: "Length Required"
  },
  {
    code: 412,
    name: "Precondition Failed"
  },
  {
    code: 413,
    name: "Payload Too Large"
  },
  {
    code: 414,
    name: "Request-URI Too Long"
  },
  {
    code: 415,
    name: "Unsupported Media Type"
  },
  {
    code: 416,
    name: "Requested range Not Satisfiable"
  },
  {
    code: 417,
    name: "Expectation Failed"
  },
  {
    code: 418,
    name: "I'm A Teapot"
  },
  {
    code: 421,
    name: "Misdirected Request"
  },
  {
    code: 422,
    name: "Unprocessable Entity"
  },
  {
    code: 423,
    name: "Locked"
  },
  {
    code: 424,
    name: "Failed Dependency"
  },
  {
    code: 426,
    name: "Upgrade Required"
  },
  {
    code: 428,
    name: "Precondition Required"
  },
  {
    code: 429,
    name: "Too Many Requests"
  },
  {
    code: 431,
    name: "Request Header Fields Too Large"
  },
  {
    code: 444,
    name: "Connection Closed Without Response"
  },
  {
    code: 451,
    name: "Unavailable For Legal Reasons"
  },
  {
    code: 499,
    name: "Client Closed Request"
  },
  {
    code: 500,
    name: "Internal Server Error"
  },
  {
    code: 501,
    name: "Not Implemented"
  },
  {
    code: 502,
    name: "Bad Gateway"
  },
  {
    code: 503,
    name: "Service Unavailable"
  },
  {
    code: 504,
    name: "Gateway Timeout"
  },
  {
    code: 505,
    name: "HTTP Version Not Supported"
  },
  {
    code: 506,
    name: "Variant Also Negotiates"
  },
  {
    code: 507,
    name: "Insufficient Storage"
  },
  {
    code: 508,
    name: "Loop Detected"
  },
  {
    code: 510,
    name: "Not Extended"
  },
  {
    code: 511,
    name: "Network Authentication Required"
  },
  {
    code: 599,
    name: "Network Connect Timeout Error"
  }
];

const defaultValues = {
  description: "A description for the stub.",
  priority: 1,
  urlPath: "/path",
  fullPath: "/path?query=val1",
};

const formHelperKeys = {
  tenant: "tenant",
  httpMethod: "httpMethod"
};

export {
  tooltipResources,
  isHttpsValues,
  httpMethods,
  httpStatusCodes,
  defaultValues,
  formHelperKeys
};
