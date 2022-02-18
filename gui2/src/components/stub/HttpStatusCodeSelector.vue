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
import { useStubFormStore } from "@/store/stubForm";

export default {
  name: "HttpStatusCodeSelector",
  setup() {
    const stubFormStore = useStubFormStore();

    // Methods
    const codeSelected = (code) => {
      stubFormStore.setStatusCode(code.code);
      stubFormStore.closeFormHelper();
    };

    return { httpStatusCodes, codeSelected };
  },
};
</script>

<style scoped></style>
