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
        runtimeVersion: "",
      },
      authenticationEnabled: false,
    }) as MetadataState,
  getters: {
    getAuthenticationEnabled: (state): boolean => state.authenticationEnabled,
    getMetadataState: (state): MetadataModel => state.metadata,
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
    async getMetadata(): Promise<MetadataModel> {
      try {
        const response = await get("/ph-api/metadata");
        this.metadata = response;
        return Promise.resolve(response);
      } catch (error) {
        return await Promise.reject(error);
      }
    },
  },
});
