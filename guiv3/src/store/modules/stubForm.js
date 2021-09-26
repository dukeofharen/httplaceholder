const state = () => ({
  input: "",
});

const actions = {};

const mutations = {
  setInput(state, input) {
    state.input = input;
  },
};

const getters = {
  getInput(state) {
    return state.input;
  },
};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions,
};
