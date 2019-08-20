import axios from 'axios'
import yaml from 'js-yaml'
import urls from 'urls'
import { authenticateResults, messageTypes } from '@/constants';
import { resources } from '@/resources';

const storeToastMutation = 'storeToast'
const storeAuthMutation = 'storeAuthenticated'
const storeAuthRequired = 'storeAuthenticationRequired'
const storeLastAuthResultMutation = 'storeLastAuthenticateResult'
const userTokenMutation = 'storeUserToken'
const storeRequestsMutation = 'storeRequests'

const getConfig = (userToken, asYaml) => {
    if (!asYaml) {
        asYaml = false;
    }

    let headers = {
        Authorization: `Basic ${userToken}`
    };
    if (asYaml) {
        headers['Accept'] = 'text/yaml'
    }

    return {
        headers: headers
    }
}

const basicAuth = (username, password) => btoa(`${username}:${password}`)

const getUser = (username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/users/${username}`
    let token = basicAuth(username, password)
    let config = getConfig(token)
    return axios.get(url, config)
}

const handleHttpError = (commit, error) => {
    const status = error.response.status;
    if (status !== 401) {
        commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.somethingWentWrongServer })
    }
}

export default {
    refreshMetadata({ commit }) {
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/metadata`
        axios
            .get(url)
            .then(response => {
                commit('storeMetadata', response.data)
            })
    },
    ensureAuthenticated({ commit }) {
        let username = "testUser";
        let password = "testPassword";
        getUser(username, password)
            .then(response => {
                // No authentication on endpoint, so no login required.
                commit(storeAuthMutation, true)
                commit(storeAuthRequired, false)
            })
            .catch(error => {
                // Authentication required, so show login screen.
                if (error.response.status === 401) {
                    commit(storeAuthMutation, false)
                    commit(storeAuthRequired, true)
                }
            })
    },
    authenticate({ commit }, payload) {
        let token = basicAuth(payload.username, payload.password)
        getUser(payload.username, payload.password)
            .then(response => {
                commit(storeLastAuthResultMutation, authenticateResults.OK)
                commit(userTokenMutation, token)
                commit(storeAuthMutation, true)
            })
            .catch(error => {
                if (error.response.status === 401) {
                    commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.credentialsIncorrect })
                    commit(storeLastAuthResultMutation, authenticateResults.INVALID_CREDENTIALS)
                } else {
                    commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.somethingWentWrongServer })
                    commit(storeLastAuthResultMutation, authenticateResults.INTERNAL_SERVER_ERROR)
                }
            })
    },
    getRequests({ commit, state }) {
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/requests`
        let token = state.userToken
        let config = getConfig(token)
        axios
            .get(url, config)
            .then(response => {
                commit(storeRequestsMutation, response.data)
            })
            .catch(error => {
                handleHttpError(commit, error);
            });
    },
    clearRequests({ commit, state }) {
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/requests`
        let token = state.userToken
        let config = getConfig(token)
        axios.delete(url, config)
            .then(response => {
                commit(storeToastMutation, { type: messageTypes.SUCCESS, message: resources.requestsDeletedSuccessfully })
                commit('storeRequests', [])
            })
            .catch(error => {
                handleHttpError(commit, error);
            });
    },
    getStubs({ commit, state }) {
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/stubs`
        let token = state.userToken
        let config = getConfig(token)
        axios.get(url, config)
            .then(response => {
                commit('storeStubs', response.data)
            })
            .catch(error => {
                handleHttpError(commit, error);
            });
    },
    getStub({ commit, state }, payload) {
        let stubId = payload.stubId
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/stubs/${stubId}`
        let token = state.userToken
        let config = getConfig(token)
        axios.get(url, config)
            .then(response => {
                commit('storeLastSelectedStub', {
                    fullStub: response.data
                })
            })
            .catch(error => {
                handleHttpError(commit, error);
            });
    },
    deleteStub({ commit, state, dispatch }, payload) {
        let stubId = payload.stubId
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/stubs/${stubId}`
        let token = state.userToken
        let config = getConfig(token)
        axios.delete(url, config)
            .then(response => {
                commit(storeToastMutation, { type: messageTypes.SUCCESS, message: resources.stubDeletedSuccessfully.format(stubId) })
                dispatch('getStubs')
            })
            .catch(error => {
                handleHttpError(commit, error);
            });
    },
    addStubs({ commit, state }, payload) {
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/stubs`
        let token = state.userToken
        let config = getConfig(token)
        config.headers["Content-Type"] = 'application/json'

        let stubsArray;
        let parsedObject;
        try {
            parsedObject = yaml.safeLoad(payload.input);
        } catch (error) {
            commit(storeToastMutation, { type: messageTypes.ERROR, message: error.message });
            return;
        }

        if (!Array.isArray(parsedObject)) {
            stubsArray = [parsedObject];
        } else {
            stubsArray = parsedObject;
        }

        for (let index in stubsArray) {
            let stub = stubsArray[index];
            axios.post(url, stub, config)
                .then(response => {
                    let message = resources.stubAddedSuccessfully

                    commit(storeToastMutation, { type: messageTypes.SUCCESS, message: message.format(stub.id) })
                })
                .catch(error => {
                    if (error.response.status === 409) {
                        commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.stubAlreadyAdded.format(stub.id) })
                    } else {
                        commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.stubNotAdded.format(stub.id) })
                    }
                });
        }
    },
    updateStub({ commit, state }, payload) {
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/stubs/${payload.stubId}`
        let token = state.userToken
        let config = getConfig(token)
        config.headers["Content-Type"] = 'application/json'

        let stub;
        try {
            stub = yaml.safeLoad(payload.input);
        } catch (error) {
            commit(storeToastMutation, { type: messageTypes.ERROR, message: error.message });
            return;
        }

        axios.put(url, stub, config)
            .then(response => {
                let message = resources.stubUpdatedSuccessfully
                commit(storeToastMutation, { type: messageTypes.SUCCESS, message: message.format(stub.id) })
            })
            .catch(error => {
                if (error.response.status === 409) {
                    commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.stubAlreadyAdded.format(stub.id) })
                } else {
                    commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.stubNotAdded.format(stub.id) })
                }
            });
    },
    createStubBasedOnRequest({ commit, state }, payload) {
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/requests/${payload.correlationId}/stubs`
        let token = state.userToken
        let config = getConfig(token)
        config.headers["Content-Type"] = 'application/json'
        axios.post(url, '', config)
            .then(response => {
                let stub = response.data.stub
                let message = resources.stubAddedSuccessfully
                commit(storeToastMutation, { type: messageTypes.SUCCESS, message: message.format(stub.id) })
                commit('storeLastSelectedStub', {
                    fullStub: response.data
                })
            })
            .catch(error => commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.stubNotAddedGeneric }))
    }
}