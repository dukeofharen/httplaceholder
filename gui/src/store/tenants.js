import { defineStore } from "pinia";
import { get } from "@/utils/api";

export const useTenantsStore = defineStore({
  id: "tenants",
  state: () => ({}),
  getters: {},
  actions: {
    getTenantNames() {
      return get("/ph-api/tenants")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
