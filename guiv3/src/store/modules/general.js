import { setDarkThemeEnabled } from "@/utils/session";

const state = () => ({
  settings: {
    darkTheme: false,
  },
});

const actions = {};

const mutations = {
  storeDarkTheme(state, darkTheme) {
    state.settings.darkTheme = darkTheme;
    setDarkThemeEnabled(darkTheme);
  },
};

const getters = {
  getDarkTheme(state) {
    return state.settings.darkTheme;
  },
};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions,
};
