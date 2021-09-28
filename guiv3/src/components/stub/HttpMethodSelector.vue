<template>
  <div class="list-group">
    <button
      v-for="(method, index) of httpMethods"
      :key="index"
      class="list-group-item list-group-item-action fw-bold"
      @click="methodSelected(method)"
    >
      {{ method }}
    </button>
  </div>
</template>

<script>
import { httpMethods } from "@/constants/stubFormResources";
import { useStore } from "vuex";

export default {
  name: "HttpMethodSelector",
  setup() {
    const store = useStore();

    // Methods
    const methodSelected = (method) => {
      store.commit("stubForm/setMethod", method);
      store.commit("stubForm/closeFormHelper");
    };

    return { httpMethods, methodSelected };
  },
};
</script>

<style scoped></style>
