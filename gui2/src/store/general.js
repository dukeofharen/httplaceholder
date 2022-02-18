import { defineStore } from "pinia";
import { getSettings, setSettings } from "@/utils/session";

const savedSettings = getSettings() || {
  darkTheme: false,
  saveSearchFilters: true,
};
export const useGeneralStore = defineStore({
  id: "general",
  state: () => ({
    settings: {
      darkTheme: savedSettings.darkTheme,
      saveSearchFilters: savedSettings.saveSearchFilters,
    },
  }),
  getters: {
    getSettings: (state) => state.settings,
    getDarkTheme: (state) => state.settings.darkTheme,
    getSaveSearchFilters: (state) => state.settings.saveSearchFilters,
  },
  actions: {
    storeSettings(settings) {
      this.settings = settings;
      setSettings(settings);
    },
  },
});
