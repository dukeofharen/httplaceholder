import { defineStore } from "pinia";
import yaml from "js-yaml";
import { error } from "@/utils/toast";
import { ResponseBodyType } from "@/domain/stubForm/response-body-type";
import { vsprintf } from "sprintf-js";
import type { StubModel } from "@/domain/stub/stub-model";
import type { LineEndingType } from "@/domain/stub/enums/line-ending-type";
import { FormHelperKey } from "@/domain/stubForm/form-helper-key";
import type { StubFormModel } from "@/domain/stub/stub-form-model";
import type { StubBasicAuthenticationModel } from "@/domain/stub/stub-basic-authentication-model";
import { translate } from "@/utils/translate";
import type { StubXpathModel } from "@/domain/stub/stub-xpath-model";
import { ResponseImageType } from "@/domain/stub/enums/response-image-type";
import type { StubResponseImageModel } from "@/domain/stub/stub-response-image-model";
import type { StubResponseReplaceModel } from "@/domain/stub/stub-response-replace-model";

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
    error(vsprintf(translate("errors.errorDuringParsingOfYaml"), [e]));
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
    getHostname(state): any {
      return handle((parsed) => parsed?.conditions?.host ?? "", state.input);
    },
    getRequestBody(state): any {
      return handle((parsed) => {
        const body = parsed?.conditions?.body ?? [];
        const result: any = {};
        for (const bodyItem of body) {
          const keys = Object.keys(bodyItem);
          if (!keys.length) {
            continue;
          }

          result[keys[0]] = bodyItem[keys[0]];
        }

        return result;
      }, state.input);
    },
    getForm(state): any {
      return handle((parsed) => {
        const form = parsed?.conditions?.form ?? [];
        const result: any = {};
        for (const item of form) {
          const subResult: any = {};
          for (const key of Object.keys(item.value)) {
            subResult[key] = item.value[key];
          }

          result[item.key] = subResult;
        }

        return result;
      }, state.input);
    },
    getScenarioMinHits(state): any {
      return handle(
        (parsed) => parsed?.conditions?.scenario?.minHits ?? 0,
        state.input,
      );
    },
    getScenarioMaxHits(state): any {
      return handle(
        (parsed) => parsed?.conditions?.scenario?.maxHits ?? 0,
        state.input,
      );
    },
    getScenarioExactHits(state): any {
      return handle(
        (parsed) => parsed?.conditions?.scenario?.exactHits ?? 0,
        state.input,
      );
    },
    getScenarioStateCheck(state): any {
      return handle(
        (parsed) => parsed?.conditions?.scenario?.scenarioState ?? "",
        state.input,
      );
    },
    getBasicAuth(state): StubBasicAuthenticationModel | undefined {
      return handle(
        (parsed) => parsed?.conditions?.basicAuthentication,
        state.input,
      );
    },
    getResponseContentType(state): any {
      return handle(
        (parsed) => parsed?.response?.contentType ?? "",
        state.input,
      );
    },
    getExtraDuration(state): any {
      return handle(
        (parsed) => parsed?.response?.extraDuration ?? 0,
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
    setHostname(hostname: string): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.host = hostname;
        this.setInput(parsed);
      }, this.input);
    },
    setRequestBody(input: any): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        const result = [];
        for (const key of Object.keys(input)) {
          const entry: any = {};
          entry[key] = input[key];
          result.push(entry);
        }

        parsed.conditions.body = result;
        this.setInput(parsed);
      }, this.input);
    },
    setForm(input: any): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        const result: StubFormModel[] = [];
        for (const key of Object.keys(input)) {
          result.push({
            key,
            value: input[key],
          });
        }

        parsed.conditions.form = result;
        this.setInput(parsed);
      }, this.input);
    },
    setScenarioMinHits(minHits: string) {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.scenario) {
          parsed.conditions.scenario = {};
        }

        parsed.conditions.scenario.minHits = parseInt(minHits);

        this.setInput(parsed);
      }, this.input);
    },
    setScenarioMaxHits(maxHits: string) {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.scenario) {
          parsed.conditions.scenario = {};
        }

        parsed.conditions.scenario.maxHits = parseInt(maxHits);

        this.setInput(parsed);
      }, this.input);
    },
    setScenarioExactHits(exactHits: string) {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.scenario) {
          parsed.conditions.scenario = {};
        }

        parsed.conditions.scenario.exactHits = parseInt(exactHits);

        this.setInput(parsed);
      }, this.input);
    },
    setScenarioStateCheck(state: string) {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.scenario) {
          parsed.conditions.scenario = {};
        }

        parsed.conditions.scenario.scenarioState = state;

        this.setInput(parsed);
      }, this.input);
    },
    setBasicAuth(username: string, password: string) {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.basicAuthentication) {
          parsed.conditions.basicAuthentication = {};
        }

        parsed.conditions.basicAuthentication.username = username;
        parsed.conditions.basicAuthentication.password = password;

        this.setInput(parsed);
      }, this.input);
    },
    setExtraDuration(duration: number) {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.response = {};
        }

        parsed.response.extraDuration = duration;

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
    setDefaultJsonPath(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.jsonPath) {
          parsed.conditions.jsonPath = [];
        }

        parsed.conditions.jsonPath = parsed.conditions.jsonPath.concat([
          {
            query: "$.people[0].name",
            expectedValue: "John",
          },
        ]);
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultJsonObject(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.json = {
          stringValue: "text",
          intValue: 3,
          array: ["value1", "value2"],
        };
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultJsonArray(): void {
      handle((parsed) => {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.json = [
          "value1",
          3,
          {
            key1: "value1",
            key2: 1.45,
          },
        ];
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

        parsed.conditions.xpath = parsed.conditions.xpath.concat([
          {
            queryString: '/object/a[text() = "TEST"]',
          },
          {
            queryString: '/object/b[text() = "TEST"]',
            namespaces: {
              soap: "http://www.w3.org/2003/05/soap-envelope",
              m: "http://www.example.org/stock/Reddy",
            },
          },
        ] as StubXpathModel[]);
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
          ...{
            Header1: "val1",
            Header2: "val2",
          },
        };
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultTempRedirect(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.temporaryRedirect = "https://google.com";
        this.setInput(parsed);
      }, this.input);
    },
    setDefaultPermanentRedirect(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.permanentRedirect = "https://google.com";
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

        parsed.response.reverseProxy = {
          url: "https://jsonplaceholder.typicode.com/todos",
          appendPath: true,
          appendQueryString: true,
          replaceRootUrl: true,
        };
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
    setDefaultImage(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.image = {
          type: ResponseImageType.Png,
          width: 1024,
          height: 256,
          backgroundColor: "#ffa0d3",
          text: "Placeholder text that will be drawn in the image",
          fontSize: 10,
          wordWrap: false,
        } as StubResponseImageModel;
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

        parsed.conditions.scenario.scenarioState = "new-state";
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

        parsed.response.replace = parsed.response.replace.concat([
          {
            text: "old value",
            ignoreCase: true,
            replaceWith: "New value",
          } as StubResponseReplaceModel,
        ]);

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

        parsed.response.replace = parsed.response.replace.concat([
          {
            regex: "(ipsum|consectetur)",
            replaceWith: "New value",
          } as StubResponseReplaceModel,
        ]);

        this.setInput(parsed);
      }, this.input);
    },
    setDefaultJsonPathReplace(): void {
      handle((parsed) => {
        if (!parsed.response) {
          parsed.response = {};
        }

        if (!parsed.response.replace) {
          parsed.response.replace = [];
        }

        parsed.response.replace = parsed.response.replace.concat([
          {
            jsonPath: "$.name",
            replaceWith: "New value",
          } as StubResponseReplaceModel,
        ]);

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

        parsed.response.scenario.setScenarioState = "new-state";
        this.setInput(parsed);
      }, this.input);
    },
  },
});
