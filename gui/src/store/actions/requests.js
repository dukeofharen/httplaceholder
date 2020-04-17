import createInstance from "@/axios/axiosInstanceFactory";
import {mutationNames} from "@/store/storeConstants";
import {resources} from "@/shared/resources";
import {toastError, toastSuccess} from "@/utils/toastUtil";

const handleHttpError = (commit, error) => {
    const status = error.response.status;
    if (status !== 401) {
        toastError(resources.somethingWentWrongServer)
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
    const instance = createInstance();
    let token = state.userToken;
    let config = getConfig(token);
    instance
        .get("ph-api/requests", config)
        .then(response => {
            commit(mutationNames.storeRequestsMutation, response.data);
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}

export function clearRequests({ commit, state }) {
    const instance = createInstance();
    let token = state.userToken;
    let config = getConfig(token);
    instance
        .delete("ph-api/requests", config)
        .then(response => {
            toastSuccess(resources.requestsDeletedSuccessfully)
            commit(mutationNames.storeRequests, []);
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}