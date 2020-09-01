import {isHttpsValues, responseBodyTypes} from "@/shared/stubFormResources";

export const mutationNames = {
  userTokenMutation: "storeUserToken",
  storeDarkTheme: "storeDarkTheme",
  storeMetadata: "storeMetadata",
  storeStubQueryStrings: "storeStubQueryStrings",
  storeQueryStrings: "storeQueryStrings",
  storeStubIsHttpsSelected: "storeStubIsHttpsSelected",
  storeIsHttpsSelected: "storeIsHttpsSelected",
  storeStubHeaders: "storeStubHeaders",
  storeHeaders: "storeHeaders",
  storeStubBody: "storeStubBody",
  storeBody: "storeBody",
  storeStubFormBody: "storeStubFormBody",
  storeFormBody: "storeFormBody",
  storeStubXPathAndNamespaces: "storeStubXPathAndNamespaces",
  storeXPathAndNamespaces: "storeXPathAndNamespaces",
  storeStubJsonPath: "storeStubJsonPath",
  storeJsonPath: "storeJsonPath",
  storeResponseBodyType: "storeResponseBodyType",
  storeStubResponseBodyType: "storeStubResponseBodyType",
  storeStubResponseHeaders: "storeStubResponseHeaders",
  storeResponseHeaders: "storeResponseHeaders",
  clearStubForm: "clearStubForm",
  setResponseHeader: "setResponseHeader"
};

export const actionNames = {
  addStubs: "addStubs",
  getStubs: "getStubs",
  getTenantNames: "getTenantNames",
  authenticate: "authenticate",
  getRequestsOverview: "getRequestsOverview",
  getRequest: "getRequest",
  clearRequests: "clearRequests",
  deleteAllStubs: "deleteAllStubs",
  getStub: "getStub",
  updateStub: "updateStub",
  createStubBasedOnRequest: "createStubBasedOnRequest",
  deleteStub: "deleteStub",
  ensureAuthenticated: "ensureAuthenticated",
  getMetadata: "getMetadata"
};

export function getEmptyStubForm() {
  return {
    queryStrings: "",
    body: "",
    formBody: "",
    xpath: "",
    xpathNamespaces: "",
    jsonPath: "",
    headers: "",
    isHttps: isHttpsValues.httpAndHttps,
    bodyResponseType: responseBodyTypes.text,
    responseBody: null,
    responseHeaders: "",
    stub: {
      id: null,
      tenant: null,
      description: null,
      priority: 0,
      conditions: {
        method: null,
        url: {
          path: null,
          query: null,
          fullPath: null,
          isHttps: null
        },
        body: null,
        form: null,
        xpath: null,
        jsonPath: null,
        basicAuthentication: {
          username: null,
          password: null
        },
        clientIp: null,
        hostname: null,
        headers: null
      },
      response: {
        statusCode: null,
        text: null,
        json: null,
        html: null,
        xml: null,
        base64: null,
        headers: null,
        extraDuration: null,
        temporaryRedirect: null,
        permanentRedirect: null,
        enableDynamicMode: false,
        reverseProxy: {
          url: null,
          appendQueryString: false,
          appendPath: false,
          replaceRootUrl: false
        }
      }
    }
  };
}
