<template>
  <div class="accordion-item">
    <h2 class="accordion-header" :id="headingId">
      <button
        class="accordion-button collapsed"
        type="button"
        data-bs-toggle="collapse"
        :data-bs-target="'#' + contentId"
        aria-expanded="false"
        :aria-controls="contentId"
      >
        Stub execution results
      </button>
    </h2>
    <div
      :id="contentId"
      class="accordion-collapse collapse"
      :aria-labelledby="headingId"
      :data-bs-parent="'#' + accordionId"
    >
      <div class="accordion-body">
        <div class="accordion" :id="subAccordionId">
          <StubExecutionResult
            v-for="result of orderedStubExecutionResults"
            :key="result.stubId"
            :accordion-id="subAccordionId"
            :correlation-id="correlationId"
            :result="result"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { computed } from "vue";
import StubExecutionResult from "@/components/request/StubExecutionResult";

export default {
  name: "StubExecutionResults",
  components: { StubExecutionResult },
  props: {
    accordionId: {
      type: String,
      required: true,
    },
    correlationId: {
      type: String,
      required: true,
    },
    stubExecutionResults: {
      type: Array,
      required: true,
    },
  },
  setup(props) {
    // Computed
    const headingId = computed(
      () => `stubexecutionresults_heading-${props.correlationId}`
    );
    const contentId = computed(
      () => `stubexecutionresults_content-${props.correlationId}`
    );
    const subAccordionId = computed(
      () => `stub-execution-results_${props.correlationId}`
    );
    const orderedStubExecutionResults = computed(() => {
      const compare = (a) => {
        if (a.passed) return -1;
        if (!a.passed) return 1;
        return 0;
      };
      const results = props.stubExecutionResults;
      results.sort(compare);
      return results;
    });

    return {
      headingId,
      contentId,
      subAccordionId,
      orderedStubExecutionResults,
    };
  },
};
</script>

<style scoped></style>
