import { get } from "@/utils/api";
import { FeatureFlagType } from "@/constants/featureFlagType";

const state = () => ({
  metadata: {
    version: "",
    variableHandlers: [],
  },
  authenticationEnabled: false,
});

const checkFeature = (feature) => get(`/ph-api/metadata/features/${feature}`);

const actions = {
  getMetadata(store) {
    return get("/ph-api/metadata")
      .then((response) => {
        store.commit("storeMetadata", response);
        return Promise.resolve(response);
      })
      .catch((error) => Promise.reject(error));
  },
  async checkAuthenticationIsEnabled(store) {
    const authEnabled = (await checkFeature(FeatureFlagType.Authentication))
      .enabled;
    store.commit("storeAuthenticationEnabled", authEnabled);
    return authEnabled;
  },
};

const mutations = {
  storeMetadata(state, metadata) {
    state.metadata = metadata;
  },
  storeAuthenticationEnabled(state, enabled) {
    state.authenticationEnabled = enabled;
  },
};

const getters = {
  authenticationEnabled(state) {
    return state.authenticationEnabled;
  },
};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions,
};
