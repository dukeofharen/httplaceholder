import { getSession, removeSession, setSession } from "@/utils/storageUtil";
import { sessionKeys } from "@/shared/constants";

export function getUserToken() {
  return getSession(sessionKeys.userToken);
}

export function clearUserToken() {
  removeSession(sessionKeys.userToken);
}

export function saveUserToken(token) {
  setSession(sessionKeys.userToken, token);
}

export function getDarkThemeEnabled() {
  return getSession(sessionKeys.darkTheme);
}

export function setDarkThemeEnabled(enabled) {
  setSession(sessionKeys.darkTheme, enabled);
}

export function getIntermediateStub() {
  return getSession(sessionKeys.intermediateStub);
}

export function setIntermediateStub(stub) {
  setSession(sessionKeys.intermediateStub, stub);
}

export function clearIntermediateStub() {
  removeSession(sessionKeys.intermediateStub);
}
