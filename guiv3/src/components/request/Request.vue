<template>
  <accordion-item @buttonClicked="showDetails" :opened="accordionOpened">
    <template v-slot:button-text>
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
    </template>
    <template v-slot:accordion-body>
      <RequestDetails :request="request" />
    </template>
  </accordion-item>
</template>

<script>
import { computed, ref, onMounted, onUnmounted } from "vue";
import { useStore } from "vuex";
import { formatDateTime, formatFromNow } from "@/utils/datetime";
import Method from "@/components/request/Method";
import RequestDetails from "@/components/request/RequestDetails";
import AccordionItem from "@/components/bootstrap/AccordionItem";

export default {
  name: "Request",
  components: { AccordionItem, Method, RequestDetails },
  props: {
    overviewRequest: {
      type: Object,
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
    const requestDateTime = computed(() => formatDateTime(getRequestTime()));

    // Data
    const timeFromNow = ref(getTimeFromNow());
    const refreshTimeFromNowInterval = ref(null);
    const request = ref({});
    const accordionOpened = ref(false);

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
    const showDetails = async () => {
      if (Object.keys(request.value).length === 0) {
        request.value = await store.dispatch(
          "requests/getRequest",
          correlationId()
        );
        accordionOpened.value = true;
      } else {
        accordionOpened.value = !accordionOpened.value;
      }
    };

    return {
      executed: executed(),
      requestDateTime,
      timeFromNow,
      refreshTimeFromNowInterval,
      request,
      showDetails,
      accordionOpened,
    };
  },
};
</script>

<style scoped></style>
