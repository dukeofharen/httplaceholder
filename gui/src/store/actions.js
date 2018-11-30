import axios from 'axios'
import urls from 'urls'
import { authenticateResults } from '@/constants';

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
        let token = basicAuth(payload.username, payload.password)
        getUser(payload.username, payload.password)
            .then(response => {
                commit(mutation, authenticateResults.OK)
                commit(userTokenMutation, token)
            })
            .catch(error => {
                commit(userTokenMutation, token)
                if (error.response.status === 401) {
                    commit(mutation, authenticateResults.INVALID_CREDENTIALS)
                } else {
                    commit(mutation, authenticateResults.INTERNAL_SERVER_ERROR)
                }
            })
    }
}