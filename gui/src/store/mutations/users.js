export function storeAuthenticated(state, authenticated) {
    state.authenticated = authenticated;
    if (!authenticated) {
        state.userToken = null;
    }
}

export function storeAuthenticationRequired(state, authenticationRequired) {
    state.authenticationRequired = authenticationRequired;
}

export function storeUserToken(state, token) {
    state.userToken = token;
}