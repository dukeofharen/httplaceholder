import { del, get, put, post } from "@/utils/api";
import yaml from "js-yaml";

const state = () => ({});

const getStub = (stubId) => {
  return get(`/ph-api/stubs/${stubId}`)
    .then((response) => Promise.resolve(response))
    .catch((error) => Promise.reject(error));
};

const actions = {
  getStubsOverview() {
    return get("/ph-api/stubs/overview")
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  getStub(_, stubId) {
    return getStub(stubId);
  },
  getStubs() {
    return get("/ph-api/stubs")
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  async flipEnabled(_, stubId) {
    const stub = (await getStub(stubId)).stub;
    stub.enabled = !stub.enabled;
    await put(`/ph-api/stubs/${stubId}`, stub);
    return stub.enabled;
  },
  deleteStub(_, stubId) {
    return del(`/ph-api/stubs/${stubId}`)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  deleteStubs() {
    return del("/ph-api/stubs")
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  addStubs(_, input) {
    const parsedObject = yaml.load(input);
    const stubsArray = Array.isArray(parsedObject)
      ? parsedObject
      : [parsedObject];
    return post("/ph-api/stubs/multiple", stubsArray)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  updateStub(_, payload) {
    const parsedObject = yaml.load(payload.input);
    return put(`/ph-api/stubs/${payload.stubId}`, parsedObject)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
  createStubBasedOnRequest(_, payload) {
    return post(`/ph-api/requests/${payload.correlationId}/stubs`, {
      doNotCreateStub:
        payload.doNotCreateStub !== null ? payload.doNotCreateStub : false,
    })
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
};

const mutations = {};

const getters = {};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions,
};
