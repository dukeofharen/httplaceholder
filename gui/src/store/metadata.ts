import { defineStore } from "pinia";
import { get } from "@/utils/api";
import type { MetadataModel } from "@/domain/metadata/metadata-model";
import { FeatureFlagType } from "@/domain/metadata/feature-flag-type";
import type { FeatureResultModel } from "@/domain/metadata/feature-result-model";

type MetadataState = {
  metadata: MetadataModel;
  authenticationEnabled: boolean;
};

export const useMetadataStore = defineStore({
  id: "metadata",
  state: () =>
    ({
      metadata: {
        version: "",
        variableHandlers: [],
      },
      authenticationEnabled: false,
    }) as MetadataState,
  getters: {
    getAuthenticationEnabled: (state): boolean => state.authenticationEnabled,
  },
  actions: {
    checkFeatureIsEnabled: (
      feature: FeatureFlagType,
    ): Promise<FeatureResultModel> =>
      get(`/ph-api/metadata/features/${feature}`),
    async checkAuthenticationIsEnabled(): Promise<boolean> {
      const authEnabled = (
        await this.checkFeatureIsEnabled(FeatureFlagType.Authentication)
      ).enabled;
      this.authenticationEnabled = authEnabled;
      return authEnabled;
    },
    getMetadata(): Promise<MetadataModel> {
      return get("/ph-api/metadata")
        .then((response) => {
          this.metadata = response;
          return Promise.resolve(response);
        })
        .catch((error) => Promise.reject(error));
    },
  },
});
