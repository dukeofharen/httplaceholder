import { defineStore } from 'pinia'
import { del, get, post, put } from '@/utils/api'
import type { FullStubModel } from '@/domain/stub/full-stub-model'
import type { FullStubOverviewModel } from '@/domain/stub/full-stub-overview-model.ts'
import yaml from 'js-yaml'
import { useStubFormStore } from '@/stores/stubForm.ts'

export interface UpdateStubInputModel {
  input: string
  stubId: string
}

export interface CreateStubBasedOnRequestInputModel {
  correlationId: string
  doNotCreateStub?: boolean
}

const stubFormStore = useStubFormStore()
export const useStubsStore = defineStore('stubs', () => {
  // Actions
  async function getStub(stubId: string): Promise<FullStubModel> {
    return get(`/ph-api/stubs/${stubId}`)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  async function getStubsOverview(): Promise<FullStubOverviewModel[]> {
    return get('/ph-api/stubs/overview')
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  async function getStubs(): Promise<FullStubModel[]> {
    return get('/ph-api/stubs')
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  async function flipEnabled(stubId: string): Promise<boolean> {
    const stub = (await getStub(stubId)).stub
    stub.enabled = !stub.enabled
    await put(`/ph-api/stubs/${stubId}`, stub)
    return stub.enabled
  }

  async function enableStub(stubId: string): Promise<boolean> {
    const stub = (await getStub(stubId)).stub
    stub.enabled = true
    await put(`/ph-api/stubs/${stubId}`, stub)
    return stub.enabled
  }

  async function disableStub(stubId: string): Promise<boolean> {
    const stub = (await getStub(stubId)).stub
    stub.enabled = false
    await put(`/ph-api/stubs/${stubId}`, stub)
    return stub.enabled
  }

  async function deleteStub(stubId: string): Promise<any> {
    return del(`/ph-api/stubs/${stubId}`)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  async function deleteStubs(): Promise<any> {
    return del('/ph-api/stubs')
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  async function addStubs(input: string): Promise<FullStubModel[]> {
    const parsedObject = yaml.load(input)
    const stubsArray = Array.isArray(parsedObject) ? parsedObject : [parsedObject]
    return post('/ph-api/stubs/multiple', stubsArray)
      .then((response) => {
        stubFormStore.setFormIsDirty(false)
        return Promise.resolve(response)
      })
      .catch((error) => Promise.reject(error))
  }

  async function updateStub(payload: UpdateStubInputModel): Promise<any> {
    const parsedObject = yaml.load(payload.input)
    return put(`/ph-api/stubs/${payload.stubId}`, parsedObject)
      .then((response) => {
        stubFormStore.setFormIsDirty(false)
        return Promise.resolve(response)
      })
      .catch((error) => Promise.reject(error))
  }

  async function createStubBasedOnRequest(
    payload: CreateStubBasedOnRequestInputModel,
  ): Promise<FullStubModel> {
    return post(`/ph-api/requests/${payload.correlationId}/stubs`, {
      doNotCreateStub: payload.doNotCreateStub !== null ? payload.doNotCreateStub : false,
    })
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error))
  }

  return {
    getStubsOverview,
    getStubs,
    flipEnabled,
    enableStub,
    disableStub,
    deleteStub,
    deleteStubs,
    addStubs,
    updateStub,
    createStubBasedOnRequest,
  }
})
