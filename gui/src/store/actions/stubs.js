import createInstance from "@/axios/axiosInstanceFactory";
import yaml from "js-yaml";
import {mutationNames} from "@/store/storeConstants";
import {resources} from "@/shared/resources";
import {toastError, toastSuccess} from "@/utils/toastUtil";

const handleHttpError = (commit, error) => {
    const status = error.response.status;
    if (status !== 401) {
        toastError(resources.somethingWentWrongServer);
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

export function getStubs({commit, state}) {
    const instance = createInstance();
    let token = state.userToken;
    let config = getConfig(token);
    instance
        .get("ph-api/stubs", config)
        .then(response => {
            commit("storeStubs", response.data);
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}

export function getStub({commit, state}, payload) {
    const instance = createInstance();
    let stubId = payload.stubId;
    let token = state.userToken;
    let config = getConfig(token);
    instance
        .get(`ph-api/stubs/${stubId}`, config)
        .then(response => {
            commit("storeLastSelectedStub", {
                fullStub: response.data
            });
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}

export function deleteStub({commit, state, dispatch}, payload) {
    const instance = createInstance();
    let stubId = payload.stubId;
    let token = state.userToken;
    let config = getConfig(token);
    instance
        .delete(`ph-api/stubs/${stubId}`, config)
        .then(response => {
            toastSuccess(resources.stubDeletedSuccessfully.format(stubId));
            dispatch("getStubs");
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}

export function deleteAllStubs({commit, state, dispatch}, payload) {
    const instance = createInstance();
    let token = state.userToken;
    let config = getConfig(token);
    instance
        .delete("ph-api/stubs", config)
        .then(response => {
            toastSuccess(resources.stubsDeletedSuccessfully);
            dispatch("getStubs");
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}

export function addStubs({commit, state}, payload) {
    const instance = createInstance();
    let token = state.userToken;
    let config = getConfig(token);
    config.headers["Content-Type"] = "application/json";

    let stubsArray;
    let parsedObject;
    try {
        parsedObject = yaml.safeLoad(payload.input);
    } catch (error) {
        toastError(error.message);
        return;
    }

    if (!Array.isArray(parsedObject)) {
        stubsArray = [parsedObject];
    } else {
        stubsArray = parsedObject;
    }

    for (let index in stubsArray) {
        let stub = stubsArray[index];
        instance
            .post("ph-api/stubs", stub, config)
            .then(() => toastSuccess(resources.stubAddedSuccessfully.format(stub.id)))
            .catch(error => {
                if (error.response.status === 409) {
                    toastError(resources.stubAlreadyAdded.format(stub.id))
                } else {
                    toastError(resources.stubNotAdded.format(stub.id))
                }
            });
    }
}

export function updateStub({commit, state}, payload) {
    const instance = createInstance();
    let token = state.userToken;
    let config = getConfig(token);
    config.headers["Content-Type"] = "application/json";

    let stub;
    try {
        stub = yaml.safeLoad(payload.input);
    } catch (error) {
        toastError(error.message);
        return;
    }

    if (!stub || Array.isArray(stub)) {
        toastError(resources.onlyOneStubAtATime);
        return;
    }

    instance
        .put(`ph-api/stubs/${payload.stubId}`, stub, config)
        .then(() => toastSuccess(resources.stubUpdatedSuccessfully.format(stub.id)))
        .catch(error => {
            if (error.response.status === 409) {
                toastError(resources.stubAlreadyAdded.format(stub.id))
            } else {
                toastError(resources.stubNotAdded.format(stub.id));
            }
        });
}

export function createStubBasedOnRequest({commit, state}, payload) {
    const instance = createInstance();
    let token = state.userToken;
    let config = getConfig(token);
    config.headers["Content-Type"] = "application/json";
    instance
        .post(`ph-api/requests/${payload.correlationId}/stubs`, "", config)
        .then(response => {
            let stub = response.data.stub;
            let message = resources.stubAddedSuccessfully;
            toastSuccess(message.format(stub.id))
            commit(mutationNames.storeLastSelectedStub, {
                fullStub: response.data
            });
        })
        .catch(() => toastError(resources.stubNotAddedGeneric));
}