import { post } from "@/utils/api";

const state = () => ({});

const actions = {
  importCurlCommands(_, input) {
    return post(
      `/ph-api/import/curl?doNotCreateStub=${input.doNotCreateStub}`,
      input.commands
    )
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  importHar(_, input) {
    return post(
      `/ph-api/import/har?doNotCreateStub=${input.doNotCreateStub}`,
      input.har
    )
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
