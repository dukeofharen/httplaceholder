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
        <span>{{ result.stubId }}</span>
        <span>&nbsp;</span>
        <span>(</span>
        <span
          class="fw-bold"
          :class="{
            'text-success': result.passed,
            'text-danger': !result.passed,
          }"
          >{{ result.passed ? "passed" : "not passed" }}</span
        >
        <span>)</span>
      </button>
    </h2>
    <div
      :id="contentId"
      class="accordion-collapse collapse"
      :aria-labelledby="headingId"
      :data-bs-parent="'#' + accordionId"
    >
      <div v-if="result.conditions.length" class="accordion-body">
        <div
          v-for="condition of result.conditions"
          :key="condition.checkerName"
        >
          <label class="fw-bold">{{ condition.checkerName }}</label>
          <div
            :class="{
              'text-success':
                condition.conditionValidation === conditionValidationType.Valid,
              'text-danger':
                condition.conditionValidation ===
                conditionValidationType.Invalid,
            }"
          >
            {{
              condition.conditionValidation === conditionValidationType.Valid
                ? "passed"
                : "not passed"
            }}
          </div>
          <div v-if="condition.log">{{ condition.log }}</div>
          <hr />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { computed } from "vue";
import { conditionValidationType } from "@/constants/conditionValidationType";

export default {
  name: "StubExecutionResult",
  props: {
    accordionId: {
      type: String,
      required: true,
    },
    correlationId: {
      type: String,
      required: true,
    },
    result: {
      type: Object,
      required: true,
    },
  },
  setup(props) {
    // Computed
    const headingId = computed(
      () =>
        `stubexecutionresults_heading-${props.result.stubId}-${props.correlationId}`
    );
    const contentId = computed(
      () =>
        `stubexecutionresults_content-${props.result.stubId}-${props.correlationId}`
    );

    return { headingId, contentId, conditionValidationType };
  },
};
</script>

<style scoped>
label {
  display: block;
}
</style>
