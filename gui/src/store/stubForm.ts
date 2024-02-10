import { defineStore } from "pinia";
import yaml from "js-yaml";
import { error } from "@/utils/toast";
import { resources } from "@/constants/resources";
import { defaultValues } from "@/domain/stubForm/default-values";
import { ResponseBodyType } from "@/domain/stubForm/response-body-type";
import { vsprintf } from "sprintf-js";
import type { StubModel } from "@/domain/stub/stub-model";
import type { LineEndingType } from "@/domain/stub/enums/line-ending-type";
import { FormHelperKey } from "@/domain/stubForm/form-helper-key";
import type { StringCheckingKeyword } from "@/constants/string-checking-keywords";

type StubFormState = {
  input: string;
  inputHasMultipleStubs: boolean;
  currentSelectedFormHelper: FormHelperKey;
  formHelperSelectorFilter: string;
  formIsDirty: boolean;
};

export interface SetResponseInput {
  type: ResponseBodyType;
  body: string;
}

const handle = (func: (parsed: StubModel) => any, input: string) => {
  try {
    const parsed = yaml.load(input) as StubModel;
    return func(parsed);
  } catch (e) {
    error(vsprintf(resources.errorDuringParsingOfYaml, [e]));
    return null;
  }
};

export const useStubFormStore = defineStore({
  id: "stubForm",
  state: () =>
    ({
      input: "",
      inputHasMultipleStubs: false,
      currentSelectedFormHelper: FormHelperKey.None,
      formIsDirty: false,
      formHelperSelectorFilter: "",
    }) as StubFormState,
  getters: {
    getInput: (state): string => state.input,
    getInputLength: (state): number => state.input.length,
    getCurrentSelectedFormHelper: (state): FormHelperKey =>
      state.currentSelectedFormHelper,
    getFormIsDirty: (state): boolean => state.formIsDirty,
    getFormHelperSelectorFilter: (state): string =>
      state.formHelperSelectorFilter,
    getResponseBodyType(state): ResponseBodyType {
      return handle((parsed) => {
        if (!parsed.response) {
          return ResponseBodyType.text;
        }

        const res = parsed.response;
        if (res.text) {
          return ResponseBodyType.text;
        } else if (res.json) {
          return ResponseBodyType.json;
        } else if (res.xml) {
          return ResponseBodyType.xml;
        } else if (res.html) {
          return ResponseBodyType.html;
        } else if (res.base64) {
          return ResponseBodyType.base64;
        }

        return ResponseBodyType.text;
      }, state.input);
    },
    getResponseBody(state): string {
      return handle((parsed) => {
        if (!parsed.response) {
          return "";
        }

        const res = parsed.response;
        return res.text || res.json || res.xml || res.html || res.base64 || "";
      }, state.input);
    },
    getDynamicMode(state): boolean {
      return handle((parsed) => {
        if (!parsed.response) {
          return false;
        }

        return parsed.response.enableDynamicMode || false;
      }, state.input);
    },
    getStubId(state): string {
      return handle((parsed) => parsed.id, state.input);
    },
    getInputHasMultipleStubs: (state): boolean => state.inputHasMultipleStubs,
    getDescription(state): string {
      return handle((parsed) => parsed?.description ?? "", state.input);
    },
    getPriority(state): string {
      return handle((parsed) => parsed?.priority ?? "0", state.input);
    },
    getUrlPath(state): any {
      return handle(
        (parsed) => parsed?.conditions?.url?.path ?? "",
        state.input,
      );
    },
    getFullPath(state): any {
      return handle(
        (parsed) => parsed?.conditions?.url?.fullPath ?? "",
        state.input,
      );
    },
    getQuery(state): any {
      return handle(
        (parsed) => parsed?.conditions?.url?.query ?? "",
        state.input,
      );
    },
    getRequestHeaders(state): any {
      return handle((parsed) => parsed?.conditions?.headers ?? {}, state.input);
    },
    getClientIp(state): any {
      return handle(
        (parsed) => parsed?.conditions?.clientIp ?? "",
        state.input,
      );
    },
  },
  actions: {
    openFormHelper(key: FormHelperKey): void {
      this.currentSelectedFormHelper = key;
    },
    closeFormHelper(): void {
      this.currentSelectedFormHelper = FormHelperKey.None;
    },
    setFormIsDirty(formIsDirty: boolean) {
      this.formIsDirty = formIsDirty;
    },
    setInput(input: StubModel | string): void {
      let serializedYaml: string;
      if (typeof input === "string") {
        serializedYaml = input;
      } else {
        serializedYaml = yaml.dump(input);
      }

      this.input = serializedYaml;
      this.formIsDirty = true;
      this.inputHasMultipleStubs = /^-/gm.test(serializedYaml);
    },
    setFormHelperSelectorFilter(filter: string) {
      this.formHelperSelectorFilter = filter;
    },
    setDescription(description: string): void {
      handle((parsed) => {
        parsed.description = description;
        this.setInput(parsed);
      }, this.input);
    },
    setPriority(priority: string): void {
      handle((parsed) => {
        parsed.priority = parseInt(priority);
        this.setInput(parsed);
      }, this.input);
    },
    setUrlPath(input: object) {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.url) {
          parsed.conditions.url = {};
        }

        parsed.conditions.url.path = input;
        this.setInput(parsed);
      }, this.input);
    },
    setFullPath(input: object) {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.url) {
          parsed.conditions.url = {};
        }

        parsed.conditions.url.fullPath = input;
        this.setInput(parsed);
      }, this.input);
    },
    setQuery(input: object) {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.url) {
          parsed.conditions.url = {};
        }

        parsed.conditions.url.query = input;
        this.setInput(parsed);
      }, this.input);
    },
    setRequestHeaders(input: object) {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.headers = input;
        this.setInput(parsed);
      }, this.input);
    },
    setClientIp(ip: string): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.clientIp = ip;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultIsHttps(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.url) {
          parsed.conditions.url = {};
        }

        parsed.conditions.url.isHttps = true;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultBasicAuth(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.basicAuthentication =
          defaultValues.basicAuthentication;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultRequestBody(keyword: StringCheckingKeyword): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.body) {
          parsed.conditions.body = [];
        }

        for (const body of defaultValues.requestBody) {
          const newBody: any = {};
          newBody[keyword.key] = keyword.defaultValue || body;
          parsed.conditions.body.push(newBody);
        }

        this.setInput(parsed);
      }, this.input);
    },
    setDefaultFormBody(keyword: StringCheckingKeyword): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.form) {
          parsed.conditions.form = [];
        }

        for (const key of Object.keys(defaultValues.formBody)) {
          const val = {} as any;
          val[keyword.key] =
            keyword.key === "present"
              ? true
              : keyword.defaultValue || defaultValues.formBody[key];
          parsed.conditions.form.push({
            key: key,
            value: val,
          });
        }

        this.setInput(parsed);
      }, this.input);
    },
    setDefaultHostname(keyword: StringCheckingKeyword): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.host) {
          parsed.conditions.host = {};
        }

        parsed.conditions.host[keyword.key] =
          keyword.defaultValue || defaultValues.hostname;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultJsonPath(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.jsonPath) {
          parsed.conditions.jsonPath = [];
        }

        parsed.conditions.jsonPath = parsed.conditions.jsonPath.concat(
          defaultValues.jsonPath,
        );
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultJsonObject(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.json = defaultValues.jsonObject;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultJsonArray(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.json = defaultValues.jsonArray;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultXPath(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.xpath) {
          parsed.conditions.xpath = [];
        }

        parsed.conditions.xpath = parsed.conditions.xpath.concat(
          defaultValues.xpath,
        );
        this.setInput(parsed);
      }, this.input);
    },
    setMethod(method: string | string[]): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.method = method;
        this.setInput(parsed);
      }, this.input);
    },
    setTenant(tenant: string): void {
      handle((parsed) => {
        parsed.tenant = tenant;
        this.setInput(parsed);
      }, this.input);
    },
    setScenario(scenario: string): void {
      handle((parsed) => {
        parsed.scenario = scenario;
        this.setInput(parsed);
      }, this.input);
    },
    setStatusCode(code: number): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.statusCode = code;
        this.setInput(parsed);
      }, this.input);
    },
    setResponseBody(payload: SetResponseInput): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        delete parsed.response.text;
        delete parsed.response.json;
        delete parsed.response.xml;
        delete parsed.response.html;
        delete parsed.response.base64;

        const responseType = payload.type || ResponseBodyType.text;
        const responseBody = payload.body || "";
        switch (responseType) {
          case ResponseBodyType.json:
            parsed.response.json = responseBody;
            break;
          case ResponseBodyType.xml:
            parsed.response.xml = responseBody;
            break;
          case ResponseBodyType.html:
            parsed.response.html = responseBody;
            break;
          case ResponseBodyType.base64:
            parsed.response.base64 = responseBody;
            break;
          default:
          case ResponseBodyType.text:
            parsed.response.text = responseBody;
            break;
        }

        this.setInput(parsed);
      }, this.input);
    },
    setResponseContentType(contentType: string): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        if (parsed.response.headers) {
          const key: string | undefined = Object.keys(
            parsed.response.headers,
          ).find((k) => k.toLowerCase().trim() === "content-type");
          if (key) {
            delete parsed.response.headers[key];
          }
        }

        parsed.response.contentType = contentType;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultResponseHeaders(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        if (!parsed.response.headers) {
          parsed.response.headers = {};
        }

        parsed.response.headers = {
          ...parsed.response.headers,
          ...defaultValues.responseHeaders,
        };
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultExtraDuration(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.extraDuration = defaultValues.extraDuration;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultTempRedirect(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.temporaryRedirect = defaultValues.redirect;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultPermanentRedirect(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.permanentRedirect = defaultValues.redirect;
        this.setInput(parsed);
      }, this.input);
    },
    setLineEndings(lineEndings: LineEndingType | undefined): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        if (!lineEndings) {
          delete parsed.response.lineEndings;
        } else {
          parsed.response.lineEndings = lineEndings;
        }

        this.setInput(parsed);
      }, this.input);
    },
    setDynamicMode(value: boolean | undefined): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        if (value) {
          parsed.response.enableDynamicMode = value;
        } else {
          delete parsed.response.enableDynamicMode;
        }

        this.setInput(parsed);
      }, this.input);
    },
    setDefaultReverseProxy(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.reverseProxy = defaultValues.reverseProxy;
        this.setInput(parsed);
      }, this.input);
    },
    setAbortConnection(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.abortConnection = true;
        this.setInput(parsed);
      }, this.input);
    },
    setStubDisabled(): void {
      handle((parsed) => {
        parsed.enabled = false;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultResponseContentType(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.contentType = defaultValues.responseContentType;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultImage(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.image = defaultValues.image;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultMinHits(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.scenario) {
          parsed.conditions.scenario = {};
        }

        parsed.conditions.scenario.minHits = defaultValues.minHits;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultMaxHits(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.scenario) {
          parsed.conditions.scenario = {};
        }

        parsed.conditions.scenario.maxHits = defaultValues.maxHits;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultExactHits(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.scenario) {
          parsed.conditions.scenario = {};
        }

        parsed.conditions.scenario.exactHits = defaultValues.exactHits;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultScenarioState(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.scenario) {
          parsed.conditions.scenario = {};
        }

        parsed.conditions.scenario.scenarioState = defaultValues.scenarioState;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultStringReplace(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        if (!parsed.response.replace) {
          parsed.response.replace = [];
        }

        parsed.response.replace = parsed.response.replace.concat(
          defaultValues.stringReplace,
        );

        this.setInput(parsed);
      }, this.input);
    },
    setDefaultRegexReplace(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        if (!parsed.response.replace) {
          parsed.response.replace = [];
        }

        parsed.response.replace = parsed.response.replace.concat(
          defaultValues.regexReplace,
        );

        this.setInput(parsed);
      }, this.input);
    },
    setClearState(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        if (!parsed.response.scenario) {
          parsed.response.scenario = {};
        }

        parsed.response.scenario.clearState = true;
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultNewScenarioState(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        if (!parsed.response.scenario) {
          parsed.response.scenario = {};
        }

        parsed.response.scenario.setScenarioState =
          defaultValues.newScenarioState;
        this.setInput(parsed);
      }, this.input);
    },
  },
});
