import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useHttpStore = defineStore('http', () => {
  // State
  const numberOfCurrentHttpCalls = ref(0)
  const showLoader = ref(false)
  const showLoaderTimeout = ref(0)

  // Getters
  const isExecutingHttpCalls = computed(
    () => showLoader.value && numberOfCurrentHttpCalls.value > 0,
  )

  // Actions
  function increaseNumberOfCurrentHttpCalls() {
    numberOfCurrentHttpCalls.value++
    if (showLoaderTimeout.value) {
      clearTimeout(showLoaderTimeout.value)
    }

    if (!showLoader.value) {
      showLoaderTimeout.value = setTimeout(() => (showLoader.value = true), 200)
    }
  }

  function decreaseNumberOfCurrentHttpCalls() {
    if (numberOfCurrentHttpCalls.value !== 0) {
      numberOfCurrentHttpCalls.value--
    }

    if (numberOfCurrentHttpCalls.value <= 0) {
      showLoader.value = false
      if (showLoaderTimeout.value) {
        clearTimeout(showLoaderTimeout.value)
      }
    }
  }

  return {
    isExecutingHttpCalls,
    increaseNumberOfCurrentHttpCalls,
    decreaseNumberOfCurrentHttpCalls,
  }
})
