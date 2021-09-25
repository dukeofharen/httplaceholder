import { get, put } from "@/utils/api";

const state = () => ({});

const getStub = (stubId) => {
  return get(`/ph-api/stubs/${stubId}`)
    .then((response) => Promise.resolve(response))
    .catch((error) => Promise.reject(error));
};

const actions = {
  getStubsOverview() {
    return get("/ph-api/stubs/overview")
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  getStub(_, stubId) {
    return getStub(stubId);
  },
  async flipEnabled(_, stubId) {
    const stub = (await getStub(stubId)).stub;
    stub.enabled = !stub.enabled;
    await put(`/ph-api/stubs/${stubId}`, stub);
    return stub.enabled;
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
