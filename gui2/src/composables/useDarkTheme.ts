import { useSettingsStore } from '@/stores/settings.ts'
import { computed, watch } from 'vue'

/**
 * Composable for listening for dark / light theme and adjust the CSS classes accordingly.
 */
export function useDarkTheme() {
  const settingsStore = useSettingsStore()

  // Functions
  function setDarkTheme(darkTheme: boolean) {
    const element = document.documentElement
    if (darkTheme) {
      element.classList.add('dark')
    } else {
      element.classList.remove('dark')
    }
  }

  // Computed
  const darkTheme = computed(() => settingsStore.getDarkTheme)

  // Watch
  watch(darkTheme, (darkTheme) => setDarkTheme(darkTheme))
}
