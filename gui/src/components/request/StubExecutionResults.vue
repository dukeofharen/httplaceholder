<template>
  <accordion-item>
    <template v-slot:button-text>Stub execution results</template>
    <template v-slot:accordion-body>
      <div class="accordion">
        <StubExecutionResult
          v-for="result of orderedStubExecutionResults"
          :key="result.stubId"
          :correlation-id="correlationId"
          :result="result"
        />
      </div>
    </template>
  </accordion-item>
</template>

<script lang="ts">
import { computed, type PropType } from "vue";
import StubExecutionResult from "@/components/request/StubExecutionResult.vue";
import { defineComponent } from "vue";
import type { StubExecutionResultModel } from "@/domain/request/stub-execution-result-model";

export default defineComponent({
  name: "StubExecutionResults",
  components: { StubExecutionResult },
  props: {
    correlationId: {
      type: String,
      required: true,
    },
    stubExecutionResults: {
      type: Array as PropType<StubExecutionResultModel[]>,
      required: true,
    },
  },
  setup(props) {
    // Computed
    const orderedStubExecutionResults = computed(() => {
      const compare = (a: StubExecutionResultModel) => {
        if (a.passed) return -1;
        if (!a.passed) return 1;
        return 0;
      };
      const results = props.stubExecutionResults;
      results.sort(compare);
      return results;
    });

    return {
      orderedStubExecutionResults,
    };
  },
});
</script>

<style scoped></style>
