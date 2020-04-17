import createInstance from "@/axios/axiosInstanceFactory";
import yaml from "js-yaml";
import {mutationNames} from "@/store/storeConstants";
import {resources} from "@/shared/resources";
import {toastError, toastSuccess} from "@/utils/toastUtil";

export function getStubs({commit}) {
    createInstance()
        .get("ph-api/stubs")
        .then(response => commit("storeStubs", response.data));
}

export function getStub({commit, state}, payload) {
    createInstance()
        .get(`ph-api/stubs/${payload.stubId}`)
        .then(response => commit("storeLastSelectedStub", {
            fullStub: response.data
        }));
}

export function deleteStub({commit, state, dispatch}, payload) {
    createInstance()
        .delete(`ph-api/stubs/${payload.stubId}`)
        .then(() => {
            toastSuccess(resources.stubDeletedSuccessfully.format(payload.stubId));
            dispatch("getStubs");
        });
}

export function deleteAllStubs({dispatch}) {
    createInstance()
        .delete("ph-api/stubs")
        .then(() => {
            toastSuccess(resources.stubsDeletedSuccessfully);
            dispatch("getStubs");
        });
}

export function addStubs({commit, state}, payload) {
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

    const promises = [];
    for (let index in stubsArray) {
        let stub = stubsArray[index];
        promises.push(new Promise((resolve, reject) => createInstance()
            .post("ph-api/stubs", stub)
            .then(() => resolve(stub))
            .catch(error => reject({error, stubId: stub.id}))));
    }

    return promises;
}

export function updateStub({commit, state}, payload) {

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

    createInstance()
        .put(`ph-api/stubs/${payload.stubId}`, stub)
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
    createInstance()
        .post(`ph-api/requests/${payload.correlationId}/stubs`, "")
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