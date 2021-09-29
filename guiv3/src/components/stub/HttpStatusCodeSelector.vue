<template>
  <div class="list-group">
    <button
      v-for="(code, index) of httpStatusCodes"
      :key="index"
      class="list-group-item list-group-item-action fw-bold"
      @click="codeSelected(code)"
    >
      {{ code.code }} - {{ code.name }}
    </button>
  </div>
</template>

<script>
import { httpStatusCodes } from "@/constants/stubFormResources";
import { useStore } from "vuex";

export default {
  name: "HttpStatusCodeSelector",
  setup() {
    const store = useStore();

    // Methods
    const codeSelected = (code) => {
      store.commit("stubForm/setStatusCode", code.code);
      store.commit("stubForm/closeFormHelper");
    };

    return { httpStatusCodes, codeSelected };
  },
};
</script>

<style scoped></style>
