export default {
    storeMetadata: (state, metadata) => {
        state.metadata = metadata
    },
    storeAuthenticated: (state, authenticated) => {
        state.authenticated = authenticated
        if(!authenticated) {
            state.userToken = null
        }
    },
    storeAuthenticationRequired: (state, authenticationRequired) => {
        state.authenticationRequired = authenticationRequired
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
    storeStubs: (state, stubs) => {
        state.stubs = stubs
    },
    storeToast: (state, toast) => {
        toast.timestamp = new Date().getTime()
        state.toast = toast
    },
    storeStubsDownloadString: (state, download) => {
        state.stubsDownloadString = download
    },
    storeLastSelectedStub: (state, stub) => {
        state.lastSelectedStub = stub
    },
    storeTheme: (state, theme) => {
        state.settings.theme = theme
    }
};