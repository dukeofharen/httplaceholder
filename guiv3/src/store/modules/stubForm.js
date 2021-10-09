import yaml from "js-yaml";
import toastr from "toastr";
import { resources } from "@/constants/resources";
import {
  defaultValues,
  responseBodyTypes,
} from "@/constants/stubFormResources";

const parseInput = (state) => {
  try {
    return yaml.load(state.input);
  } catch (e) {
    toastr.error(resources.errorDuringParsingOfYaml.format(e));
    return null;
  }
};

const handle = (func) => {
  try {
    return func();
  } catch (e) {
    toastr.error(resources.errorDuringParsingOfYaml.format(e));
    return null;
  }
};

const state = () => ({
  input: "",
  currentSelectedFormHelper: "",
});

const actions = {};

const mutations = {
  openFormHelper(state, key) {
    state.currentSelectedFormHelper = key;
  },
  closeFormHelper(state) {
    state.currentSelectedFormHelper = "";
  },
  setInput(state, input) {
    state.input = input;
  },
  setDefaultDescription(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        parsed.description = defaultValues.description;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultPriority(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        parsed.priority = defaultValues.priority;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultPath(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.url) {
          parsed.conditions.url = {};
        }

        parsed.conditions.url.path = defaultValues.urlPath;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultFullPath(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.url) {
          parsed.conditions.url = {};
        }

        parsed.conditions.url.fullPath = defaultValues.fullPath;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultQuery(state) {
    handle(() => {
      const parsed = parseInput(state);
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
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultIsHttps(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        if (!parsed.conditions.url) {
          parsed.conditions.url = {};
        }

        parsed.conditions.url.isHttps = true;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultBasicAuth(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.basicAuthentication =
          defaultValues.basicAuthentication;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultRequestHeaders(state) {
    handle(() => {
      const parsed = parseInput(state);
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
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultRequestBody(state) {
    handle(() => {
      const parsed = parseInput(state);
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
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultFormBody(state) {
    handle(() => {
      const parsed = parseInput(state);
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
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultClientIp(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.clientIp = defaultValues.clientIp;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultHostname(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.hostname = defaultValues.hostname;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultJsonPath(state) {
    handle(() => {
      const parsed = parseInput(state);
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
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultJsonObject(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.json = defaultValues.jsonObject;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultJsonArray(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.json = defaultValues.jsonArray;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultXPath(state) {
    handle(() => {
      const parsed = parseInput(state);
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
        state.input = yaml.dump(parsed);
      }
    });
  },
  setMethod(state, method) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.conditions) {
          parsed.conditions = {};
        }

        parsed.conditions.method = method;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setTenant(state, tenant) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        parsed.tenant = tenant;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setStatusCode(state, code) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.statusCode = code;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setResponseBody(state, payload) {
    handle(() => {
      const parsed = parseInput(state);
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

        state.input = yaml.dump(parsed);
      }
    });
  },
  setResponseContentType(state, contentType) {
    handle(() => {
      const parsed = parseInput(state);
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
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultResponseHeaders(state) {
    handle(() => {
      const parsed = parseInput(state);
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
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultExtraDuration(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.extraDuration = defaultValues.extraDuration;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultTempRedirect(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.temporaryRedirect = defaultValues.redirect;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultPermanentRedirect(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.permanentRedirect = defaultValues.redirect;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setLineEndings(state, lineEndings) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.response) {
          parsed.response = {};
        }

        if (!lineEndings) {
          delete parsed.response.lineEndings;
        } else {
          parsed.response.lineEndings = lineEndings;
        }

        state.input = yaml.dump(parsed);
      }
    });
  },
  setDynamicMode(state, value) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.response) {
          parsed.response = {};
        }

        if (value) {
          parsed.response.enableDynamicMode = value;
        } else {
          delete parsed.response.enableDynamicMode;
        }

        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultReverseProxy(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.reverseProxy = defaultValues.reverseProxy;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setStubDisabled(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        parsed.enabled = false;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultResponseContentType(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.contentType = defaultValues.responseContentType;
        state.input = yaml.dump(parsed);
      }
    });
  },
  setDefaultImage(state) {
    handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.response) {
          parsed.response = {};
        }

        parsed.response.image = defaultValues.image;
        state.input = yaml.dump(parsed);
      }
    });
  },
};

const getters = {
  getInput(state) {
    return state.input;
  },
  getCurrentSelectedFormHelper(state) {
    return state.currentSelectedFormHelper;
  },
  getResponseBodyType(state) {
    return handle(() => {
      const parsed = parseInput(state);
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
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.response) {
          return "";
        }

        const res = parsed.response;
        return res.text || res.json || res.xml || res.html || res.base64 || "";
      }

      return "";
    });
  },
  getDynamicMode(state) {
    return handle(() => {
      const parsed = parseInput(state);
      if (parsed) {
        if (!parsed.response) {
          return false;
        }

        return parsed.response.enableDynamicMode || false;
      }

      return false;
    });
  },
};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions,
};
