import { defineStore } from "pinia";
import { del, get } from "@/utils/api";

export const useRequestsStore = defineStore({
  id: "requests",
  state: () => ({}),
  getters: {},
  actions: {
    getRequestsOverview() {
      return get("/ph-api/requests/overview")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    getRequest(correlationId) {
      return get(`/ph-api/requests/${correlationId}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    clearRequests() {
      return del("/ph-api/requests")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    deleteRequest(correlationId) {
      return del(`/ph-api/requests/${correlationId}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
