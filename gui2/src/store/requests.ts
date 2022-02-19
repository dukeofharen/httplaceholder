import { defineStore } from "pinia";
import { del, get } from "@/utils/api";
import type { RequestOverviewModel } from "@/domain/request/request-overview-model";
import type { RequestResultModel } from "@/domain/request/request-result-model";

export const useRequestsStore = defineStore({
  id: "requests",
  state: () => ({}),
  getters: {},
  actions: {
    getRequestsOverview(): Promise<RequestOverviewModel[]> {
      return get("/ph-api/requests/overview")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    getRequest(correlationId: string): Promise<RequestResultModel> {
      return get(`/ph-api/requests/${correlationId}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    clearRequests() {
      return del("/ph-api/requests")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    deleteRequest(correlationId: string) {
      return del(`/ph-api/requests/${correlationId}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
