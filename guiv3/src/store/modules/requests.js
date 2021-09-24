import { del, get } from "@/utils/api";

const state = () => ({});

const actions = {
  getRequestsOverview() {
    return get("/ph-api/requests/overview")
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  getRequest(_, correlationId) {
    return get(`/ph-api/requests/${correlationId}`)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  clearRequests() {
    return del("/ph-api/requests")
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
