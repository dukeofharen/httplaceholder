import { defineStore } from 'pinia'
import { del, get } from '@/utils/api'
import type { RequestOverviewModel } from '@/domain/request/request-overview-model.ts'
import type { RequestResultModel } from '@/domain/request/request-result-model.ts'
import type { ResponseModel } from '@/domain/request/response-model.ts'

export const useRequestsStore = defineStore('requests', () => {
  // Actions
  async function getRequestsOverview(
    fromIdentifier?: string,
    itemsPerPage?: number,
  ): Promise<RequestOverviewModel[]> {
    const headers: any = {}
    if (itemsPerPage && Math.max(itemsPerPage, 0)) {
      headers['x-items-per-page'] = itemsPerPage + 1
    }
    if (fromIdentifier) {
      headers['x-from-identifier'] = fromIdentifier
    }
    return get('/ph-api/requests/overview', {
      headers,
    })
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  async function getRequest(correlationId: string): Promise<RequestResultModel> {
    return get(`/ph-api/requests/${correlationId}`)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  async function getResponse(correlationId: string): Promise<ResponseModel> {
    return get(`/ph-api/requests/${correlationId}/response`)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  async function clearRequests(): Promise<any> {
    return del('/ph-api/requests')
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  async function deleteRequest(correlationId: string): Promise<any> {
    return del(`/ph-api/requests/${correlationId}`)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  return { getRequestsOverview, getRequest, getResponse, clearRequests, deleteRequest }
})
