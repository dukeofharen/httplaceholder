export default {
    storeMetadata: (state, metadata) => {
        state.metadata = metadata
    },
    storeAuthenticated: (state, authenticated) => {
        state.authenticated = authenticated
    },
    storeLastAuthenticateResult: (state, result) => {
        state.lastAuthenticateResult = result
    },
    storeUserToken: (state, token) => {
        state.userToken = token
    },
    storeRequests: (state, requests) => {
        state.requests = requests
    },
    storeToast: (state, toast) => {
        toast.timestamp = new Date().getTime()
        state.toast = toast
    }
};