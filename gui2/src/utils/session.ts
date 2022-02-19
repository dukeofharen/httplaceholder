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

// TODO make this NOT an any
export function setStubFilterForm(filter: any) {
  setSession(sessionKeys.stubsFilter, filter);
}

export function getStubFilterForm(): any {
  return getSession(sessionKeys.stubsFilter);
}

export function setRequestFilterForm(filter: StubSavedFilterModel) {
  setSession(sessionKeys.requestsFilter, filter);
}

export function getRequestFilterForm(): StubSavedFilterModel {
  return getSession(sessionKeys.requestsFilter);
}
