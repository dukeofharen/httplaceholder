import { defineStore } from "pinia";
import { del, get } from "@/utils/api";
import type { RequestOverviewModel } from "@/domain/request/request-overview-model";
import type { RequestResultModel } from "@/domain/request/request-result-model";
import type { ResponseModel } from "@/domain/request/response-model";

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
    getResponse(correlationId: string): Promise<ResponseModel> {
      return get(`/ph-api/requests/${correlationId}/response`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    clearRequests(): Promise<any> {
      return del("/ph-api/requests")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    deleteRequest(correlationId: string): Promise<any> {
      return del(`/ph-api/requests/${correlationId}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
