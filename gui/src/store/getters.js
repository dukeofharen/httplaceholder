export default {
    getMetadata: state => state.metadata,
    getAuthenticated: state => state.authenticated,
    getLastAuthenticateResult: state => state.lastAuthenticateResult,
    getUserToken: state => state.userToken,
    getRequests: state => state.requests,
    getStubs: state => state.stubs,
    getToast: state => state.toast,
    stubsDownloadString: state => state.stubsDownloadString
};