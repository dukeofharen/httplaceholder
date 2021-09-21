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
        Query parameters
      </button>
    </h2>
    <div
      :id="contentId"
      class="accordion-collapse collapse"
      :aria-labelledby="headingId"
      :data-bs-parent="'#' + accordionId"
    >
      <div class="accordion-body">
        <table class="table" v-if="queryParameters">
          <tbody>
            <tr v-for="(value, key) in queryParameters" :key="key">
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
import { parseUrl } from "@/utils/url";

export default {
  name: "QueryParams",
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
    const headingId = computed(() => `queryparams_heading-${correlationId()}`);
    const contentId = computed(() => `queryparams_content-${correlationId()}`);
    const requestParams = computed(
      () => props.request?.requestParameters || {}
    );
    const queryParameters = computed(() => {
      const req = requestParams.value;
      return req.url ? parseUrl(req.url) : {};
    });

    return { headingId, contentId, requestParams, queryParameters };
  },
};
</script>

<style scoped></style>
