import { defineStore } from "pinia";
import { get } from "@/utils/api";

export const useTenantsStore = defineStore({
  id: "tenants",
  state: () => ({}),
  getters: {},
  actions: {
    getTenantNames(): Promise<string[]> {
      return get("/ph-api/tenants")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
