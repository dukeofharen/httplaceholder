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
        @click="showDetails"
      >
        <span>
          <Method :method="overviewRequest.method" />
          <span class="ms-sm-1">{{ overviewRequest.url }}</span>
          <span class="ms-sm-1">
            <span>(</span>
            <span
              class="fw-bold"
              :class="{ 'text-success': executed, 'text-danger': !executed }"
              >{{ executed ? "executed" : "not executed" }}</span
            >
            <span>&nbsp;|&nbsp;</span>
            <span :title="requestDateTime">{{ timeFromNow }}</span>
            <span>)</span></span
          >
        </span>
      </button>
    </h2>
    <div
      :id="contentId"
      class="accordion-collapse collapse"
      :aria-labelledby="headingId"
      :data-bs-parent="'#' + accordionId"
    >
      <div class="accordion-body">
        <RequestDetails :request="{}" />
      </div>
    </div>
  </div>
</template>

<script>
import { computed, ref, onMounted, onUnmounted } from "vue";
import { useStore } from "vuex";
import { formatDateTime, formatFromNow } from "@/utils/datetime";
import Method from "@/components/request/Method";
import RequestDetails from "@/components/request/RequestDetails";

export default {
  name: "Request",
  components: { Method, RequestDetails },
  props: {
    overviewRequest: {
      type: Object,
      required: true,
    },
    accordionId: {
      type: String,
      required: true,
    },
  },
  setup(props) {
    const store = useStore();

    // Functions
    const getRequestTime = () => props.overviewRequest.requestEndTime;
    const correlationId = () => props.overviewRequest.correlationId;
    const executed = () => props.overviewRequest.executingStubId;
    const getTimeFromNow = () => formatFromNow(getRequestTime());

    // Computed
    const headingId = computed(() => `heading-${correlationId()}`);
    const contentId = computed(() => `content-${correlationId()}`);
    const requestDateTime = computed(() => formatDateTime(getRequestTime()));

    // Data
    const timeFromNow = ref(getTimeFromNow());
    const refreshTimeFromNowInterval = ref(null);
    const request = ref(null);

    // Lifecycle
    onMounted(() => {
      refreshTimeFromNowInterval.value = setInterval(() => {
        timeFromNow.value = getTimeFromNow();
      }, 60000);
    });
    onUnmounted(() => {
      if (refreshTimeFromNowInterval.value) {
        clearInterval(refreshTimeFromNowInterval.value);
      }
    });

    // Methods
    const loadRequest = async () => {
      if (!request.value) {
        request.value = await store.dispatch(
          "requests/getRequest",
          correlationId()
        );
      }
    };
    const showDetails = async () => {
      await loadRequest();
    };

    return {
      headingId,
      contentId,
      executed: executed(),
      requestDateTime,
      timeFromNow,
      refreshTimeFromNowInterval,
      request,
      loadRequest,
      showDetails,
    };
  },
};
</script>

<style scoped></style>
