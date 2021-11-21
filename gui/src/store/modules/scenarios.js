import { get } from "@/utils/api";

const state = () => ({});

const actions = {
  getAllScenarios() {
    return get("/ph-api/scenarios")
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
