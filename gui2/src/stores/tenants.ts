import { defineStore } from 'pinia'
import { get } from '@/utils/api'

export const useTenantsStore = defineStore('tenants', () => {
  // Actions
  async function getTenantNames(): Promise<string[]> {
    return get('/ph-api/tenants')
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  return { getTenantNames }
})
