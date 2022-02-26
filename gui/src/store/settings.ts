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
export const useSettingsStore = defineStore({
  id: "settings",
  state: () =>
    ({
      settings: <SettingsModel>{
        darkTheme: savedSettings.darkTheme,
        saveSearchFilters: savedSettings.saveSearchFilters,
      },
    } as GeneralState),
  getters: {
    getSettings: (state): SettingsModel => state.settings,
    getDarkTheme: (state): boolean => state.settings.darkTheme,
    getSaveSearchFilters: (state): boolean => state.settings.saveSearchFilters,
  },
  actions: {
    storeSettings(settings: SettingsModel): void {
      this.settings = settings;
      setSettings(settings);
    },
  },
});
