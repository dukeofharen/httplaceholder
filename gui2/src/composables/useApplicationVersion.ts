import { useMetadataStore } from '@/stores/metadata.ts'
import { onMounted } from 'vue'

/**
 * Simple composable for adding the application version to the document title.
 */
export function useApplicationVersion() {
  const metadataStore = useMetadataStore()

  // Lifecycle
  onMounted(() => {
    metadataStore.getMetadata().then((metadata) => {
      document.title = `HttPlaceholder - v${metadata.version}`
    })
  })
}
