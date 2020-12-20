import createInstance from "@/axios/axiosInstanceFactory";

const state = () => ({});

const actions = {
  getRequestsOverview() {
    return new Promise((resolve, reject) =>
      createInstance()
        .get("ph-api/requests/overview")
        .then(response => resolve(response.data))
        .catch(error => reject(error))
    );
  },
  getRequest({}, correlationId) {
    return new Promise((resolve, reject) =>
      createInstance()
        .get(`ph-api/requests/${correlationId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error))
    );
  },
  clearRequests() {
    return new Promise((resolve, reject) =>
      createInstance()
        .delete("ph-api/requests")
        .then(() => resolve())
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
