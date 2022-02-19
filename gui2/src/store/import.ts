import { defineStore } from "pinia";
import { post } from "@/utils/api";
import type { FullStubModel } from "@/domain/stub/full-stub-model";

export interface ImportInputModel {
  input: string;
  doNotCreateStub: boolean;
  tenant: string;
}

export const useImportStore = defineStore({
  id: "import",
  state: () => ({}),
  getters: {},
  actions: {
    async importCurlCommands(
      input: ImportInputModel
    ): Promise<FullStubModel[]> {
      return post(
        `/ph-api/import/curl?doNotCreateStub=${input.doNotCreateStub}&tenant=${input.tenant}`,
        input.input
      )
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    async importHar(input: ImportInputModel): Promise<FullStubModel[]> {
      return post(
        `/ph-api/import/har?doNotCreateStub=${input.doNotCreateStub}&tenant=${input.tenant}`,
        input.input
      )
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    importOpenApi(input: ImportInputModel): Promise<FullStubModel[]> {
      return post(
        `/ph-api/import/openapi?doNotCreateStub=${input.doNotCreateStub}&tenant=${input.tenant}`,
        input.input
      )
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
