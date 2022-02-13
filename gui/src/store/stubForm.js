import { defineStore } from "pinia";
import yaml from "js-yaml";
import { error } from "@/utils/toast";
import { resources } from "@/constants/resources";
import {
  defaultValues,
  responseBodyTypes,
} from "@/constants/stubFormResources";

const parseInput = (input) => {
  try {
    return yaml.load(input);
  } catch (e) {
    error(resources.errorDuringParsingOfYaml.format(e));
    return null;
  }
};

const handle = (func) => {
  try {
    return func();
  } catch (e) {
    error(resources.errorDuringParsingOfYaml.format(e));
    return null;
  }
};

export const useStubFormStore = defineStore({
  id: "stubForm",
  state: () => ({
    input: "",
    inputHasMultipleStubs: false,
    currentSelectedFormHelper: "",
  }),
  getters: {
    getInput: (state) => state.input,
    getInputLength: (state) => state.input.length,
    getCurrentSelectedFormHelper: (state) => state.currentSelectedFormHelper,
    getResponseBodyType(state) {
      return handle(() => {
        const parsed = parseInput(state.input);
        if (parsed) {
          if (!parsed.response) {
            return responseBodyTypes.text;
          }

          const res = parsed.response;
          if (res.text) {
            return responseBodyTypes.text;
          } else if (res.json) {
            return responseBodyTypes.json;
          } else if (res.xml) {
            return responseBodyTypes.xml;
          } else if (res.html) {
            return responseBodyTypes.html;
          } else if (res.base64) {
            return responseBodyTypes.base64;
          }
        }

        return responseBodyTypes.text;
      });
    },
    getResponseBody(state) {
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
    getDynamicMode(state) {
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
    getStubId(state) {
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
    openFormHelper(key) {
      this.currentSelectedFormHelper = key;
    },
    closeFormHelper() {
      this.currentSelectedFormHelper = "";
    },
    setInput(input) {
      this.input = input;
      this.inputHasMultipleStubs = /^-/gm.test(input);
    },
    setDefaultDescription() {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.description = defaultValues.description;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setDefaultPriority() {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.priority = defaultValues.priority;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setDefaultPath() {
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
    setDefaultFullPath() {
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
    setDefaultQuery() {
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
    setDefaultIsHttps() {
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
    setDefaultBasicAuth() {
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
    setDefaultRequestHeaders() {
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
    setDefaultRequestBody() {
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
    setDefaultFormBody() {
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
    setDefaultClientIp() {
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
    setDefaultHostname() {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.conditions) {
            parsed.conditions = {};
          }

          parsed.conditions.hostname = defaultValues.hostname;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setDefaultJsonPath() {
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
    setDefaultJsonObject() {
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
    setDefaultJsonArray() {
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
    setDefaultXPath() {
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
    setMethod(method) {
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
    setTenant(tenant) {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.tenant = tenant;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setScenario(scenario) {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.scenario = scenario;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setStatusCode(code) {
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
    setResponseBody(payload) {
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

          const responseType = payload.type || responseBodyTypes.text;
          const responseBody = payload.body || "";
          switch (responseType) {
            case responseBodyTypes.json:
              parsed.response.json = responseBody;
              break;
            case responseBodyTypes.xml:
              parsed.response.xml = responseBody;
              break;
            case responseBodyTypes.html:
              parsed.response.html = responseBody;
              break;
            case responseBodyTypes.base64:
              parsed.response.base64 = responseBody;
              break;
            default:
            case responseBodyTypes.text:
              parsed.response.text = responseBody;
              break;
          }

          this.input = yaml.dump(parsed);
        }
      });
    },
    setResponseContentType(contentType) {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          if (!parsed.response) {
            parsed.response = {};
          }

          if (parsed.response.headers) {
            const key = Object.keys(parsed.response.headers).find(
              (k) => k.toLowerCase().trim() === "content-type"
            );
            delete parsed.response.headers[key];
          }

          parsed.response.contentType = contentType;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setDefaultResponseHeaders() {
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
    setDefaultExtraDuration() {
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
    setDefaultTempRedirect() {
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
    setDefaultPermanentRedirect() {
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
    setLineEndings(lineEndings) {
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
    setDynamicMode(value) {
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
    setDefaultReverseProxy() {
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
    setStubDisabled() {
      handle(() => {
        const parsed = parseInput(this.input);
        if (parsed) {
          parsed.enabled = false;
          this.input = yaml.dump(parsed);
        }
      });
    },
    setDefaultResponseContentType() {
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
    setDefaultImage() {
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
    setDefaultMinHits() {
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
    setDefaultMaxHits() {
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
    setDefaultExactHits() {
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
    setDefaultScenarioState() {
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
    setClearState() {
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
    setDefaultNewScenarioState() {
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
