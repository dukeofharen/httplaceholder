<template>
  <div>
    <div class="row">
      <div class="col-md-12 mb-3">
        <label>{{ $translate('request.url') }}</label>
        <pre class="request-url"><code>{{ requestParams?.url }}</code></pre>
      </div>
      <div class="col-md-12 mb-3">
        <label>{{ $translate('request.clientIp') }}</label>
        <span>{{ requestParams?.clientIp }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>{{ $translate('request.correlationId') }}</label>
        <span>{{ request?.correlationId }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>{{ $translate('request.executedStub') }}</label>
        <router-link :to="{ name: 'Stubs', query: { filter: request?.executingStubId } }"
          >{{ request?.executingStubId }}
        </router-link>
      </div>
      <div class="col-md-12 mb-3">
        <label>{{ $translate('request.stubTenant') }}</label>
        <router-link :to="{ name: 'Stubs', query: { tenant: request?.stubTenant } }"
          >{{ request?.stubTenant }}
        </router-link>
      </div>
      <div class="col-md-12 mb-3">
        <label>{{ $translate('request.requestTime') }}</label>
        <span>{{ requestTime }} ({{ $vsprintf($translate('request.itTookMs'), [duration]) }})</span>
      </div>
      <div class="col-md-12 mb-3">
        <div class="accordion">
          <RequestHeaders :request="request" />
          <QueryParams v-if="showQueryParameters" :request="request" />
          <RequestResponse :request="request" />
        </div>
      </div>
      <div class="col-md-12 mb-3" v-if="showRequestBody">
        <label>{{ $translate('request.requestBody') }}</label>
        <RequestResponseBody :render-model="bodyRenderModel" />
      </div>
      <div v-if="showResults" class="col-md-12">
        <div class="accordion">
          <StubExecutionResults
            v-if="showStubExecutionResults"
            :correlation-id="request?.correlationId"
            :stub-execution-results="request?.stubExecutionResults"
          />
          <ResponseWriterResults
            v-if="showStubResponseWriterResults"
            :correlation-id="request?.correlationId"
            :stub-response-writer-results="request?.stubResponseWriterResults"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, type PropType } from 'vue'
import { formatDateTime, getDuration } from '@/utils/datetime'
import RequestHeaders from '@/components/request/RequestHeaders.vue'
import QueryParams from '@/components/request/QueryParams.vue'
import RequestResponse from '@/components/request/RequestResponse.vue'
import StubExecutionResults from '@/components/request/StubExecutionResults.vue'
import ResponseWriterResults from '@/components/request/ResponseWriterResults.vue'
import { defineComponent } from 'vue'
import type { RequestResultModel } from '@/domain/request/request-result-model'
import RequestResponseBody from '@/components/request/body/RequestResponseBody.vue'
import type { RequestResponseBodyRenderModel } from '@/domain/request/request-response-body-render-model'

export default defineComponent({
  name: 'RequestDetails',
  components: {
    RequestResponseBody,
    StubExecutionResults,
    RequestHeaders,
    QueryParams,
    ResponseWriterResults,
    RequestResponse,
  },
  props: {
    request: {
      type: Object as PropType<RequestResultModel>,
      required: true,
    },
  },
  setup(props) {
    // Computed
    const requestParams = computed(() => props.request?.requestParameters)
    const requestTime = computed(() => formatDateTime(props.request?.requestEndTime))
    const duration = computed(() => {
      const req = props.request
      return getDuration(req?.requestBeginTime, req?.requestEndTime)
    })
    const showQueryParameters = computed(() => requestParams.value?.url?.includes('?') || false)
    const showRequestBody = computed(() => !!requestParams.value?.body || false)
    const showStubExecutionResults = computed(
      () => props.request?.stubExecutionResults && props.request?.stubExecutionResults.length,
    )
    const showStubResponseWriterResults = computed(
      () =>
        props.request?.stubResponseWriterResults && props.request?.stubResponseWriterResults.length,
    )
    const showResults = computed(
      () => showStubExecutionResults.value || showStubResponseWriterResults.value,
    )
    const bodyRenderModel = computed<RequestResponseBodyRenderModel>(() => {
      return {
        body: props.request.requestParameters.body,
        bodyIsBinary: props.request.requestParameters.bodyIsBinary,
        headers: props.request.requestParameters.headers,
        base64DecodeNotBinary: false,
      }
    })

    return {
      requestParams,
      requestTime,
      duration,
      showQueryParameters,
      showStubExecutionResults,
      showStubResponseWriterResults,
      showResults,
      showRequestBody,
      bodyRenderModel,
    }
  },
})
</script>

<style lang="scss" scoped>
label {
  display: block;
  font-weight: bold;
}

.request-url {
  margin-bottom: 0;
}
</style>
