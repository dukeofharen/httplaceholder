import {
  getLocal,
  getSession,
  removeSession,
  setLocal,
  setSession,
} from "@/utils/storage";
import type { SettingsModel } from "@/domain/settings-model";
import type { StubSavedFilterModel } from "@/domain/stub-saved-filter-model";
import type { RequestSavedFilterModel } from "@/domain/request-saved-filter-model";
import { SessionKey } from "@/constants/session-key";

export function getIntermediateStub(): any {
  return getSession(SessionKey.intermediateStub);
}

export function setIntermediateStub(stub: any) {
  setSession(SessionKey.intermediateStub, stub);
}

export function clearIntermediateStub() {
  removeSession(SessionKey.intermediateStub);
}

export function getSettings(): SettingsModel {
  return getLocal(SessionKey.settings) as SettingsModel;
}

export function setSettings(settings: SettingsModel) {
  setLocal(SessionKey.settings, settings);
}

export function getUserToken(): string {
  return getSession(SessionKey.userToken);
}

export function clearUserToken() {
  removeSession(SessionKey.userToken);
}

export function saveUserToken(token: string) {
  setSession(SessionKey.userToken, token);
}

export function setStubFilterForm(filter: StubSavedFilterModel) {
  setSession(SessionKey.stubsFilter, filter);
}

export function getStubFilterForm(): StubSavedFilterModel {
  return getSession(SessionKey.stubsFilter);
}

export function setRequestFilterForm(filter: RequestSavedFilterModel) {
  setSession(SessionKey.requestsFilter, filter);
}

export function getRequestFilterForm(): RequestSavedFilterModel {
  return getSession(SessionKey.requestsFilter);
}
