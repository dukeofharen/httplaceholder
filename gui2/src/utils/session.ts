import {
  getLocal,
  getSession,
  removeSession,
  setLocal,
  setSession,
} from "@/utils/storage";
import { sessionKeys } from "@/constants/sessionKeys";
import type { SettingsModel } from "@/domain/settings-model";
import type { StubSavedFilterModel } from "@/domain/stub-saved-filter-model";
import type { RequestSavedFilterModel } from "@/domain/request-saved-filter-model";

export function getIntermediateStub(): any {
  return getSession(sessionKeys.intermediateStub);
}

export function setIntermediateStub(stub: any) {
  setSession(sessionKeys.intermediateStub, stub);
}

export function clearIntermediateStub() {
  removeSession(sessionKeys.intermediateStub);
}

export function getSettings(): SettingsModel {
  return getLocal(sessionKeys.settings) as SettingsModel;
}

export function setSettings(settings: SettingsModel) {
  setLocal(sessionKeys.settings, settings);
}

export function getUserToken(): string {
  return getSession(sessionKeys.userToken);
}

export function clearUserToken() {
  removeSession(sessionKeys.userToken);
}

export function saveUserToken(token: string) {
  setSession(sessionKeys.userToken, token);
}

export function setStubFilterForm(filter: StubSavedFilterModel) {
  setSession(sessionKeys.stubsFilter, filter);
}

export function getStubFilterForm(): StubSavedFilterModel {
  return getSession(sessionKeys.stubsFilter);
}

export function setRequestFilterForm(filter: RequestSavedFilterModel) {
  setSession(sessionKeys.requestsFilter, filter);
}

export function getRequestFilterForm(): RequestSavedFilterModel {
  return getSession(sessionKeys.requestsFilter);
}
