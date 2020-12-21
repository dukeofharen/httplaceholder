import yaml from "js-yaml";
import {toastError} from "@/utils/toastUtil";
import {resources} from "@/shared/resources";
import {defaultValues} from "@/shared/stubFormResources";

const parseInput = state => {
  try {
    return yaml.load(state.input);
  } catch (e) {
    toastError(resources.errorDuringParsingOfYaml.format(e));
    return null;
  }
};

const handle = func => {
  try {
    func();
  } catch (e) {
    toastError(resources.errorDuringParsingOfYaml.format(e));
  }
};

const state = () => ({
  input: "",
  currentSelectedFormHelper: ""
});

const actions = {};

const mutations = {
  setInput(state, input) {
    state.input = input;
  },
  openFormHelper(state, key) {
    state.currentSelectedFormHelper = key;
  },
  closeFormHelper(state) {
    state.currentSelectedFormHelper = "";
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

        parsed.conditions.url.query = defaultValues.query;
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

        parsed.conditions.basicAuthentication = defaultValues.basicAuthentication;
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
  }
};

const getters = {
  getInput(state) {
    return state.input;
  },
  getCurrentSelectedFormHelper(state) {
    return state.currentSelectedFormHelper;
  }
};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions
};
