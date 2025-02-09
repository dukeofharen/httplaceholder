import { defineStore } from "pinia";

type HttpState = {
  numberOfCurrentHttpCalls: number;
  showLoader: boolean;
  showLoaderTimeout: any;
};

export const useHttpStore = defineStore({
  id: "http",
  state: () =>
    ({
      numberOfCurrentHttpCalls: 0,
      showLoader: false,
    }) as HttpState,
  getters: {
    isExecutingHttpCalls: (state): boolean =>
      state.showLoader && state.numberOfCurrentHttpCalls > 0,
  },
  actions: {
    increaseNumberOfCurrentHttpCalls() {
      this.numberOfCurrentHttpCalls++;
      if (this.showLoaderTimeout) {
        clearTimeout(this.showLoaderTimeout);
      }

      if (!this.showLoader) {
        this.showLoaderTimeout = setTimeout(
          () => (this.showLoader = true),
          200,
        );
      }
    },
    decreaseNumberOfCurrentHttpCalls() {
      if (this.numberOfCurrentHttpCalls !== 0) {
        this.numberOfCurrentHttpCalls--;
      }

      if (this.numberOfCurrentHttpCalls <= 0) {
        this.showLoader = false;
        if (this.showLoaderTimeout) {
          clearTimeout(this.showLoaderTimeout);
        }
      }
    },
  },
});
