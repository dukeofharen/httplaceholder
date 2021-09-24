<template>
  <div>
    <div class="row">
      <div class="col-md-12 mb-3">
        <label>URL</label>
        <span>{{ requestParams.url }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>Client IP</label>
        <span>{{ requestParams.clientIp }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>Correlation ID</label>
        <span>{{ request.correlationId }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>Executed stub</label>
        <!-- TODO Add router link here -->
        <span>{{ request.executingStubId }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>Stub tenant (category)</label>
        <!-- TODO Add router link here -->
        <span>{{ request.stubTenant }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>Request time</label>
        <span>{{ requestTime }} (it took {{ duration }} ms)</span>
      </div>
      <div class="col-md-12 mb-3">
        <div class="accordion" :id="headerAndQueryAccordionId">
          <RequestHeaders
            :accordion-id="headerAndQueryAccordionId"
            :request="request"
          />
          <QueryParams
            v-if="showQueryParameters"
            :accordion-id="headerAndQueryAccordionId"
            :request="request"
          />
        </div>
      </div>
      <div v-if="showResults" class="col-md-12">
        <div class="accordion" :id="resultsAccordionId">
          <StubExecutionResults
            v-if="showStubExecutionResults"
            :accordion-id="resultsAccordionId"
            :correlation-id="request.correlationId"
            :stub-execution-results="request.stubExecutionResults"
          />
          <ResponseWriterResults
            v-if="showStubResponseWriterResults"
            :accordion-id="resultsAccordionId"
            :correlation-id="request.correlationId"
            :stub-response-writer-results="request.stubResponseWriterResults"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { computed } from "vue";
import { formatDateTime, getDuration } from "@/utils/datetime";
import RequestHeaders from "@/components/request/RequestHeaders";
import QueryParams from "@/components/request/QueryParams";
import StubExecutionResults from "@/components/request/StubExecutionResults";
import ResponseWriterResults from "@/components/request/ResponseWriterResults";

export default {
  name: "RequestDetails",
  components: {
    StubExecutionResults,
    RequestHeaders,
    QueryParams,
    ResponseWriterResults,
  },
  props: {
    request: {
      type: Object,
      required: true,
    },
  },
  setup(props) {
    // Data
    const headerAndQueryAccordionId = "headers-query-accordion";
    const resultsAccordionId = "results-accordion";

    // Computed
    const requestParams = computed(
      () => props.request?.requestParameters || {}
    );
    const requestTime = computed(() =>
      formatDateTime(requestParams.value.requestEndTime)
    );
    const duration = computed(() => {
      const req = props.request;
      return getDuration(req.requestBeginTime, req.requestEndTime);
    });
    const showQueryParameters = computed(
      () => requestParams.value?.url?.includes("?") || false
    );

    const showStubExecutionResults = computed(
      () =>
        props.request.stubExecutionResults &&
        props.request.stubExecutionResults.length
    );
    const showStubResponseWriterResults = computed(
      () =>
        props.request.stubResponseWriterResults &&
        props.request.stubResponseWriterResults.length
    );
    const showResults = computed(
      () =>
        showStubExecutionResults.value && showStubResponseWriterResults.value
    );

    return {
      requestParams,
      requestTime,
      duration,
      headerAndQueryAccordionId,
      resultsAccordionId,
      showQueryParameters,
      showStubExecutionResults,
      showStubResponseWriterResults,
      showResults,
    };
  },
};
</script>

<style lang="scss" scoped>
label {
  display: block;
  font-weight: bold;
}
</style>
