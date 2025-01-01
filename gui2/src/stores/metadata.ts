import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import type { MetadataModel } from '@/domain/metadata/metadata-model'
import { FeatureFlagType } from '@/domain/metadata/feature-flag-type'
import type { FeatureResultModel } from '@/domain/metadata/feature-result-model'
import { get } from '@/utils/api'

// type MetadataState = {
//   metadata: MetadataModel;
//   authenticationEnabled: boolean;
// };

export const useMetadataStore = defineStore('metadata', () => {
  // State
  const metadata = ref<MetadataModel>({
    version: '',
    variableHandlers: [],
    runtimeVersion: '',
  })
  const authenticationEnabled = ref(false)

  // Getters
  const getAuthenticationEnabled = computed(() => authenticationEnabled.value)
  const getMetadataState = computed(() => metadata.value)

  // Actions
  function checkFeatureIsEnabled(feature: FeatureFlagType): Promise<FeatureResultModel> {
    return get(`/ph-api/metadata/features/${feature}`)
  }

  async function checkAuthenticationIsEnabled(): Promise<boolean> {
    const authEnabled = (await checkFeatureIsEnabled(FeatureFlagType.Authentication)).enabled
    authenticationEnabled.value = authEnabled
    return authEnabled
  }

  async function getMetadata(): Promise<MetadataModel> {
    try {
      const response = await get('/ph-api/metadata')
      metadata.value = response
      return Promise.resolve(response)
    } catch (error) {
      return await Promise.reject(error)
    }
  }

  return {
    getAuthenticationEnabled,
    getMetadataState,
    checkFeatureIsEnabled,
    checkAuthenticationIsEnabled,
    getMetadata,
  }
})
