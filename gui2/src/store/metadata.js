import { defineStore } from "pinia";
import { get } from "@/utils/api";
import { FeatureFlagType } from "@/constants/featureFlagType";

export const useMetadataStore = defineStore({
  id: "metadata",
  state: () => ({
    metadata: {
      version: "",
      variableHandlers: [],
    },
    authenticationEnabled: false,
  }),
  getters: {
    getAuthenticationEnabled: (state) => state.authenticationEnabled,
  },
  actions: {
    checkFeatureIsEnabled: (feature) =>
      get(`/ph-api/metadata/features/${feature}`),
    async checkAuthenticationIsEnabled() {
      const authEnabled = (
        await this.checkFeatureIsEnabled(FeatureFlagType.Authentication)
      ).enabled;
      this.authenticationEnabled = authEnabled;
      return authEnabled;
    },
    getMetadata() {
      return get("/ph-api/metadata")
        .then((response) => {
          this.metadata = response;
          return Promise.resolve(response);
        })
        .catch((error) => Promise.reject(error));
    },
  },
});
