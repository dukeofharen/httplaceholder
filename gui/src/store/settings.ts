import { defineStore } from "pinia";
import { getSettings, setSettings } from "@/utils/session";
import { browserUsesDarkTheme } from "@/utils/theme";
import type { SettingsModel } from "@/domain/settings-model";
import { requestsPerPage } from "@/constants/technical";

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
};
export const useSettingsStore = defineStore({
  id: "settings",
  state: () =>
    ({
      settings: <SettingsModel>{
        darkTheme: settings.darkTheme,
        saveSearchFilters: settings.saveSearchFilters,
        requestPageSize: settings.requestPageSize,
      },
    }) as GeneralState,
  getters: {
    getSettings: (state): SettingsModel => state.settings,
    getDarkTheme: (state): boolean => state.settings.darkTheme,
    getSaveSearchFilters: (state): boolean => state.settings.saveSearchFilters,
    getRequestsPageSize: (state): number => state.settings.requestPageSize,
  },
  actions: {
    storeSettings(settings: SettingsModel): void {
      this.settings = settings;
      setSettings(settings);
    },
  },
});
