import { defineStore } from "pinia";
import { del, get, post, put } from "@/utils/api";
import yaml from "js-yaml";

export const useStubsStore = defineStore({
  id: "stubs",
  state: () => ({}),
  getters: {},
  actions: {
    getStub(stubId) {
      return get(`/ph-api/stubs/${stubId}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    getStubsOverview() {
      return get("/ph-api/stubs/overview")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    getStubs() {
      return get("/ph-api/stubs")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    async flipEnabled(stubId) {
      const stub = (await this.getStub(stubId)).stub;
      stub.enabled = !stub.enabled;
      await put(`/ph-api/stubs/${stubId}`, stub);
      return stub.enabled;
    },
    async enableStub(stubId) {
      const stub = (await this.getStub(stubId)).stub;
      stub.enabled = true;
      await put(`/ph-api/stubs/${stubId}`, stub);
      return stub.enabled;
    },
    async disableStub(stubId) {
      const stub = (await this.getStub(stubId)).stub;
      stub.enabled = false;
      await put(`/ph-api/stubs/${stubId}`, stub);
      return stub.enabled;
    },
    deleteStub(stubId) {
      return del(`/ph-api/stubs/${stubId}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    deleteStubs() {
      return del("/ph-api/stubs")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    addStubs(input) {
      const parsedObject = yaml.load(input);
      const stubsArray = Array.isArray(parsedObject)
        ? parsedObject
        : [parsedObject];
      return post("/ph-api/stubs/multiple", stubsArray)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    updateStub(payload) {
      const parsedObject = yaml.load(payload.input);
      return put(`/ph-api/stubs/${payload.stubId}`, parsedObject)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    createStubBasedOnRequest(payload) {
      return post(`/ph-api/requests/${payload.correlationId}/stubs`, {
        doNotCreateStub:
          payload.doNotCreateStub !== null ? payload.doNotCreateStub : false,
      })
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
