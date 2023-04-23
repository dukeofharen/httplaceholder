import { defineStore } from "pinia";
import { post } from "@/utils/api";
import type { FullStubModel } from "@/domain/stub/full-stub-model";

export interface ImportInputModel {
  input: string;
  doNotCreateStub: boolean;
  tenant: string;
  stubIdPrefix: string;
}

const buildQueryString = (input: ImportInputModel) => {
  let query = `?doNotCreateStub=${input.doNotCreateStub}`;
  if (input.tenant) {
    query += `&tenant=${input.tenant}`;
  }

  if (input.stubIdPrefix) {
    query += `&stubIdPrefix=${input.stubIdPrefix}`;
  }

  return query;
};

export const useImportStore = defineStore({
  id: "import",
  state: () => ({}),
  getters: {},
  actions: {
    async importCurlCommands(
      input: ImportInputModel
    ): Promise<FullStubModel[]> {
      return post(`/ph-api/import/curl${buildQueryString(input)}`, input.input)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    async importHar(input: ImportInputModel): Promise<FullStubModel[]> {
      return post(`/ph-api/import/har${buildQueryString(input)}`, input.input)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    importOpenApi(input: ImportInputModel): Promise<FullStubModel[]> {
      return post(
        `/ph-api/import/openapi${buildQueryString(input)}`,
        input.input
      )
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
