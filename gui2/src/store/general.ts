import { defineStore } from "pinia";
import { getSettings, setSettings } from "@/utils/session";
import type { SettingsModel } from "@/domain/settings-model";

type GeneralState = {
  settings: SettingsModel;
};

const savedSettings: SettingsModel = getSettings() || {
  darkTheme: false,
  saveSearchFilters: true,
};
export const useGeneralStore = defineStore({
  id: "general",
  state: () =>
    ({
      settings: <SettingsModel>{
        darkTheme: savedSettings.darkTheme,
        saveSearchFilters: savedSettings.saveSearchFilters,
      },
    } as GeneralState),
  getters: {
    getSettings: (state) => state.settings,
    getDarkTheme: (state) => state.settings.darkTheme,
    getSaveSearchFilters: (state) => state.settings.saveSearchFilters,
  },
  actions: {
    storeSettings(settings: SettingsModel) {
      this.settings = settings;
      setSettings(settings);
    },
  },
});
