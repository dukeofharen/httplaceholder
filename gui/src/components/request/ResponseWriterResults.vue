<template>
  <accordion-item>
    <template v-slot:button-text>Response writer results</template>
    <template v-slot:accordion-body>
      <ul class="list-group">
        <li
          v-for="result of stubResponseWriterResults"
          :key="result.responseWriterName"
          class="list-group-item"
        >
          <label class="fw-bold">{{ result.responseWriterName }}</label>
          <div v-if="result.log" class="text-secondary">{{ result.log }}</div>
        </li>
      </ul>
    </template>
  </accordion-item>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { StubResponseWriterResultModel } from "@/domain/request/stub-response-writer-result-model";

export default defineComponent({
  name: "ResponseWriterResults",
  props: {
    correlationId: {
      type: String,
      required: true,
    },
    stubResponseWriterResults: {
      type: Array as PropType<StubResponseWriterResultModel[]>,
      required: true,
    },
  },
});
</script>

<style scoped>
label {
  display: block;
}
</style>
