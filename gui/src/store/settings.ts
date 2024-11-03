import { defineStore } from "pinia";
import { getSettings, setSettings } from "@/utils/session";
import { browserUsesDarkTheme } from "@/utils/theme";
import type { SettingsModel } from "@/domain/settings-model";
import { defaultLanguage, requestsPerPage } from "@/constants";

type GeneralState = {
  settings: SettingsModel;
};

const savedSettings = getSettings();
const settings: SettingsModel = {
  darkTheme:
    savedSettings?.darkTheme !== undefined
      ? savedSettings.darkTheme
      : browserUsesDarkTheme(),
  saveSearchFilters:
    savedSettings?.saveSearchFilters !== undefined
      ? savedSettings.saveSearchFilters
      : true,
  requestPageSize:
    savedSettings?.requestPageSize !== undefined
      ? savedSettings.requestPageSize
      : requestsPerPage,
  language:
    savedSettings?.language !== undefined
      ? savedSettings.language
      : defaultLanguage,
};
export const useSettingsStore = defineStore({
  id: "settings",
  state: () =>
    ({
      settings: <SettingsModel>{
        darkTheme: settings.darkTheme,
        saveSearchFilters: settings.saveSearchFilters,
        requestPageSize: settings.requestPageSize,
        language: settings.language,
      },
    }) as GeneralState,
  getters: {
    getSettings: (state): SettingsModel => state.settings,
    getDarkTheme: (state): boolean => state.settings.darkTheme,
    getSaveSearchFilters: (state): boolean => state.settings.saveSearchFilters,
    getRequestsPageSize: (state): number => state.settings.requestPageSize,
    getLanguage: (state): string => state.settings.language,
  },
  actions: {
    storeSettings(settings: SettingsModel): void {
      this.settings = settings;
      setSettings(settings);
    },
  },
});
