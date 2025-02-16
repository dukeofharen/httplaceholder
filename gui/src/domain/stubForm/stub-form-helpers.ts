import { FormHelperKey } from '@/domain/stubForm/form-helper-key'
import { translate } from '@/utils/translate'

export enum StubFormHelperCategory {
  None = 'None',
  Examples = 'Examples',
  GeneralInfo = 'GeneralInfo',
  RequestCondition = 'RequestCondition',
  ResponseDefinition = 'ResponseDefinition',
}

export interface StubFormHelper {
  title: string
  isHeading?: boolean
  subTitle?: string
  defaultValueMutation?: (store: any) => void
  formHelperToOpen?: FormHelperKey
  stubFormHelperCategory: StubFormHelperCategory
}

export const stubFormHelpers = [
  {
    title: translate('stubFormHelperTitles.example'),
    subTitle: translate('stubFormHelperDescriptions.example'),
    stubFormHelperCategory: StubFormHelperCategory.Examples,
    formHelperToOpen: FormHelperKey.Example,
  },
  {
    title: translate('stubFormHelperTitles.description'),
    subTitle: translate('stubFormHelperDescriptions.description'),
    stubFormHelperCategory: StubFormHelperCategory.GeneralInfo,
    formHelperToOpen: FormHelperKey.Description,
  },
  {
    title: translate('stubFormHelperTitles.priority'),
    subTitle: translate('stubFormHelperDescriptions.priority'),
    stubFormHelperCategory: StubFormHelperCategory.GeneralInfo,
    formHelperToOpen: FormHelperKey.Priority,
  },
  {
    title: translate('stubFormHelperTitles.disableStub'),
    subTitle: translate('stubFormHelperDescriptions.disable'),
    stubFormHelperCategory: StubFormHelperCategory.GeneralInfo,
    defaultValueMutation: (store: any) => store.setStubDisabled(),
  },
  {
    title: translate('stubFormHelperTitles.tenant'),
    subTitle: translate('stubFormHelperDescriptions.tenant'),
    stubFormHelperCategory: StubFormHelperCategory.GeneralInfo,
    formHelperToOpen: FormHelperKey.Tenant,
  },
  {
    title: translate('stubFormHelperTitles.scenario'),
    subTitle: translate('stubFormHelperDescriptions.scenario'),
    stubFormHelperCategory: StubFormHelperCategory.GeneralInfo,
    formHelperToOpen: FormHelperKey.Scenario,
  },
  {
    title: translate('stubFormHelperTitles.httpMethod'),
    subTitle: translate('stubFormHelperDescriptions.httpMethod'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.HttpMethod,
  },
  {
    title: translate('stubFormHelperTitles.uri'),
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: translate('stubFormHelperTitles.urlPath'),
    subTitle: translate('stubFormHelperDescriptions.urlPath'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Path,
  },
  {
    title: translate('stubFormHelperTitles.fullPath'),
    subTitle: translate('stubFormHelperDescriptions.fullPath'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.FullPath,
  },
  {
    title: translate('stubFormHelperTitles.queryString'),
    subTitle: translate('stubFormHelperDescriptions.queryString'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Query,
  },
  {
    title: translate('stubFormHelperTitles.https'),
    subTitle: translate('stubFormHelperDescriptions.isHttps'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultIsHttps(),
  },
  {
    title: translate('stubFormHelperTitles.headers'),
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: translate('stubFormHelperTitles.basicAuthentication'),
    subTitle: translate('stubFormHelperDescriptions.basicAuthentication'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.BasicAuthentication,
  },
  {
    title: translate('stubFormHelperTitles.requestHeaders'),
    subTitle: translate('stubFormHelperDescriptions.headers'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Header,
  },
  {
    title: translate('stubFormHelperTitles.requestBody'),
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: translate('stubFormHelperTitles.requestBody'),
    subTitle: translate('stubFormHelperDescriptions.body'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Body,
  },
  {
    title: translate('stubFormHelperTitles.formBody'),
    subTitle: translate('stubFormHelperDescriptions.formBody'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Form,
  },
  {
    title: translate('stubFormHelperTitles.host'),
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: translate('stubFormHelperTitles.clientIp'),
    subTitle: translate('stubFormHelperDescriptions.clientIp'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.ClientIp,
  },
  {
    title: translate('stubFormHelperTitles.hostname'),
    subTitle: translate('stubFormHelperDescriptions.hostname'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.Host,
  },
  {
    title: translate('stubFormHelperTitles.json'),
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: translate('stubFormHelperTitles.jsonPath'),
    subTitle: translate('stubFormHelperDescriptions.jsonPath'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultJsonPath(),
  },
  {
    title: translate('stubFormHelperTitles.jsonObject'),
    subTitle: translate('stubFormHelperDescriptions.jsonObject'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultJsonObject(),
  },
  {
    title: translate('stubFormHelperTitles.jsonArray'),
    subTitle: translate('stubFormHelperDescriptions.jsonArray'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultJsonArray(),
  },
  {
    title: translate('stubFormHelperTitles.xml'),
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: translate('stubFormHelperTitles.xpath'),
    subTitle: translate('stubFormHelperDescriptions.xpath'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    defaultValueMutation: (store: any) => store.setDefaultXPath(),
  },
  {
    title: translate('stubFormHelperTitles.scenario'),
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
  },
  {
    title: translate('stubFormHelperTitles.minHits'),
    subTitle: translate('stubFormHelperDescriptions.minHits'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.ScenarioMinHits,
  },
  {
    title: translate('stubFormHelperTitles.maxHits'),
    subTitle: translate('stubFormHelperDescriptions.maxHits'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.ScenarioMaxHits,
  },
  {
    title: translate('stubFormHelperTitles.exactHits'),
    subTitle: translate('stubFormHelperDescriptions.exactHits'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.ScenarioExactHits,
  },
  {
    title: translate('stubFormHelperTitles.scenarioState'),
    subTitle: translate('stubFormHelperDescriptions.scenarioState'),
    stubFormHelperCategory: StubFormHelperCategory.RequestCondition,
    formHelperToOpen: FormHelperKey.ScenarioState,
  },
  {
    title: translate('stubFormHelperTitles.statusCode'),
    subTitle: translate('stubFormHelperDescriptions.statusCode'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.StatusCode,
  },
  {
    title: translate('stubFormHelperTitles.responseBody'),
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
  },
  {
    title: translate('stubFormHelperTitles.responseBody'),
    subTitle: translate('stubFormHelperDescriptions.responseBody'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBody,
  },
  {
    title: translate('stubFormHelperTitles.responseBodyPlainText'),
    subTitle: translate('stubFormHelperDescriptions.responseBodyPlainText'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBodyPlainText,
  },
  {
    title: translate('stubFormHelperTitles.responseBodyJson'),
    subTitle: translate('stubFormHelperDescriptions.responseBodyJson'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBodyJson,
  },
  {
    title: translate('stubFormHelperTitles.responseBodyXml'),
    subTitle: translate('stubFormHelperDescriptions.responseBodyXml'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBodyXml,
  },
  {
    title: translate('stubFormHelperTitles.responseBodyHtml'),
    subTitle: translate('stubFormHelperDescriptions.responseBodyHtml'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBodyHtml,
  },
  {
    title: translate('stubFormHelperTitles.responseBodyBase64'),
    subTitle: translate('stubFormHelperDescriptions.responseBodyBase64'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseBodyBase64,
  },
  {
    title: translate('stubFormHelperTitles.responseDynamicMode'),
    subTitle: translate('stubFormHelperDescriptions.responseDynamicMode'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.DynamicMode,
  },
  {
    title: translate('stubFormHelperTitles.stringReplace'),
    subTitle: translate('stubFormHelperDescriptions.stringReplace'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultStringReplace(),
  },
  {
    title: translate('stubFormHelperTitles.regexReplace'),
    subTitle: translate('stubFormHelperDescriptions.regexReplace'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultRegexReplace(),
  },
  {
    title: translate('stubFormHelperTitles.jsonPathReplace'),
    subTitle: translate('stubFormHelperDescriptions.jsonPathReplace'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultJsonPathReplace(),
  },
  {
    title: translate('stubFormHelperTitles.lineEndings'),
    subTitle: translate('stubFormHelperDescriptions.lineEndings'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.LineEndings,
  },
  {
    title: translate('stubFormHelperTitles.image'),
    subTitle: translate('stubFormHelperDescriptions.image'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultImage(),
  },
  {
    title: translate('stubFormHelperTitles.headers'),
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
  },
  {
    title: translate('stubFormHelperTitles.responseHeaders'),
    subTitle: translate('stubFormHelperDescriptions.responseHeaders'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultResponseHeaders(),
  },
  {
    title: translate('stubFormHelperTitles.responseContentType'),
    subTitle: translate('stubFormHelperDescriptions.responseContentType'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ResponseContentType,
  },
  {
    title: translate('stubFormHelperTitles.redirect'),
    subTitle: translate('stubFormHelperDescriptions.redirect'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.Redirect,
  },
  {
    title: translate('stubFormHelperTitles.scenario'),
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
  },
  {
    title: translate('stubFormHelperTitles.clearState'),
    subTitle: translate('stubFormHelperDescriptions.clearState'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setClearState(),
  },
  {
    title: translate('stubFormHelperTitles.newScenarioState'),
    subTitle: translate('stubFormHelperDescriptions.newScenarioState'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultNewScenarioState(),
  },
  {
    title: translate('stubFormHelperTitles.other'),
    isHeading: true,
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
  },
  {
    title: translate('stubFormHelperTitles.reverseProxy'),
    subTitle: translate('stubFormHelperDescriptions.reverseProxy'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setDefaultReverseProxy(),
  },
  {
    title: translate('stubFormHelperTitles.abortConnection'),
    subTitle: translate('stubFormHelperDescriptions.abortConnection'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    defaultValueMutation: (store: any) => store.setAbortConnection(),
  },
  {
    title: translate('stubFormHelperTitles.extraDuration'),
    subTitle: translate('stubFormHelperDescriptions.extraDuration'),
    stubFormHelperCategory: StubFormHelperCategory.ResponseDefinition,
    formHelperToOpen: FormHelperKey.ExtraDuration,
  },
] as StubFormHelper[]
