import axios from "axios";
import urls from "urls";
import {mutationNames} from "@/store/storeConstants";
import {resources} from "@/shared/resources";
import {messageTypes} from "@/shared/constants";

const handleHttpError = (commit, error) => {
    const status = error.response.status;
    if (status !== 401) {
        commit(mutationNames.storeToastMutation, {
            type: messageTypes.ERROR,
            message: resources.somethingWentWrongServer
        });
    }
};

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

export function getRequests({ commit, state }) {
    let rootUrl = urls.rootUrl;
    let url = `${rootUrl}ph-api/requests`;
    let token = state.userToken;
    let config = getConfig(token);
    axios
        .get(url, config)
        .then(response => {
            commit(mutationNames.storeRequestsMutation, response.data);
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}

export function clearRequests({ commit, state }) {
    let rootUrl = urls.rootUrl;
    let url = `${rootUrl}ph-api/requests`;
    let token = state.userToken;
    let config = getConfig(token);
    axios
        .delete(url, config)
        .then(response => {
            commit(mutationNames.storeToastMutation, {
                type: messageTypes.SUCCESS,
                message: resources.requestsDeletedSuccessfully
            });
            commit(mutationNames.storeRequests, []);
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}