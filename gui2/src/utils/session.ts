import {
  getLocal,
  getSession,
  removeSession,
  setLocal,
  setSession,
} from "@/utils/storage";
import { sessionKeys } from "@/constants/sessionKeys";
import type { SettingsModel } from "@/domain/settings-model";

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

// TODO make this NOT an any
export function getUserToken(): any {
  return getSession(sessionKeys.userToken);
}

export function clearUserToken() {
  removeSession(sessionKeys.userToken);
}

// TODO make this NOT an any
export function saveUserToken(token: any) {
  setSession(sessionKeys.userToken, token);
}

// TODO make this NOT an any
export function setStubFilterForm(filter: any) {
  setSession(sessionKeys.stubsFilter, filter);
}

export function getStubFilterForm(): any {
  return getSession(sessionKeys.stubsFilter);
}

// TODO make this NOT an any
export function setRequestFilterForm(filter: any) {
  setSession(sessionKeys.requestsFilter, filter);
}

export function getRequestFilterForm(): any {
  return getSession(sessionKeys.requestsFilter);
}
