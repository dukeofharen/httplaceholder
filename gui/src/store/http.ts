import { defineStore } from "pinia";

type HttpState = {
  numberOfCurrentHttpCalls: number;
};

export const useHttpStore = defineStore({
  id: "http",
  state: () =>
    ({
      numberOfCurrentHttpCalls: 0,
    } as HttpState),
  getters: {
    isExecutingHttpCalls: (state): boolean =>
      state.numberOfCurrentHttpCalls > 0,
  },
  actions: {
    increaseNumberOfCurrentHttpCalls() {
      this.numberOfCurrentHttpCalls++;
    },
    decreaseNumberOfCurrentHttpCalls() {
      if (this.numberOfCurrentHttpCalls !== 0) {
        this.numberOfCurrentHttpCalls--;
      }
    },
  },
});
