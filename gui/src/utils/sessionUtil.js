import {getSession, removeSession, setSession} from "@/utils/storageUtil";
import {sessionKeys} from "@/shared/constants";

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