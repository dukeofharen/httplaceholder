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
        Request headers
      </button>
    </h2>
    <div
      :id="contentId"
      class="accordion-collapse collapse"
      :aria-labelledby="headingId"
      :data-bs-parent="'#' + accordionId"
    >
      <div class="accordion-body">
        <table class="table" v-if="requestParams.headers">
          <tbody>
            <tr v-for="(value, key) in requestParams.headers" :key="key">
              <td class="p-1">{{ key }}</td>
              <td class="p-1">{{ value }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import { computed } from "vue";

export default {
  name: "RequestHeaders",
  props: {
    accordionId: {
      type: String,
      required: true,
    },
    request: {
      type: Object,
      required: true,
    },
  },
  setup(props) {
    // Functions
    const correlationId = () => props.request.correlationId;

    // Computed
    const headingId = computed(
      () => `requestheaders_heading-${correlationId()}`
    );
    const contentId = computed(
      () => `requestheaders_content-${correlationId()}`
    );
    const requestParams = computed(
      () => props.request?.requestParameters || {}
    );

    return { headingId, contentId, requestParams };
  },
};
</script>

<style scoped></style>
