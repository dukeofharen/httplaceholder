<template>
  <accordion-item>
    <template v-slot:button-text>Stub execution results</template>
    <template v-slot:accordion-body>
      <accordion> </accordion>
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

<script>
import { computed } from "vue";
import StubExecutionResult from "@/components/request/StubExecutionResult";
import AccordionItem from "@/components/bootstrap/AccordionItem";
import Accordion from "@/components/bootstrap/Accordion";

export default {
  name: "StubExecutionResults",
  components: { Accordion, AccordionItem, StubExecutionResult },
  props: {
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
      orderedStubExecutionResults,
    };
  },
};
</script>

<style scoped></style>
