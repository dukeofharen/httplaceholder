import createInstance from "@/axios/axiosInstanceFactory";
import { FeatureFlagType } from "@/models/enums/featureFlagType";

const state = () => ({
  metadata: {
    version: "",
    variableHandlers: []
  },
  authenticationEnabled: false
});

const checkFeature = feature =>
  new Promise((resolve, reject) =>
    createInstance()
      .get(`ph-api/metadata/features/${feature}`)
      .then(response => {
        resolve(response.data);
      })
      .catch(error => reject(error))
  );

const actions = {
  getMetadata(store) {
    return new Promise((resolve, reject) =>
      createInstance()
        .get("ph-api/metadata")
        .then(response => {
          resolve(response.data);
          store.commit("storeMetadata", response.data);
        })
        .catch(error => reject(error))
    );
  },
  async featureIsEnabled(store, feature) {
    return (await checkFeature(feature)).enabled;
  },
  async checkAuthenticationIsEnabled(store) {
    store.commit(
      "storeAuthenticationEnabled",
      (await checkFeature(FeatureFlagType.Authentication)).enabled
    );
  }
};

const mutations = {
  storeMetadata(state, metadata) {
    state.metadata = metadata;
  },
  storeAuthenticationEnabled(state, enabled) {
    state.authenticationEnabled = enabled;
  }
};

const getters = {
  getAuthenticationEnabled(state) {
    return state.authenticationEnabled;
  }
};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions
};
