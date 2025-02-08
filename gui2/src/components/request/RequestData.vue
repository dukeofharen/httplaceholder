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

const trClass = 'hover:bg-gray-100 dark:hover:bg-gray-700'
const tdClass = 'py-1 px-2 text-sm font-medium text-gray-900 whitespace-nowrap dark:text-white'

// Computed
const requestParams = computed(() => props.request.requestParameters)
const requestTime = computed(() => formatDateTime(props.request?.requestEndTime))
const duration = computed(() => {
  const req = props.request
  return getDuration(req?.requestBeginTime, req?.requestEndTime)
})
</script>

<template>
  <div class="max-w-full overflow-x-scroll">
    <table class="min-w-full max-w-full divide-y divide-gray-200 table-fixed dark:divide-gray-700">
      <tbody class="bg-white divide-y divide-gray-200 dark:bg-gray-800 dark:divide-gray-700">
        <tr :class="trClass">
          <td :class="tdClass">
            {{ $translate('request.url') }}
          </td>
          <td :class="tdClass">
            {{ requestParams.url }}
          </td>
        </tr>
        <tr :class="trClass">
          <td :class="tdClass">
            {{ $translate('request.clientIp') }}
          </td>
          <td :class="tdClass">
            {{ requestParams.clientIp }}
          </td>
        </tr>
        <tr :class="trClass">
          <td :class="tdClass">
            {{ $translate('request.correlationId') }}
          </td>
          <td :class="tdClass">
            {{ props.request.correlationId }}
          </td>
        </tr>
        <tr :class="trClass">
          <td :class="tdClass">
            {{ $translate('request.executedStub') }}
          </td>
          <td :class="tdClass">
            <router-link :to="{ name: 'Stubs', query: { tenant: props.request.executingStubId } }"
              >{{ props.request.executingStubId }}
            </router-link>
          </td>
        </tr>
        <tr :class="trClass">
          <td :class="tdClass">
            {{ $translate('request.stubTenant') }}
          </td>
          <td :class="tdClass">
            <router-link :to="{ name: 'Stubs', query: { tenant: props.request.stubTenant } }"
              >{{ props.request.stubTenant }}
            </router-link>
          </td>
        </tr>
        <tr :class="trClass">
          <td :class="tdClass">
            {{ $translate('request.requestTime') }}
          </td>
          <td :class="tdClass">
            <span
              >{{ requestTime }} ({{ $vsprintf($translate('request.itTookMs'), [duration]) }})</span
            >
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped></style>
