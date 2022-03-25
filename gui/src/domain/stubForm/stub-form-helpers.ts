import { elementDescriptions } from "@/domain/stubForm/element-descriptions";
import { FormHelperKey } from "@/domain/stubForm/form-helper-key";

export interface StubFormHelper {
  title: string;
  subTitle?: string;
  isMainItem?: boolean;
  defaultValueMutation?: (store: any) => void;
  formHelperToOpen?: FormHelperKey;
}

export const stubFormHelpers = [
  {
    title: "Add general information",
    isMainItem: true,
  },
  {
    title: "Description",
    subTitle: elementDescriptions.description,
    defaultValueMutation: (store: any) => store.setDefaultDescription(),
  },
  {
    title: "Priority",
    subTitle: elementDescriptions.priority,
    defaultValueMutation: (store: any) => store.setDefaultPriority(),
  },
  {
    title: "Disable stub",
    subTitle: elementDescriptions.disable,
    defaultValueMutation: (store: any) => store.setStubDisabled(),
  },
  {
    title: "Tenant",
    subTitle: elementDescriptions.tenant,
    formHelperToOpen: FormHelperKey.Tenant,
  },
  {
    title: "Scenario",
    subTitle: elementDescriptions.scenario,
    formHelperToOpen: FormHelperKey.Scenario,
  },
  {
    title: "Add request condition",
    isMainItem: true,
  },
  {
    title: "HTTP method",
    subTitle: elementDescriptions.httpMethod,
    formHelperToOpen: FormHelperKey.HttpMethod,
  },
  {
    title: "URL path",
    subTitle: elementDescriptions.urlPath,
    defaultValueMutation: (store: any) => store.setDefaultPath(),
  },
  {
    title: "Full path",
    subTitle: elementDescriptions.fullPath,
    defaultValueMutation: (store: any) => store.setDefaultFullPath(),
  },
  {
    title: "Query string",
    subTitle: elementDescriptions.queryString,
    defaultValueMutation: (store: any) => store.setDefaultQuery(),
  },
  {
    title: "HTTPS",
    subTitle: elementDescriptions.isHttps,
    defaultValueMutation: (store: any) => store.setDefaultIsHttps(),
  },
  {
    title: "Basic authentication",
    subTitle: elementDescriptions.basicAuthentication,
    defaultValueMutation: (store: any) => store.setDefaultBasicAuth(),
  },
  {
    title: "Headers",
    subTitle: elementDescriptions.headers,
    defaultValueMutation: (store: any) => store.setDefaultRequestHeaders(),
  },
  {
    title: "Request body",
    subTitle: elementDescriptions.body,
    defaultValueMutation: (store: any) => store.setDefaultRequestBody(),
  },
  {
    title: "Form body",
    subTitle: elementDescriptions.formBody,
    defaultValueMutation: (store: any) => store.setDefaultFormBody(),
  },
  {
    title: "Client IP",
    subTitle: elementDescriptions.clientIp,
    defaultValueMutation: (store: any) => store.setDefaultClientIp(),
  },
  {
    title: "Hostname",
    subTitle: elementDescriptions.hostname,
    defaultValueMutation: (store: any) => store.setDefaultHostname(),
  },
  {
    title: "JSONPath",
    subTitle: elementDescriptions.jsonPath,
    defaultValueMutation: (store: any) => store.setDefaultJsonPath(),
  },
  {
    title: "JSON object",
    subTitle: elementDescriptions.jsonObject,
    defaultValueMutation: (store: any) => store.setDefaultJsonObject(),
  },
  {
    title: "JSON array",
    subTitle: elementDescriptions.jsonArray,
    defaultValueMutation: (store: any) => store.setDefaultJsonArray(),
  },
  {
    title: "XPath",
    subTitle: elementDescriptions.xpath,
    defaultValueMutation: (store: any) => store.setDefaultXPath(),
  },
  {
    title: "Scenario min hit counter",
    subTitle: elementDescriptions.minHits,
    defaultValueMutation: (store: any) => store.setDefaultMinHits(),
  },
  {
    title: "Scenario max hit counter",
    subTitle: elementDescriptions.maxHits,
    defaultValueMutation: (store: any) => store.setDefaultMaxHits(),
  },
  {
    title: "Scenario exact hit counter",
    subTitle: elementDescriptions.exactHits,
    defaultValueMutation: (store: any) => store.setDefaultExactHits(),
  },
  {
    title: "Scenario state check",
    subTitle: elementDescriptions.scenarioState,
    defaultValueMutation: (store: any) => store.setDefaultScenarioState(),
  },
  {
    title: "Add response definition",
    isMainItem: true,
  },
  {
    title: "HTTP status code",
    subTitle: elementDescriptions.statusCode,
    formHelperToOpen: FormHelperKey.StatusCode,
  },
  {
    title: "Response body",
    subTitle: elementDescriptions.responseBody,
    formHelperToOpen: FormHelperKey.ResponseBody,
  },
  {
    title: "Plain text body",
    subTitle: elementDescriptions.responseBodyPlainText,
    formHelperToOpen: FormHelperKey.ResponseBodyPlainText,
  },
  {
    title: "JSON body",
    subTitle: elementDescriptions.responseBodyJson,
    formHelperToOpen: FormHelperKey.ResponseBodyJson,
  },
  {
    title: "XML body",
    subTitle: elementDescriptions.responseBodyXml,
    formHelperToOpen: FormHelperKey.ResponseBodyXml,
  },
  {
    title: "HTML body",
    subTitle: elementDescriptions.responseBodyHtml,
    formHelperToOpen: FormHelperKey.ResponseBodyHtml,
  },
  {
    title: "Base64 (binary) body",
    subTitle: elementDescriptions.responseBodyBase64,
    formHelperToOpen: FormHelperKey.ResponseBodyBase64,
  },
  {
    title: "Enable / disable dynamic mode",
    subTitle: elementDescriptions.responseDynamicMode,
    formHelperToOpen: FormHelperKey.DynamicMode,
  },
  {
    title: "Response headers",
    subTitle: elementDescriptions.responseHeaders,
    defaultValueMutation: (store: any) => store.setDefaultResponseHeaders(),
  },
  {
    title: "Content type",
    subTitle: elementDescriptions.responseContentType,
    defaultValueMutation: (store: any) => store.setDefaultResponseContentType(),
  },
  {
    title: "Extra duration",
    subTitle: elementDescriptions.extraDuration,
    defaultValueMutation: (store: any) => store.setDefaultExtraDuration(),
  },
  {
    title: "Image",
    subTitle: elementDescriptions.image,
    defaultValueMutation: (store: any) => store.setDefaultImage(),
  },
  {
    title: "Redirect",
    subTitle: elementDescriptions.redirect,
    formHelperToOpen: FormHelperKey.Redirect,
  },
  {
    title: "Line endings",
    subTitle: elementDescriptions.lineEndings,
    formHelperToOpen: FormHelperKey.LineEndings,
  },
  {
    title: "Reverse proxy",
    subTitle: elementDescriptions.reverseProxy,
    defaultValueMutation: (store: any) => store.setDefaultReverseProxy(),
  },
  {
    title: "Clear scenario state",
    subTitle: elementDescriptions.clearState,
    defaultValueMutation: (store: any) => store.setClearState(),
  },
  {
    title: "Set new scenario state",
    subTitle: elementDescriptions.newScenarioState,
    defaultValueMutation: (store: any) => store.setDefaultNewScenarioState(),
  },
] as StubFormHelper[];
