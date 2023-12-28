import { defineStore } from "pinia";

type GeneralState = {
  showLoader: boolean;
};
export const useGeneralStore = defineStore({
  id: "general",
  state: () =>
    ({
      showLoader: false,
    }) as GeneralState,
  getters: {
    shouldShowLoader: (state) => state.showLoader,
  },
  actions: {
    doShowLoader() {
      this.showLoader = true;
    },
    doHideLoader() {
      this.showLoader = false;
    },
  },
});
