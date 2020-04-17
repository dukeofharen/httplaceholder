import axios from "axios";
import urls from "urls";
import yaml from "js-yaml";
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

export function getStubs({ commit, state }) {
    let rootUrl = urls.rootUrl;
    let url = `${rootUrl}ph-api/stubs`;
    let token = state.userToken;
    let config = getConfig(token);
    axios
        .get(url, config)
        .then(response => {
            commit("storeStubs", response.data);
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}

export function getStub({ commit, state }, payload) {
    let stubId = payload.stubId;
    let rootUrl = urls.rootUrl;
    let url = `${rootUrl}ph-api/stubs/${stubId}`;
    let token = state.userToken;
    let config = getConfig(token);
    axios
        .get(url, config)
        .then(response => {
            commit("storeLastSelectedStub", {
                fullStub: response.data
            });
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}

export function deleteStub({ commit, state, dispatch }, payload) {
    let stubId = payload.stubId;
    let rootUrl = urls.rootUrl;
    let url = `${rootUrl}ph-api/stubs/${stubId}`;
    let token = state.userToken;
    let config = getConfig(token);
    axios
        .delete(url, config)
        .then(response => {
            commit(mutationNames.storeToastMutation, {
                type: messageTypes.SUCCESS,
                message: resources.stubDeletedSuccessfully.format(stubId)
            });
            dispatch("getStubs");
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}

export function deleteAllStubs({ commit, state, dispatch }, payload) {
    let rootUrl = urls.rootUrl;
    let url = `${rootUrl}ph-api/stubs`;
    let token = state.userToken;
    let config = getConfig(token);
    axios
        .delete(url, config)
        .then(response => {
            commit(mutationNames.storeToastMutation, {
                type: messageTypes.SUCCESS,
                message: resources.stubsDeletedSuccessfully
            });
            dispatch("getStubs");
        })
        .catch(error => {
            handleHttpError(commit, error);
        });
}

export function addStubs({ commit, state }, payload) {
    let rootUrl = urls.rootUrl;
    let url = `${rootUrl}ph-api/stubs`;
    let token = state.userToken;
    let config = getConfig(token);
    config.headers["Content-Type"] = "application/json";

    let stubsArray;
    let parsedObject;
    try {
        parsedObject = yaml.safeLoad(payload.input);
    } catch (error) {
        commit(mutationNames.storeToastMutation, {
            type: messageTypes.ERROR,
            message: error.message
        });
        return;
    }

    if (!Array.isArray(parsedObject)) {
        stubsArray = [parsedObject];
    } else {
        stubsArray = parsedObject;
    }

    for (let index in stubsArray) {
        let stub = stubsArray[index];
        axios
            .post(url, stub, config)
            .then(response => {
                let message = resources.stubAddedSuccessfully;

                commit(mutationNames.storeToastMutation, {
                    type: messageTypes.SUCCESS,
                    message: message.format(stub.id)
                });
            })
            .catch(error => {
                if (error.response.status === 409) {
                    commit(mutationNames.storeToastMutation, {
                        type: messageTypes.ERROR,
                        message: resources.stubAlreadyAdded.format(stub.id)
                    });
                } else {
                    commit(mutationNames.storeToastMutation, {
                        type: messageTypes.ERROR,
                        message: resources.stubNotAdded.format(stub.id)
                    });
                }
            });
    }
}

export function updateStub({ commit, state }, payload) {
    let rootUrl = urls.rootUrl;
    let url = `${rootUrl}ph-api/stubs/${payload.stubId}`;
    let token = state.userToken;
    let config = getConfig(token);
    config.headers["Content-Type"] = "application/json";

    let stub;
    try {
        stub = yaml.safeLoad(payload.input);
    } catch (error) {
        commit(mutationNames.storeToastMutation, {
            type: messageTypes.ERROR,
            message: error.message
        });
        return;
    }

    if (!stub || Array.isArray(stub)) {
        commit(mutationNames.storeToastMutation, {
            type: messageTypes.ERROR,
            message: resources.onlyOneStubAtATime
        });
        return;
    }

    axios
        .put(url, stub, config)
        .then(response => {
            let message = resources.stubUpdatedSuccessfully;
            commit(mutationNames.storeToastMutation, {
                type: messageTypes.SUCCESS,
                message: message.format(stub.id)
            });
        })
        .catch(error => {
            if (error.response.status === 409) {
                commit(mutationNames.storeToastMutation, {
                    type: messageTypes.ERROR,
                    message: resources.stubAlreadyAdded.format(stub.id)
                });
            } else {
                commit(mutationNames.storeToastMutation, {
                    type: messageTypes.ERROR,
                    message: resources.stubNotAdded.format(stub.id)
                });
            }
        });
}

export function createStubBasedOnRequest({ commit, state }, payload) {
    let rootUrl = urls.rootUrl;
    let url = `${rootUrl}ph-api/requests/${payload.correlationId}/stubs`;
    let token = state.userToken;
    let config = getConfig(token);
    config.headers["Content-Type"] = "application/json";
    axios
        .post(url, "", config)
        .then(response => {
            let stub = response.data.stub;
            let message = resources.stubAddedSuccessfully;
            commit(mutationNames.storeToastMutation, {
                type: messageTypes.SUCCESS,
                message: message.format(stub.id)
            });
            commit(mutationNames.storeLastSelectedStub, {
                fullStub: response.data
            });
        })
        .catch(error =>
            commit(mutationNames.storeToastMutation, {
                type: messageTypes.ERROR,
                message: resources.stubNotAddedGeneric
            })
        );
}