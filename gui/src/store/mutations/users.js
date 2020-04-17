import {authenticateResults} from "@/shared/constants";

export function storeAuthenticated(state, authenticated) {
    state.authenticated = authenticated;
    if (!authenticated) {
        state.userToken = null;
        state.lastAuthenticateResult = authenticateResults.NOT_SET;
    }
}

export function storeAuthenticationRequired(state, authenticationRequired) {
    state.authenticationRequired = authenticationRequired;
}

export function storeLastAuthenticateResult(state, result) {
    state.lastAuthenticateResult = result;
}

export function storeUserToken(state, token) {
    state.userToken = token;
}