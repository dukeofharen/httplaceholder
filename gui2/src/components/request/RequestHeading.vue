<script setup lang="ts">
import RequestMethod from '@/components/request/RequestMethod.vue'
import { computed, onMounted, onUnmounted, type PropType, ref } from 'vue'
import type { RequestOverviewModel } from '@/domain/request/request-overview-model.ts'
import { translate } from '@/utils/translate.ts'
import { formatDateTime, formatFromNow } from '@/utils/datetime.ts'
import { refreshRequestTimesInterval } from '@/constants.ts'

const props = defineProps({
  overviewRequest: {
    type: Object as PropType<RequestOverviewModel>,
    required: true,
  },
})

// Functions
const getTimeFromNow = () => formatFromNow(props.overviewRequest.requestEndTime)

// Data
let refreshTimeFromNowInterval: any
const timeFromNow = ref(getTimeFromNow())

// Computed
const requestDateTime = computed(() => formatDateTime(props.overviewRequest.requestEndTime))
const executed = computed(() => !!props.overviewRequest.executingStubId)
const headingTitle = computed(() =>
  executed.value
    ? `${translate('request.executedStub')}: ${props.overviewRequest.executingStubId}`
    : '',
)

// Lifecycle
onMounted(() => {
  refreshTimeFromNowInterval = setInterval(() => {
    timeFromNow.value = getTimeFromNow()
  }, refreshRequestTimesInterval)
})
onUnmounted(() => {
  if (refreshTimeFromNowInterval) {
    clearInterval(refreshTimeFromNowInterval)
  }
})
</script>

<template>
  <div class="flex flex-col md:flex-row gap-1 md:gap-2" :title="headingTitle">
    <RequestMethod :method="props.overviewRequest.method" />
    <span class="break-words">{{ props.overviewRequest.url }}</span>
    <span>
      <span>(</span>
      <span :class="{ 'text-green-700': executed, 'text-red-700': !executed }">
        {{ translate(executed ? 'request.executed' : 'request.notExecuted') }}
      </span>
      <span>)</span>
      <span> | </span>
      <span :title="requestDateTime">{{ timeFromNow }}</span>
    </span>
  </div>
</template>

<style scoped></style>
