import createInstance from "@/axios/axiosInstanceFactory";
import yaml from "js-yaml";
import {resources} from "@/shared/resources";
import {toastError, toastSuccess} from "@/utils/toastUtil";

export function getStubs() {
    return new Promise((resolve, reject) =>
        createInstance()
            .get("ph-api/stubs")
            .then(response => resolve(response.data))
            .catch(error => reject(error)));
}

export function getStub({}, payload) {
    return new Promise((resolve, reject) => createInstance()
        .get(`ph-api/stubs/${payload.stubId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error)));
}

export function deleteStub({commit, state, dispatch}, payload) {
    return new Promise((resolve, reject) => createInstance()
        .delete(`ph-api/stubs/${payload.stubId}`)
        .then(() => resolve())
        .catch(error => reject(error)));
}

export function deleteAllStubs() {
    return new Promise((resolve, reject) =>
        createInstance()
            .delete("ph-api/stubs")
            .then(() => resolve())
            .catch(error => reject(error)));
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
    return new Promise((resolve, reject) => {
        let stub;
        try {
            stub = yaml.safeLoad(payload.input);
        } catch (e) {
            reject(e);
            return;
        }

        if (!stub || Array.isArray(stub)) {
            reject(resources.onlyOneStubAtATime);
            return;
        }

        createInstance()
            .put(`ph-api/stubs/${payload.stubId}`, stub)
            .then(() => resolve())
            .catch(error => reject(error));
    })
}

export function createStubBasedOnRequest({commit, state}, payload) {
    return new Promise((resolve, reject) =>
        createInstance()
            .post(`ph-api/requests/${payload.correlationId}/stubs`, "")
            .then(response => resolve(response.data))
            .catch(error => reject(error)));
}