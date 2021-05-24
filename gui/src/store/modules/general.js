import { setDarkThemeEnabled } from "@/utils/sessionUtil";

const state = () => ({
  settings: {
    darkTheme: false
  },
  drawer: true
});

const actions = {};

const mutations = {
  storeDarkTheme(state, darkTheme) {
    state.settings.darkTheme = darkTheme;
    setDarkThemeEnabled(darkTheme);
  },
  flipDrawerIsOpen(state) {
    state.drawer = !state.drawer;
  }
};

const getters = {
  getDarkTheme(state) {
    return state.settings.darkTheme;
  },
  getDrawerIsOpen(state) {
    return state.drawer;
  }
};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions
};
