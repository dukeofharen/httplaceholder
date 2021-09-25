import { get } from "@/utils/api";

const state = () => ({});

const actions = {
  getStubsOverview() {
    return get("/ph-api/stubs/overview")
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  getStub(_, stubId) {
    return get(`/ph-api/stubs/${stubId}`)
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
