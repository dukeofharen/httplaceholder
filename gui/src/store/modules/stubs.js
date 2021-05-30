import createInstance from "@/axios/axiosInstanceFactory";
import yaml from "js-yaml";

const state = () => ({});

const actions = {
  getStubsOverview() {
    return new Promise((resolve, reject) =>
      createInstance()
        .get("ph-api/stubs/overview")
        .then(response => resolve(response.data))
        .catch(error => reject(error))
    );
  },
  getStubs() {
    return new Promise((resolve, reject) =>
      createInstance()
        .get("ph-api/stubs")
        .then(response => resolve(response.data))
        .catch(error => reject(error))
    );
  },
  /* eslint no-empty-pattern: 0 */
  getStub({}, payload) {
    return new Promise((resolve, reject) =>
      createInstance()
        .get(`ph-api/stubs/${payload.stubId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error))
    );
  },
  /* eslint no-empty-pattern: 0 */
  deleteStub({}, payload) {
    return new Promise((resolve, reject) =>
      createInstance()
        .delete(`ph-api/stubs/${payload.stubId}`)
        .then(() => resolve())
        .catch(error => reject(error))
    );
  },
  deleteAllStubs() {
    return new Promise((resolve, reject) =>
      createInstance()
        .delete("ph-api/stubs")
        .then(() => resolve())
        .catch(error => reject(error))
    );
  },
  /* eslint no-empty-pattern: 0 */
  addStubs({}, payload) {
    /* eslint no-async-promise-executor: 0 */
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
          v => ({ v, status: "fulfilled" }),
          e => ({ e, status: "rejected" })
        );

      const promises = [];
      for (let index in stubsArray) {
        let stub = stubsArray[index];
        promises.push(
          new Promise((resolve, reject) =>
            createInstance()
              .post("ph-api/stubs", stub)
              .then(r => r.data)
              .then(result => resolve(result))
              .catch(error => reject({ error, stubId: stub.id }))
          )
        );
      }

      const results = await Promise.all(promises.map(reflect));
      resolve(results);
    });
  },
  /* eslint no-empty-pattern: 0 */
  createStubBasedOnRequest({}, payload) {
    return new Promise((resolve, reject) =>
      createInstance()
        .post(`ph-api/requests/${payload.correlationId}/stubs`, {
          doNotCreateStub:
            payload.doNotCreateStub !== null ? payload.doNotCreateStub : false
        })
        .then(response => resolve(response.data))
        .catch(error => reject(error))
    );
  },
  async enableStub({}, stubId) {
    const stub = (await createInstance().get(`ph-api/stubs/${stubId}`)).data
      .stub;
    stub.enabled = true;
    return new Promise((resolve, reject) =>
      createInstance()
        .put(`ph-api/stubs/${stubId}`, stub)
        .then(() => resolve())
        .catch(error => reject(error))
    );
  },
  async disableStub({}, stubId) {
    const stub = (await createInstance().get(`ph-api/stubs/${stubId}`)).data
      .stub;
    stub.enabled = false;
    return new Promise((resolve, reject) =>
      createInstance()
        .put(`ph-api/stubs/${stubId}`, stub)
        .then(() => resolve())
        .catch(error => reject(error))
    );
  }
};

const mutations = {};

const getters = {};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions
};
