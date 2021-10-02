<template>
  <div class="list-group">
    <button
      class="list-group-item list-group-item-action fw-bold"
      @click="tempRedirect"
    >
      Temporary redirect
    </button>
    <button
      class="list-group-item list-group-item-action fw-bold"
      @click="permanentRedirect"
    >
      Permanent redirect
    </button>
  </div>
</template>

<script>
import { useStore } from "vuex";

export default {
  name: "RedirectSelector",
  setup() {
    const store = useStore();

    // Methods
    const tempRedirect = () => {
      store.commit("stubForm/setDefaultTempRedirect");
      store.commit("stubForm/closeFormHelper");
    };
    const permanentRedirect = () => {
      store.commit("stubForm/setDefaultPermanentRedirect");
      store.commit("stubForm/closeFormHelper");
    };

    return { tempRedirect, permanentRedirect };
  },
};
</script>
