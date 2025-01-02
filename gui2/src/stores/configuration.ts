import { defineStore } from 'pinia'
import { get, patch } from '@/utils/api'
import type { ConfigurationModel } from '@/domain/stub/configuration-model'
import type { UpdateConfigurationValueInputModel } from '@/domain/stub/update-configuration-value-input-model'

export const useConfigurationStore = defineStore('configuration', () => {
  // Actions
  async function getConfiguration(): Promise<ConfigurationModel[]> {
    return get('/ph-api/configuration')
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  async function updateConfigurationValue(
    inputModel: UpdateConfigurationValueInputModel,
  ): Promise<any> {
    return patch('/ph-api/configuration', inputModel)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  return { getConfiguration, updateConfigurationValue }
})
