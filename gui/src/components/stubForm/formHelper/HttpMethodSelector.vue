<template>
  <div class="list-group">
    <button
      class="list-group-item list-group-item-action fw-bold"
      @click="multipleMethods"
    >
      Multiple methods
    </button>
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
import { defaultValues } from "@/domain/stubForm/default-values";

export default defineComponent({
  name: "HttpMethodSelector",
  setup() {
    const stubFormStore = useStubFormStore();

    // Methods
    const methodSelected = (method: string) => {
      stubFormStore.setMethod(method);
      stubFormStore.closeFormHelper();
    };
    const multipleMethods = () => {
      stubFormStore.setMethod(defaultValues.methods);
      stubFormStore.closeFormHelper();
    };

    return { httpMethods, methodSelected, multipleMethods };
  },
});
</script>

<style scoped></style>
