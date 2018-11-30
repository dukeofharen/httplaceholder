export default {
    getMetadata: state => state.metadata,
    getAuthenticated: state => state.authenticated,
    getLastAuthenticateResult: state => state.lastAuthenticateResult,
    getUserToken: state => state.userToken
};