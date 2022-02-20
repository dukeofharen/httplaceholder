import { defineStore } from "pinia";
import yaml from "js-yaml";
import { error } from "@/utils/toast";
import { resources } from "@/constants/resources";
import { defaultValues } from "@/domain/stubForm/default-values";
import { ResponseBodyType } from "@/domain/stubForm/response-body-type";
import { vsprintf } from "sprintf-js";
import type { StubModel } from "@/domain/stub/stub-model";
import type { LineEndingType } from "@/domain/stub/enums/line-ending-type";

type StubFormState = {
  input: string;
  inputHasMultipleStubs: boolean;
  currentSelectedFormHelper: string;
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
      currentSelectedFormHelper: "",
    } as StubFormState),
  getters: {
    getInput: (state) => state.input,
    getInputLength: (state) => state.input.length,
    getCurrentSelectedFormHelper: (state) => state.currentSelectedFormHelper,
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
    getInputHasMultipleStubs: (state) => state.inputHasMultipleStubs,
  },
  actions: {
    openFormHelper(key: string): void {
      this.currentSelectedFormHelper = key;
    },
    closeFormHelper(): void {
      this.currentSelectedFormHelper = "";
    },
    setInput(input: string): void {
      this.input = input;
      this.inputHasMultipleStubs = /^-/gm.test(input);
    },
    setDefaultDescription(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.description = defaultValues.description;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setDefaultPriority(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.priority = defaultValues.priority;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setDefaultPath(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.url) {
            parsed.conditions.url = {};
          }

          parsed.conditions.url.path = defaultValues.urlPath;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setDefaultFullPath(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          if (!parsed.conditions.url) {
            parsed.conditions.url = {};
          }

          parsed.conditions.url.fullPath = defaultValues.fullPath;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setDefaultQuery(): void {
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

          parsed.conditions.url.query = {
            ...parsed.conditions.url.query,
            ...defaultValues.query,
          };
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
        }
      });
    },
    setTenant(tenant: string): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.tenant = tenant;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setScenario(scenario: string): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.scenario = scenario;
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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

          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
        }
      });
    },
    setLineEndings(lineEndings: LineEndingType): void {
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

          this.input = yaml.dump(parsed);
        }
      });
    },
    setDynamicMode(value: boolean): void {
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

          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
        }
      });
    },
    setStubDisabled(): void {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.enabled = false;
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
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
          this.input = yaml.dump(parsed);
        }
      });
    },
  },
});
