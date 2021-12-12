import { get, put } from "@/utils/api";

const state = () => ({});

const actions = {
  getAllScenarios() {
    return get("/ph-api/scenarios")
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  getScenario(_, scenario) {
    return get(`/ph-api/scenarios/${scenario}`)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  setScenario(_, scenario) {
    return put(`/ph-api/scenarios/${scenario.scenario}`, scenario)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
};

const mutations = {};

const getters = {};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions,
};
