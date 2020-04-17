export function getAuthenticated(state) {
    return state.authenticated
}

export function getAuthenticationRequired(state) {
    return state.authenticationRequired
}

export function getLastAuthenticateResult(state) {
    return state.lastAuthenticateResult
}

export function getUserToken(state) {
    return state.userToken
}