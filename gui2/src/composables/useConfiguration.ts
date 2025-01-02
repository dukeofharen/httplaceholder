import { useConfigurationStore } from '@/stores/configuration.ts'
import { computed, onMounted, ref } from 'vue'
import type { ConfigurationModel } from '@/domain/stub/configuration-model.ts'

/**
 * A simple composable for retrieving the HttPlaceholder configuration and querying it.
 */
export function useConfiguration() {
  const configStore = useConfigurationStore()

  // Data
  const configuration = ref<ConfigurationModel[]>([])

  // Getters
  const getOldRequestsQueueLength = computed(() => {
    const foundOldRequestsQueueLength = configuration.value.find(
      (c) => c.key === 'oldRequestsQueueLength',
    )
    return foundOldRequestsQueueLength ? parseInt(foundOldRequestsQueueLength.value) : 0
  })
  const getConfiguration = computed(() => configuration)

  onMounted(async () => (configuration.value = await configStore.getConfiguration()))

  return { getOldRequestsQueueLength, getConfiguration }
}
