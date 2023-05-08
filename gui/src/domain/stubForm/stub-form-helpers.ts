import { elementDescriptions } from "@/domain/stubForm/element-descriptions";
import { FormHelperKey } from "@/domain/stubForm/form-helper-key";

export enum StubFormHelperCategory {
  Examples = "Examples",
  GeneralInfo = "GeneralInfo",
  RequestCondition = "RequestCondition",
  ResponseDefinition = "ResponseDefinition",
}

export interface StubFormHelper {
  title: string;
  isHeading?: boolean;
  subTitle?: string;
  defaultValueMutation?: (store: any) => void;
  formHelperToOpen?: FormHelperKey;
  stubFormHelperCategory: StubFormHelperCategory;
}

export const stubFormHelpers = [
  {
    title: "Add example",
    subTitle: elementDescriptions.example,
    stubFormHelperCategory: StubFormHelperCategory.Examples,
    formHelperToOpen: FormHelperKey.Example,
  },
  {
    title: "Description",
    subTitle: elementDescriptions.description,
    stubFormHelperCategory: StubFormHelperCategory.GeneralInfo,
    defaultValueMutation: (store: any) => store.setDefaultDescription(),
  },
  {
    title: "Priority",
    subTitle: elementDescriptions.priority,
    stubFormHelperCategory: StubFormHelperCategory.GeneralInfo,
    defaultValueMutation: (store: any) => store.setDefaultPriority(),
  },
  {
    title: "Disable stub",
    subTitle: elementDescriptions.disable,
    stubFormHelperCategory: StubFormHelperCategory.GeneralInfo,
    defaultValueMutation: (store: any) => store.setStubDisabled(),
  },
  {
    title: "Tenant",
    subTitle: elementDescriptions.tenant,
    stubFormHelperCategory: StubFormHelperCategory.GeneralInfo,
    formHelperToOpen: FormHelperKey.Tenant,
  },
  {
    title: "Scenario",
    subTitle: elementDescriptions.scenario,
    stubFormHelperCategory: StubFormHelperCategory.GeneralInfo,
    formHelperToOpen: FormHelperKey.Scenario,
  },
  {
    title: "HTTP method",
    subTitle: elementDescriptions.httpMethod,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.HttpMethod,
  },
  {
    title: "URI",
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: "URL path",
    subTitle: elementDescriptions.urlPath,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Path,
  },
  {
    title: "Full path",
    subTitle: elementDescriptions.fullPath,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.FullPath,
  },
  {
    title: "Query string",
    subTitle: elementDescriptions.queryString,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Query,
  },
  {
    title: "HTTPS",
    subTitle: elementDescriptions.isHttps,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultIsHttps(),
  },
  {
    title: "Headers",
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: "Basic authentication",
    subTitle: elementDescriptions.basicAuthentication,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultBasicAuth(),
  },
  {
    title: "Headers",
    subTitle: elementDescriptions.headers,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Header,
  },
  {
    title: "Request body",
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: "Request body",
    subTitle: elementDescriptions.body,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Body,
  },
  {
    title: "Form body",
    subTitle: elementDescriptions.formBody,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Form,
  },
  {
    title: "Host",
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: "Client IP",
    subTitle: elementDescriptions.clientIp,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultClientIp(),
  },
  {
    title: "Hostname",
    subTitle: elementDescriptions.hostname,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Host,
  },
  {
    title: "JSON",
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: "JSONPath",
    subTitle: elementDescriptions.jsonPath,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultJsonPath(),
  },
  {
    title: "JSON object",
    subTitle: elementDescriptions.jsonObject,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultJsonObject(),
  },
  {
    title: "JSON array",
    subTitle: elementDescriptions.jsonArray,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultJsonArray(),
  },
  {
    title: "XML",
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: "XPath",
    subTitle: elementDescriptions.xpath,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultXPath(),
  },
  {
    title: "Scenario",
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: "Scenario min hit counter",
    subTitle: elementDescriptions.minHits,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultMinHits(),
  },
  {
    title: "Scenario max hit counter",
    subTitle: elementDescriptions.maxHits,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultMaxHits(),
  },
  {
    title: "Scenario exact hit counter",
    subTitle: elementDescriptions.exactHits,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultExactHits(),
  },
  {
    title: "Scenario state check",
    subTitle: elementDescriptions.scenarioState,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultScenarioState(),
  },
  {
    title: "HTTP status code",
    subTitle: elementDescriptions.statusCode,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.StatusCode,
  },
  {
    title: "Response body",
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
  },
  {
    title: "Response body",
    subTitle: elementDescriptions.responseBody,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBody,
  },
  {
    title: "Plain text body",
    subTitle: elementDescriptions.responseBodyPlainText,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBodyPlainText,
  },
  {
    title: "JSON body",
    subTitle: elementDescriptions.responseBodyJson,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBodyJson,
  },
  {
    title: "XML body",
    subTitle: elementDescriptions.responseBodyXml,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBodyXml,
  },
  {
    title: "HTML body",
    subTitle: elementDescriptions.responseBodyHtml,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBodyHtml,
  },
  {
    title: "Base64 (binary) body",
    subTitle: elementDescriptions.responseBodyBase64,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBodyBase64,
  },
  {
    title: "Enable / disable dynamic mode",
    subTitle: elementDescriptions.responseDynamicMode,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.DynamicMode,
  },
  {
    title: "Line endings",
    subTitle: elementDescriptions.lineEndings,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.LineEndings,
  },
  {
    title: "Image",
    subTitle: elementDescriptions.image,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultImage(),
  },
  {
    title: "Headers",
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
  },
  {
    title: "Response headers",
    subTitle: elementDescriptions.responseHeaders,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultResponseHeaders(),
  },
  {
    title: "Content type",
    subTitle: elementDescriptions.responseContentType,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultResponseContentType(),
  },
  {
    title: "Redirect",
    subTitle: elementDescriptions.redirect,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.Redirect,
  },
  {
    title: "Scenario",
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
  },
  {
    title: "Clear scenario state",
    subTitle: elementDescriptions.clearState,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setClearState(),
  },
  {
    title: "Set new scenario state",
    subTitle: elementDescriptions.newScenarioState,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultNewScenarioState(),
  },
  {
    title: "Other",
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
  },
  {
    title: "Reverse proxy",
    subTitle: elementDescriptions.reverseProxy,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultReverseProxy(),
  },
  {
    title: "Abort connection",
    subTitle: elementDescriptions.abortConnection,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setAbortConnection(),
  },
  {
    title: "Extra duration",
    subTitle: elementDescriptions.extraDuration,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultExtraDuration(),
  },
] as StubFormHelper[];
