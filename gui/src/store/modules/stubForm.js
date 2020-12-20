import yaml from "js-yaml";
import {toastError} from "@/utils/toastUtil";
import {resources} from "@/shared/resources";
import {defaultValues} from "@/shared/stubFormResources";

const parseInput = state => {
  try {
    return yaml.load(state.input);
  } catch(e) {
    toastError(resources.errorDuringParsingOfYaml.format(e));
    return null;
  }
};

const state = () => ({
  input: ""
});

const actions = {};

const mutations = {
  setInput(state, input) {
    state.input = input;
  },
  setDefaultDescription(state) {
    const parsed = parseInput(state);
    if(parsed) {
      parsed.description = defaultValues.description;
      state.input = yaml.dump(parsed);
    }
  },
  setDefaultPriority(state) {
    const parsed = parseInput(state);
    if(parsed) {
      parsed.priority = defaultValues.priority;
      state.input = yaml.dump(parsed);
    }
  }
};

const getters = {
  getInput(state) {
    return state.input;
  }
};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions
};
