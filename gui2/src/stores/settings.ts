import { defineStore } from 'pinia'
import { getSettings, setSettings } from '@/utils/session'
import { browserUsesDarkTheme } from '@/utils/theme'
import type { SettingsModel } from '@/domain/settings-model'
import { defaultLanguage, requestsPerPage } from '@/constants'
import { computed, ref } from 'vue'

const savedSettings = getSettings()
export const useSettingsStore = defineStore('settings', () => {
  // State
  const settings = ref<SettingsModel>({
    darkTheme:
      savedSettings?.darkTheme !== undefined ? savedSettings.darkTheme : browserUsesDarkTheme(),
    saveSearchFilters:
      savedSettings?.saveSearchFilters !== undefined ? savedSettings.saveSearchFilters : true,
    requestPageSize:
      savedSettings?.requestPageSize !== undefined
        ? savedSettings.requestPageSize
        : requestsPerPage,
    language: savedSettings?.language !== undefined ? savedSettings.language : defaultLanguage,
  })

  // Getters
  const getSettings = computed(() => settings.value)
  const getDarkTheme = computed(() => settings.value.darkTheme)
  const getSaveSearchFilters = computed(() => settings.value.saveSearchFilters)
  const getRequestsPageSize = computed(() => settings.value.requestPageSize)
  const getLanguage = computed(() => settings.value.language)

  // Actions
  function storeSettings(settingsValue: SettingsModel) {
    settings.value = settingsValue
    setSettings(settingsValue)
  }

  return {
    getSettings,
    getDarkTheme,
    getSaveSearchFilters,
    getRequestsPageSize,
    getLanguage,
    storeSettings,
  }
})
