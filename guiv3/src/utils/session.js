import {
  getLocal,
  getSession,
  removeSession,
  setLocal,
  setSession,
} from "@/utils/storage";
import { sessionKeys } from "@/constants/sessionKeys";

export function getIntermediateStub() {
  return getSession(sessionKeys.intermediateStub);
}

export function setIntermediateStub(stub) {
  setSession(sessionKeys.intermediateStub, stub);
}

export function clearIntermediateStub() {
  removeSession(sessionKeys.intermediateStub);
}

export function getSettings() {
  return getLocal(sessionKeys.settings);
}

export function setSettings(settings) {
  setLocal(sessionKeys.settings, settings);
}

export function getUserToken() {
  return getSession(sessionKeys.userToken);
}

export function clearUserToken() {
  removeSession(sessionKeys.userToken);
}

export function saveUserToken(token) {
  setSession(sessionKeys.userToken, token);
}

export function setStubFilterForm(filter) {
  setSession(sessionKeys.stubsFilter, filter);
}

export function getStubFilterForm() {
  return getSession(sessionKeys.stubsFilter);
}
