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
        Response writer results
      </button>
    </h2>
    <div
      :id="contentId"
      class="accordion-collapse collapse"
      :aria-labelledby="headingId"
      :data-bs-parent="'#' + accordionId"
    >
      <div class="accordion-body">
        <ul class="list-group">
          <li
            v-for="result of stubResponseWriterResults"
            :key="result.responseWriterName"
            class="list-group-item fw-bold"
          >
            {{ result.responseWriterName }}
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script>
import { computed } from "vue";

export default {
  name: "ResponseWriterResults",
  props: {
    accordionId: {
      type: String,
      required: true,
    },
    correlationId: {
      type: String,
      required: true,
    },
    stubResponseWriterResults: {
      type: Array,
      required: true,
    },
  },
  setup(props) {
    // Computed
    const headingId = computed(
      () => `responsewriterresults_heading-${props.correlationId}`
    );
    const contentId = computed(
      () => `responsewriterresults_content-${props.correlationId}`
    );

    return { headingId, contentId };
  },
};
</script>

<style scoped></style>
