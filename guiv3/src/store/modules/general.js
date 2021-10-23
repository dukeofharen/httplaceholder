import { setSettings, getSettings } from "@/utils/session";

const savedSettings = getSettings();
const state = () => ({
  settings: {
    darkTheme: savedSettings?.darkTheme || false,
    saveSearchFilters: savedSettings?.saveSearchFilters || true,
  },
});

const actions = {};

const mutations = {
  storeSettings(state, settings) {
    state.settings = settings;
    setSettings(settings);
  },
};

const getters = {
  getSettings(state) {
    return state.settings;
  },
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
