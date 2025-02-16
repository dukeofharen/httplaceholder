import { defineStore } from 'pinia'
import { get } from '@/utils/api'
import type { RequestExportType } from '@/domain/request/enums/request-export-type'
import type { RequestExportResultDto } from '@/domain/export/request-export-result-dto'

export const useExportStore = defineStore({
  id: 'export',
  state: () => ({}),
  getters: {},
  actions: {
    async exportRequest(
      correlationId: string,
      requestExportType: RequestExportType,
    ): Promise<RequestExportResultDto> {
      return get(`/ph-api/export/requests/${correlationId}?type=${requestExportType}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error))
    },
  },
})
