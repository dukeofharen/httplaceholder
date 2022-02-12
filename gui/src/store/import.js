import { defineStore } from "pinia";
import { post } from "@/utils/api";

export const useImportStore = defineStore({
  id: "import",
  state: () => {},
  getters: {},
  actions: {
    async importCurlCommands(input) {
      return post(
        `/ph-api/import/curl?doNotCreateStub=${input.doNotCreateStub}&tenant=${input.tenant}`,
        input.commands
      )
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    async importHar(input) {
      return post(
        `/ph-api/import/har?doNotCreateStub=${input.doNotCreateStub}&tenant=${input.tenant}`,
        input.har
      )
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    importOpenApi(input) {
      return post(
        `/ph-api/import/openapi?doNotCreateStub=${input.doNotCreateStub}&tenant=${input.tenant}`,
        input.openapi
      )
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
