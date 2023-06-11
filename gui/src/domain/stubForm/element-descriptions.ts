export const elementDescriptions = {
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
};
