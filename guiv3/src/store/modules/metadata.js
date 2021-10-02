import { get } from "@/utils/api";

const state = () => ({
  metadata: {
    version: "",
    variableHandlers: [],
  },
  authenticationEnabled: false,
});

const actions = {
  getMetadata(store) {
    return get("/ph-api/metadata")
      .then((response) => {
        store.commit("storeMetadata", response);
        return Promise.resolve(response);
      })
      .catch((error) => Promise.reject(error));
  },
};

const mutations = {
  storeMetadata(state, metadata) {
    state.metadata = metadata;
  },
};

const getters = {};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions,
};
