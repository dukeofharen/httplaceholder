import urls from "urls";
import axios from "axios";
import {mutationNames} from "@/store/storeConstants";
import {authenticateResults, messageTypes} from "@/shared/constants";
import {resources} from "@/shared/resources";
import {toastError, toastSuccess} from "@/utils/toastUtil";

const basicAuth = (username, password) => btoa(`${username}:${password}`);

const getConfig = (userToken, asYaml) => {
    if (!asYaml) {
        asYaml = false;
    }

    let headers = {
        Authorization: `Basic ${userToken}`
    };
    if (asYaml) {
        headers["Accept"] = "text/yaml";
    }

    return {
        headers: headers
    };
};

const getUser = (username, password) => {
    let rootUrl = urls.rootUrl;
    let url = `${rootUrl}ph-api/users/${username}`;
    let token = basicAuth(username, password);
    let config = getConfig(token);
    return axios.get(url, config);
};

export function ensureAuthenticated({commit}) {
    let username = "testUser";
    let password = "testPassword";
    getUser(username, password)
        .then(response => {
            // No authentication on endpoint, so no login required.
            commit(mutationNames.storeAuthMutation, true);
            commit(mutationNames.storeAuthRequired, false);
        })
        .catch(error => {
            // Authentication required, so show login screen.
            if (error.response.status === 401) {
                commit(mutationNames.storeAuthMutation, false);
                commit(mutationNames.storeAuthRequired, true);
            }
        });
}

export function authenticate({commit}, payload) {
    let token = basicAuth(payload.username, payload.password);
    getUser(payload.username, payload.password)
        .then(response => {
            commit(mutationNames.storeLastAuthResultMutation, authenticateResults.OK);
            commit(mutationNames.userTokenMutation, token);
            commit(mutationNames.storeAuthMutation, true);
        })
        .catch(error => {
            if (error.response.status === 401) {
                toastError(resources.credentialsIncorrect);
                commit(
                    mutationNames.storeLastAuthResultMutation,
                    authenticateResults.INVALID_CREDENTIALS
                );
            } else {
                toastError(resources.somethingWentWrongServer);
                commit(
                    mutationNames.storeLastAuthResultMutation,
                    authenticateResults.INTERNAL_SERVER_ERROR
                );
            }
        });
}