import createInstance from "@/axios/axiosInstanceFactory";

const state = () => ({
  metadata: {
    version: "",
    variableHandlers: []
  }
});

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
  }
};

const mutations = {
  storeMetadata(state, metadata) {
    state.metadata = metadata;
  }
};

const getters = {};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions
};
