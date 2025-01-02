import { useMetadataStore } from '@/stores/metadata.ts'
import { onMounted } from 'vue'
import { useUsersStore } from '@/stores/users.ts'
import { useRouter } from 'vue-router'

/**
 * Simple composable for verifying if authentication is enabled and also redirects the user to the
 * login page if needed.
 */
export function useLoginGuard() {
  const router = useRouter()
  const metadataStore = useMetadataStore()
  const userStore = useUsersStore()

  // Lifecycle
  onMounted(async () => {
    const authEnabled = await metadataStore.checkAuthenticationIsEnabled()
    if (!userStore.getAuthenticated && authEnabled) {
      await router.push({ name: 'Login' })
    }
  })
}
