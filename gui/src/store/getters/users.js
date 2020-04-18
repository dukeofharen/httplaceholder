export function getAuthenticated(state) {
    return !!state.userToken
}

export function getUserToken(state) {
    return state.userToken
}