import { authenticateResults } from "@/constants";

export default {
    storeMetadata: (state, metadata) => {
        state.metadata = metadata
    },
    storeAuthenticated: (state, authenticated) => {
        state.authenticated = authenticated
        if (!authenticated) {
            state.userToken = null
            state.lastAuthenticateResult = authenticateResults.NOT_SET
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
    addAdditionalRequest: (state, request) => {
        state.requests.unshift(request);
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
    },
    storeTenantNames: (state, tenantNames) => {
        state.tenantNames = tenantNames
    }
};