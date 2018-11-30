import axios from 'axios'
import urls from 'urls'
import { authenticateResults, messageTypes } from '@/constants';
import resources from '@/resources';

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
        const mutation = 'storeAuthenticated';
        let username = "testUser";
        let password = "testPassword";
        getUser(username, password)
            .then(response => commit(mutation, true))
            .catch(error => {
                if (error.response.status === 401) {
                    commit(mutation, false)
                }
            })
    },
    authenticate({ commit }, payload) {
        const mutation = 'storeLastAuthenticateResult'
        const userTokenMutation = 'storeUserToken'
        const storeToastMutation = 'storeToast'
        let token = basicAuth(payload.username, payload.password)
        getUser(payload.username, payload.password)
            .then(response => {
                commit(mutation, authenticateResults.OK)
                commit(userTokenMutation, token)
            })
            .catch(error => {
                if (error.response.status === 401) {
                    commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.credentialsIncorrect })
                    commit(mutation, authenticateResults.INVALID_CREDENTIALS)
                } else {
                    commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.somethingWentWrongServer })
                    commit(mutation, authenticateResults.INTERNAL_SERVER_ERROR)
                }
            })
    },
    getRequests({ commit, state }) {
        const storeToastMutation = 'storeToast'
        const mutation = 'storeRequests'
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/requests`
        let token = state.userToken
        let config = getConfig(token)
        axios
            .get(url, config)
            .then(response => {
                commit(mutation, response.data)
            })
            .catch(error => {
                commit(storeToastMutation, { type: messageTypes.ERROR, message: resources.somethingWentWrongServer })
            });
    },
    clearRequests({ commit, state }) {
        const storeToastMutation = 'storeToast'
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
    }
}