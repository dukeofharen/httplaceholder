import axios from 'axios'
import yaml from 'js-yaml'
import urls from 'urls'
import { authenticateResults, messageTypes } from '@/constants';
import resources from '@/resources';

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
                commit(storeAuthMutation, true)
                commit(storeAuthRequired, false)
            })
            .catch(error => {
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
                commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.somethingWentWrongServer })
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
                commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.somethingWentWrongServer })
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
                commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.somethingWentWrongServer })
            });
    },
    getStubsAsYaml({ commit, state }) {
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/stubs`
        let token = state.userToken
        let config = getConfig(token, true)
        axios.get(url, config)
            .then(response => {
                commit('storeStubsDownloadString', response.data)
            })
            .catch(error => {
                commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.somethingWentWrongServer })
            });
    },
    getStubAsYaml({ commit, state }, payload) {
        let stubId = payload.stubId
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/stubs/${stubId}`
        let token = state.userToken
        let config = getConfig(token, true)
        axios.get(url, config)
            .then(response => {
                commit('storeLastSelectedStub', {
                    id: stubId,
                    yaml: response.data
                })
            })
            .catch(error => {
                commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.somethingWentWrongServer })
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
                commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.somethingWentWrongServer })
            });
    },
    addStubs({ commit, state }, payload) {
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/stubs`
        let token = state.userToken
        let config = getConfig(token)
        config.headers["Content-Type"] = 'application/json'

        let stubsArray;
        let parsedObject = yaml.safeLoad(payload.input);
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
                    if(payload.updated){
                        message = resources.stubUpdatedSuccessfully
                    }

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
    }
}