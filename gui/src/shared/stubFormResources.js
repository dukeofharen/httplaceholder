const tooltipResources = {
  id: "The ID of the stub. If you don't fill in an ID, an ID will be calculated when the stub is added. If you provide an ID of a stub that already exists, that stub will be overwritten with this one.",
  tenant: "The stub also has a 'tenant' field defined. This is a free text field which is optional. This field makes it possible to do operations of multiple stubs at once (e.g. delete all stubs with a specific tenant, get all stubs of a specific tenant or update all stubs of a specific tenant).",
  description: "A free text field where you can specify where the stub is for. It is optional.",
  priority: "There are cases when a request matches multiple stub. If this is the case, you can use the 'priority' element. With the priority element, you can specify which stub should be used if multiple stubs are found. The stub with the highest priority will be used. If you don't set the priority on the stub, it will be 0 by default.",
  httpMethod: "This condition checker can check the HTTP method (e.g. GET, POST, PUT, DELETE etc.).",
  urlPath: "The path condition is used to check a part of the URL path (so the part after http://... and before the query string). The condition can both check on substring and regular expressions.",
  queryString: "This condition checker can check the query string in a name-value collection like way. The condition can both check on substring and regular expressions. Place a new query string condition on a new line in the form of 'key: expected_value'.",
  fullPath: "This condition checker looks a lot like the path checker, but this checker also checks extra URL parameters, like the query string. The condition can both check on substring and regular expressions.",
  isHttps: "This condition checker can be used to verify if a request uses HTTPS or not.",
  body: "This condition checker can check whether the posted body corresponds to the given rules in the stub. It is possible to add multiple conditions. Add one condition per line. Add The condition can both check on substring and regular expressions.",
  formBody: "The form value condition checker can check whether the posted form values correspond to the given rules in the stub. It is possible to add multiple conditions. The condition can both check on substring and regular expressions. Place a new form body condition on a new line in the form of 'key: expected_value'.",
  xpath: "Using the XPath condition checker, you can check the posted XML body to see if it contains the correct elements. It is possible to add multiple conditions. If no namespaces are set in the stub, HttPlaceholder will try to fetch the namespaces itself using a regular expression.\n\nIt is also possible to (pre)-set the XML namespaces of a posted XML body. If no namespaces are set in the stub, HttPlaceholder will try to fetch the namespaces itself using a regular expression.",
  xpathNamespaces: "Fill in the XML namespaces here. If no namespaces are set in the stub, HttPlaceholder will try to fetch the namespaces itself using a regular expression. Place a new namespace on a new line in the form of 'key: expected_value'.",
  jsonPath: "Using the JSONPath condition checker, you can check the posted JSON body to see if it contains the correct elements. It is possible to add multiple conditions. Add one condition per line.",
  basicAuthentication: "This condition checker can check whether the sent basic authentication matches with the data in the stub."
};

const formPlaceholderResources = {
  urlPath: "e.g. '/users' or '^/users$' (regex)",
  queryString: "id: 14\nfilter: last_name\napi_key: ^apikey1122$\n...",
  fullPath: "e.g. '/users?filter=first_name'",
  body: "first_name=duco\nid=14\n^somevalue$\n...",
  formBody: "first_name: duco\nid: 14\naddress: Some street 1\n...",
  xpath: "/object/a[text() = 'TEST']\n/object/b[text() = 'SomeValue']\n...",
  xpathNamespaces: "soap: http://www.w3.org/2003/05/soap-envelope\nm: http://www.example.org/stock/Reddy\n...",
  jsonPath: "$.phoneNumbers[?(@.type=='iPhone')]\n$.phoneNumbers[?(@.year=='2016')]\n..."
};

const formValidationMessages = {
  queryStringIncorrect: "You've filled in a value at 'Query strings', but the value could not be parsed. Make sure to fill in a correct value here.",
  formBodyIncorrect: "You've filled in a value at 'Form body', but the value could not be parsed. Make sure to fill in a correct value here.",
  priorityNotInteger: "Make sure the priority is numeric.",
  xpathNotFilledIn: "You filled in XML namespaces, but you didn't fill in any XPath expressions.",
  basicAuthInvalid: "Leave basic authentication username and password both empty or fill in both fields. You can't fill in only one field."
};

const formLabels = {
  id: "ID",
  tenant: "Stub tenant / category",
  description: "Description",
  priority: "Priority",
  httpMethod: "HTTP method",
  urlPath: "URL path",
  queryString: "Query strings (1 on each line)",
  fullPath: "Full path",
  onlyHttps: "The request should be made over HTTPS",
  onlyHttp: "The request should be made over HTTP",
  httpAndHttps: "The request can be made over HTTP or HTTPS",
  body: "Body",
  formBody: "Form body",
  xpath: "XPath expressions",
  xpathNamespaces: "XML namespaces (optional)",
  jsonPath: "JSONPath",
  basicAuthUsername: "Basic authentication username",
  basicAuthPassword: "Basic authentication password",
};

const isHttpsValues = {
  onlyHttps: 0,
  onlyHttp: 1,
  httpAndHttps: 2
};

export {
  tooltipResources,
  formPlaceholderResources,
  formValidationMessages,
  formLabels,
  isHttpsValues
};
