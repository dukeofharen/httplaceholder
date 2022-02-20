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

<script lang="ts">
import { useStubFormStore } from "@/store/stubForm";
import { defineComponent } from "vue";
import { httpMethods } from "@/domain/stubForm/http-methods";

export default defineComponent({
  name: "HttpMethodSelector",
  setup() {
    const stubFormStore = useStubFormStore();

    // Methods
    const methodSelected = (method: string) => {
      stubFormStore.setMethod(method);
      stubFormStore.closeFormHelper();
    };

    return { httpMethods, methodSelected };
  },
});
</script>

<style scoped></style>
