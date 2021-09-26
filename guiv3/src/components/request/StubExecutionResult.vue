<template>
  <accordion-item>
    <template v-slot:button-text>
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
    </template>
    <template v-slot:accordion-body>
      <div v-if="!result.conditions.length">
        No condition checkers executed for this stub.
      </div>
      <div
        v-else
        v-for="condition of result.conditions"
        :key="condition.checkerName"
      >
        <label class="fw-bold">{{ condition.checkerName }}</label>
        <div
          :class="{
            'text-success':
              condition.conditionValidation === conditionValidationType.Valid,
            'text-danger':
              condition.conditionValidation === conditionValidationType.Invalid,
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
    </template>
  </accordion-item>
</template>

<script>
import { conditionValidationType } from "@/constants/conditionValidationType";
import AccordionItem from "@/components/bootstrap/AccordionItem";

export default {
  name: "StubExecutionResult",
  components: { AccordionItem },
  props: {
    correlationId: {
      type: String,
      required: true,
    },
    result: {
      type: Object,
      required: true,
    },
  },
  setup() {
    return { conditionValidationType };
  },
};
</script>

<style scoped>
label {
  display: block;
}
</style>
