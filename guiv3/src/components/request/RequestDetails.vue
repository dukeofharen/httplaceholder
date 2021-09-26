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
        <router-link
          :to="{ name: 'Stubs', query: { filter: request.executingStubId } }"
          >{{ request.executingStubId }}</router-link
        >
      </div>
      <div class="col-md-12 mb-3">
        <label>Stub tenant (category)</label>
        <router-link
          :to="{ name: 'Stubs', query: { tenant: request.stubTenant } }"
          >{{ request.stubTenant }}</router-link
        >
      </div>
      <div class="col-md-12 mb-3">
        <label>Request time</label>
        <span>{{ requestTime }} (it took {{ duration }} ms)</span>
      </div>
      <div class="col-md-12 mb-3">
        <accordion> </accordion>
        <div class="accordion">
          <RequestHeaders :request="request" />
          <QueryParams v-if="showQueryParameters" :request="request" />
        </div>
      </div>
      <div v-if="showResults" class="col-md-12">
        <accordion> </accordion>
        <div class="accordion">
          <StubExecutionResults
            v-if="showStubExecutionResults"
            :correlation-id="request.correlationId"
            :stub-execution-results="request.stubExecutionResults"
          />
          <ResponseWriterResults
            v-if="showStubResponseWriterResults"
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
import Accordion from "@/components/bootstrap/Accordion";

export default {
  name: "RequestDetails",
  components: {
    Accordion,
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
        showStubExecutionResults.value || showStubResponseWriterResults.value
    );

    return {
      requestParams,
      requestTime,
      duration,
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
