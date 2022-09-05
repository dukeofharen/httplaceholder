import { defineStore } from "pinia";
import { get } from "@/utils/api";
import type { ConfigurationModel } from "@/domain/stub/configuration-model";

export const useConfigurationStore = defineStore({
  id: "configuration",
  state: () => ({}),
  getters: {},
  actions: {
    async getConfiguration(): Promise<ConfigurationModel[]> {
      return get(`/ph-api/configuration`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
