/* eslint-disable no-empty-pattern */
/* eslint-disable no-async-promise-executor */
import createInstance from "@/axios/axiosInstanceFactory";
import yaml from "js-yaml";
import {resources} from "@/shared/resources";

export function getStubs() {
  return new Promise((resolve, reject) =>
    createInstance()
      .get("ph-api/stubs")
      .then(response => resolve(response.data))
      .catch(error => reject(error))
  );
}

export function getStub({}, payload) {
  return new Promise((resolve, reject) =>
    createInstance()
      .get(`ph-api/stubs/${payload.stubId}`)
      .then(response => resolve(response.data))
      .catch(error => reject(error))
  );
}

export function deleteStub({}, payload) {
  return new Promise((resolve, reject) =>
    createInstance()
      .delete(`ph-api/stubs/${payload.stubId}`)
      .then(() => resolve())
      .catch(error => reject(error))
  );
}

export function deleteAllStubs() {
  return new Promise((resolve, reject) =>
    createInstance()
      .delete("ph-api/stubs")
      .then(() => resolve())
      .catch(error => reject(error))
  );
}

export function addStubs({}, payload) {
  return new Promise(async (resolve, reject) => {
    let stubsArray;
    let parsedObject;
    if (payload.inputIsJson) {
      parsedObject = payload.input;
    } else {
      try {
        parsedObject = yaml.safeLoad(payload.input);
      } catch (error) {
        reject(error.message);
        return;
      }
    }

    if (!Array.isArray(parsedObject)) {
      stubsArray = [parsedObject];
    } else {
      stubsArray = parsedObject;
    }

    // Source: https://stackoverflow.com/questions/31424561/wait-until-all-promises-complete-even-if-some-rejected (Benjamin Gruenbaum)
    const reflect = p =>
      p.then(
        v => ({v, status: "fulfilled"}),
        e => ({e, status: "rejected"})
      );

    const promises = [];
    for (let index in stubsArray) {
      let stub = stubsArray[index];
      promises.push(
        new Promise((resolve, reject) =>
          createInstance()
            .post("ph-api/stubs", stub)
            .then(() => resolve(stub))
            .catch(error => reject({error, stubId: stub.id}))
        )
      );
    }

    const results = await Promise.all(promises.map(reflect));
    resolve(results);
  });
}

export function updateStub({}, payload) {
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
  });
}

export function createStubBasedOnRequest({}, payload) {
  return new Promise((resolve, reject) =>
    createInstance()
      .post(`ph-api/requests/${payload.correlationId}/stubs`, "")
      .then(response => resolve(response.data))
      .catch(error => reject(error))
  );
}
