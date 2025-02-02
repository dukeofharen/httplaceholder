<script setup lang="ts">
import { computed, type PropType } from 'vue'
import type { RequestResultModel } from '@/domain/request/request-result-model.ts'
import { formatDateTime, getDuration } from '@/utils/datetime.ts'

const props = defineProps({
  request: {
    type: Object as PropType<RequestResultModel>,
    required: true,
  },
})

// Computed
const requestParams = computed(() => props.request.requestParameters)
const requestTime = computed(() => formatDateTime(props.request?.requestEndTime))
const duration = computed(() => {
  const req = props.request
  return getDuration(req?.requestBeginTime, req?.requestEndTime)
})
</script>

<template>
  <div>
    <table>
      <tr>
        <td>{{ $translate('request.url') }}</td>
        <td>{{ requestParams.url }}</td>
      </tr>
      <tr>
        <td>{{ $translate('request.clientIp') }}</td>
        <td>{{ requestParams.clientIp }}</td>
      </tr>
      <tr>
        <td>{{ $translate('request.correlationId') }}</td>
        <td>{{ props.request.correlationId }}</td>
      </tr>
      <tr>
        <td>{{ $translate('request.executedStub') }}</td>
        <td>
          <router-link :to="{ name: 'Stubs', query: { tenant: props.request.executingStubId } }">{{
            props.request.executingStubId
          }}</router-link>
        </td>
      </tr>
      <tr>
        <td>{{ $translate('request.stubTenant') }}</td>
        <td>
          <router-link :to="{ name: 'Stubs', query: { tenant: props.request.stubTenant } }">{{
            props.request.stubTenant
          }}</router-link>
        </td>
      </tr>
      <tr>
        <td>{{ $translate('request.requestTime') }}</td>
        <td>
          <span
            >{{ requestTime }} ({{ $vsprintf($translate('request.itTookMs'), [duration]) }})</span
          >
        </td>
      </tr>
    </table>
  </div>
</template>

<style scoped></style>
