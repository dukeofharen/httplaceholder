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
import { useStubFormStore } from "@/store/stubForm";
import { defineComponent } from "vue";

export default defineComponent({
  name: "HttpMethodSelector",
  setup() {
    const stubFormStore = useStubFormStore();

    // Methods
    const methodSelected = (method) => {
      stubFormStore.setMethod(method);
      stubFormStore.closeFormHelper();
    };

    return { httpMethods, methodSelected };
  },
});
</script>

<style scoped></style>
