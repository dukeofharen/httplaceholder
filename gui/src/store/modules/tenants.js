import createInstance from "@/axios/axiosInstanceFactory";

const state = () => ({});

const actions = {
  getTenantNames() {
    return new Promise((resolve, reject) =>
      createInstance()
        .get("ph-api/tenants")
        .then(response => resolve(response.data))
        .catch(error => reject(error))
    );
  }
};

const mutations = {};

const getters = {};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions
};
