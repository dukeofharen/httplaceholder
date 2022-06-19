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
    const compare = (
      a: StubExecutionResultModel,
      b: StubExecutionResultModel
    ) => {
      if (a.stubId > b.stubId) return 1;
      if (a.stubId < b.stubId) return -1;
      return 0;
    };
    const orderedStubExecutionResults = computed(() => {
      const allResults = props.stubExecutionResults;
      const passedResults = allResults.filter((r) => r.passed);
      passedResults.sort(compare);
      const otherResults = allResults.filter((r) => !r.passed);
      otherResults.sort(compare);
      return passedResults.concat(otherResults);
    });

    return {
      orderedStubExecutionResults,
    };
  },
});
</script>

<style scoped></style>
