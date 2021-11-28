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
        <div class="accordion">
          <RequestHeaders :request="request" />
          <QueryParams v-if="showQueryParameters" :request="request" />
        </div>
      </div>
      <div class="col-md-12 mb-3" v-if="showRequestBody">
        <label>Request body</label>
        <RequestBody :request="request" />
      </div>
      <div v-if="showResults" class="col-md-12">
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
import RequestBody from "@/components/request/RequestBody";

export default {
  name: "RequestDetails",
  components: {
    RequestBody,
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
      formatDateTime(props.request.requestEndTime)
    );
    const duration = computed(() => {
      const req = props.request;
      return getDuration(req.requestBeginTime, req.requestEndTime);
    });
    const showQueryParameters = computed(
      () => requestParams.value?.url?.includes("?") || false
    );
    const showRequestBody = computed(() => requestParams.value?.body || false);
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
      showRequestBody,
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