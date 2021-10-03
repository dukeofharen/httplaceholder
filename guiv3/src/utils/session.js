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

export function getDarkThemeEnabled() {
  return getLocal(sessionKeys.darkTheme);
}

export function setDarkThemeEnabled(enabled) {
  setLocal(sessionKeys.darkTheme, enabled);
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
