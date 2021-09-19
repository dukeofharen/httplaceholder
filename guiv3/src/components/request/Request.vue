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
        Accordion Item #{{ request.correlationId }}
      </button>
    </h2>
    <div
      :id="contentId"
      class="accordion-collapse collapse"
      :aria-labelledby="headingId"
      :data-bs-parent="'#' + accordionId"
    >
      <div class="accordion-body">
        {{ request }}
      </div>
    </div>
  </div>
</template>

<script>
import { computed } from "vue";

export default {
  name: "Request",
  props: {
    request: {
      type: Object,
      required: true,
    },
    accordionId: {
      type: String,
      required: true,
    },
  },
  setup(props) {
    const correlationId = () => props.request.correlationId;
    const headingId = computed(() => `heading-${correlationId()}`);
    const contentId = computed(() => `content-${correlationId()}`);

    return { headingId, contentId };
  },
};
</script>

<style scoped></style>
