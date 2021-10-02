<template>
  <div class="list-group">
    <button
      v-for="(type, index) of types"
      :key="index"
      class="list-group-item list-group-item-action fw-bold"
      @click="lineEndingSelected(type.value)"
    >
      {{ type.name }}
    </button>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { lineEndingTypes } from "@/constants/stubFormResources";

export default {
  name: "LineEndingSelector",
  setup() {
    const store = useStore();

    // Data
    const types = [
      {
        name: "As provided in response body",
        value: null,
      },
      {
        name: "UNIX line endings",
        value: lineEndingTypes.unix,
      },
      {
        name: "Windows line endings",
        value: lineEndingTypes.windows,
      },
    ];

    // Methods
    const lineEndingSelected = (value) => {
      store.commit("stubForm/setLineEndings", value);
      store.commit("stubForm/closeFormHelper");
    };

    return { types, lineEndingSelected };
  },
};
</script>

<style scoped></style>
