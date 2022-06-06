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
  formIsDirty: boolean;
};

export interface SetResponseInput {
  type: ResponseBodyType;
  body: string;
}

const parseInput = (input: string): StubModel | undefined => {
  try {
    return yaml.load(input) as StubModel;
  } catch (e) {
    error(vsprintf(resources.errorDuringParsingOfYaml, [e]));
    return undefined;
  }
};

const handle = (func: () => any) => {
  try {
    return func();
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
    } as StubFormState),
  getters: {
    getInput: (state): string => state.input,
    getInputLength: (state): number => state.input.length,
    getCurrentSelectedFormHelper: (state): FormHelperKey =>
      state.currentSelectedFormHelper,
    getFormIsDirty: (state): boolean => state.formIsDirty,
    getResponseBodyType(state): ResponseBodyType {
      return handle(() => {
        const parsed = parseInput(state.input);
        if (parsed) {
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
        }

        return ResponseBodyType.text;
      });
    },
    getResponseBody(state): string {
      return handle(() => {
        const parsed = parseInput(state.input);
        if (parsed) {
          if (!parsed.response) {
            return "";
          }

          const res = parsed.response;
          return (
            res.text || res.json || res.xml || res.html || res.base64 || ""
          );
        }

        return "";
      });
    },
    getDynamicMode(state): boolean {
      return handle(() => {
        const parsed = parseInput(state.input);
        if (parsed) {
          if (!parsed.response) {
            return false;
          }

          return parsed.response.enableDynamicMode || false;
        }

        return false;
      });
    },
    getStubId(state): string {
      return handle(() => {
        const parsed = parseInput(state.input);
        if (parsed) {
          return parsed.id;
        }

        return "";
      });
    },
    getInputHasMultipleStubs: (state): boolean => state.inputHasMultipleStubs,
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
    setInput(input: string): void {
      this.input = input;
      this.formIsDirty = true;
      this.inputHasMultipleStubs = /^-/gm.test(input);
    },
    setDefaultDescription(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.description = defaultValues.description;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultPriority(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.priority = defaultValues.priority;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultPath(keyword: StringCheckingKeyword): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.url) {
            parsed.conditions.url = {};
          }

          if (!parsed.conditions.url.path) {
            parsed.conditions.url.path = {};
          }

          parsed.conditions.url.path[keyword.key] = defaultValues.urlPath;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultFullPath(keyword: StringCheckingKeyword): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.url) {
            parsed.conditions.url = {};
          }

          if (!parsed.conditions.url.fullPath) {
            parsed.conditions.url.fullPath = {};
          }

          parsed.conditions.url.fullPath[keyword.key] = defaultValues.fullPath;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultQuery(keyword: StringCheckingKeyword): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.url) {
            parsed.conditions.url = {};
          }

          if (!parsed.conditions.url.query) {
            parsed.conditions.url.query = {};
          }

          for (const key of Object.keys(defaultValues.query)) {
            parsed.conditions.url.query[key] = {};
            parsed.conditions.url.query[key][keyword.key] =
              keyword.key === "present" ? true : defaultValues.query[key];
          }

          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultIsHttps(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.url) {
            parsed.conditions.url = {};
          }

          parsed.conditions.url.isHttps = true;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultBasicAuth(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          parsed.conditions.basicAuthentication =
            defaultValues.basicAuthentication;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultRequestHeaders(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.headers) {
            parsed.conditions.headers = {};
          }

          parsed.conditions.headers = {
            ...parsed.conditions.headers,
            ...defaultValues.requestHeaders,
          };
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultRequestBody(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.body) {
            parsed.conditions.body = [];
          }

          parsed.conditions.body = parsed.conditions.body.concat(
            defaultValues.requestBody
          );
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultFormBody(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.form) {
            parsed.conditions.form = [];
          }

          parsed.conditions.form = parsed.conditions.form.concat(
            defaultValues.formBody
          );
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultClientIp(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          parsed.conditions.clientIp = defaultValues.clientIp;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultHostname(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          parsed.conditions.host = defaultValues.hostname;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultJsonPath(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.jsonPath) {
            parsed.conditions.jsonPath = [];
          }

          parsed.conditions.jsonPath = parsed.conditions.jsonPath.concat(
            defaultValues.jsonPath
          );
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultJsonObject(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          parsed.conditions.json = defaultValues.jsonObject;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultJsonArray(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          parsed.conditions.json = defaultValues.jsonArray;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultXPath(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.xpath) {
            parsed.conditions.xpath = [];
          }

          parsed.conditions.xpath = parsed.conditions.xpath.concat(
            defaultValues.xpath
          );
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setMethod(method: string): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          parsed.conditions.method = method;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setTenant(tenant: string): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.tenant = tenant;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setScenario(scenario: string): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.scenario = scenario;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setStatusCode(code: number): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          parsed.response.statusCode = code;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setResponseBody(payload: SetResponseInput): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
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

          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setResponseContentType(contentType: string): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          if (parsed.response.headers) {
            const key: string | undefined = Object.keys(
              parsed.response.headers
            ).find((k) => k.toLowerCase().trim() === "content-type");
            if (key) {
              delete parsed.response.headers[key];
            }
          }

          parsed.response.contentType = contentType;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultResponseHeaders(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
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
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultExtraDuration(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          parsed.response.extraDuration = defaultValues.extraDuration;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultTempRedirect(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          parsed.response.temporaryRedirect = defaultValues.redirect;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultPermanentRedirect(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          parsed.response.permanentRedirect = defaultValues.redirect;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setLineEndings(lineEndings: LineEndingType | undefined): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          if (!lineEndings) {
            delete parsed.response.lineEndings;
          } else {
            parsed.response.lineEndings = lineEndings;
          }

          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDynamicMode(value: boolean | undefined): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          if (value) {
            parsed.response.enableDynamicMode = value;
          } else {
            delete parsed.response.enableDynamicMode;
          }

          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultReverseProxy(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          parsed.response.reverseProxy = defaultValues.reverseProxy;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setStubDisabled(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.enabled = false;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultResponseContentType(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          parsed.response.contentType = defaultValues.responseContentType;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultImage(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          parsed.response.image = defaultValues.image;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultMinHits(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.scenario) {
            parsed.conditions.scenario = {};
          }

          parsed.conditions.scenario.minHits = defaultValues.minHits;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultMaxHits(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.scenario) {
            parsed.conditions.scenario = {};
          }

          parsed.conditions.scenario.maxHits = defaultValues.maxHits;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultExactHits(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.scenario) {
            parsed.conditions.scenario = {};
          }

          parsed.conditions.scenario.exactHits = defaultValues.exactHits;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultScenarioState(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.scenario) {
            parsed.conditions.scenario = {};
          }

          parsed.conditions.scenario.scenarioState =
            defaultValues.scenarioState;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setClearState(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          if (!parsed.response.scenario) {
            parsed.response.scenario = {};
          }

          parsed.response.scenario.clearState = true;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
    setDefaultNewScenarioState(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          if (!parsed.response.scenario) {
            parsed.response.scenario = {};
          }

          parsed.response.scenario.setScenarioState =
            defaultValues.newScenarioState;
          this.setInput(yaml.dump(parsed));
        }
      });
    },
  },
});
