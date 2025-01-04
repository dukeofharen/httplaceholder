export enum SessionKey {
  intermediateStub = 'intermediateStub',
  settings = 'settings',
  userToken = 'userToken',
  stubsFilter = 'stubsFilter',
  requestsFilter = 'requestsFilter',
}

import { getLocal, getSession, removeSession, setLocal, setSession } from '@/utils/storage'
import type { SettingsModel } from '@/domain/ui/settings-model.ts'
import type { SavedFilterModel } from '@/domain/ui/saved-filter-model.ts'

export function getIntermediateStub(): string {
  return getSession(SessionKey.intermediateStub)
}

export function setIntermediateStub(stub: string) {
  setSession(SessionKey.intermediateStub, stub)
}

export function clearIntermediateStub() {
  removeSession(SessionKey.intermediateStub)
}

export function getSettings(): SettingsModel {
  return getLocal(SessionKey.settings) as SettingsModel
}

export function setSettings(settings: SettingsModel) {
  setLocal(SessionKey.settings, settings)
}

export function getUserToken(): string {
  return getSession(SessionKey.userToken)
}

export function clearUserToken() {
  removeSession(SessionKey.userToken)
}

export function saveUserToken(token: string) {
  setSession(SessionKey.userToken, token)
}

export function setStubFilterForm(filter: SavedFilterModel) {
  setSession(SessionKey.stubsFilter, filter)
}

export function getStubFilterForm(): SavedFilterModel {
  return getSession(SessionKey.stubsFilter)
}

export function setRequestFilterForm(filter: SavedFilterModel) {
  setSession(SessionKey.requestsFilter, filter)
}

export function getRequestFilterForm(): SavedFilterModel {
  return getSession(SessionKey.requestsFilter)
}
