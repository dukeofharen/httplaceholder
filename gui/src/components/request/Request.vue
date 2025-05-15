<template>
  <accordion-item @opened="showDetails">
    <template v-slot:button-text>
      <span
        class="request-header"
        :title="executed ? `${$translate('request.executedStub')}: ` + executingStubId : ''"
      >
        <Method v-if="overviewRequest.method" :method="overviewRequest.method" />
        <span class="ms-sm-1 request-url">{{ overviewRequest.url }}</span>
        <span class="ms-sm-1">
          <span>(</span>
          <span class="fw-bold" :class="{ 'text-success': executed, 'text-danger': !executed }">{{
            executed ? 'executed' : 'not executed'
          }}</span>
          <span>&nbsp;|&nbsp;</span>
          <span :title="requestDateTime">{{ timeFromNow }}</span>
          <span>)</span></span
        >
      </span>
    </template>
    <template v-slot:accordion-body>
      <div class="row mb-3">
        <div class="col-md-12">
          <button
            class="btn btn-success btn-sm me-2"
            @click="createStub"
            :title="$translate('request.createRequestStubTitle')"
          >
            {{ $translate('request.createRequestStub') }}
          </button>
          <button
            class="btn btn-success btn-sm me-2"
            @click="exportRequest"
            :title="$translate('request.exportRequestTitle')"
          >
            {{ $translate('request.exportRequest') }}
          </button>
          <button
            class="btn btn-danger btn-sm"
            @click="doDeleteRequest"
            :title="$translate('request.deleteRequestTitle')"
          >
            {{ $translate('request.deleteRequest') }}
          </button>
        </div>
      </div>
      <RequestExport v-if="exportRequestOpened" :request="request" />
      <RequestDetails :request="request" />
    </template>
  </accordion-item>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { formatDateTime, formatFromNow } from '@/utils/datetime'
import { handleHttpError } from '@/utils/error'
import { setIntermediateStub } from '@/utils/session'
import yaml from 'js-yaml'
import { useRouter } from 'vue-router'
import { success } from '@/utils/toast'
import { useStubsStore } from '@/store/stubs'
import { useRequestsStore } from '@/store/requests'
import type { RequestOverviewModel } from '@/domain/request/request-overview-model'
import type { RequestResultModel } from '@/domain/request/request-result-model'
import { getDefaultRequestResultModel } from '@/domain/request/request-result-model'
import { refreshRequestTimesInterval } from '@/constants'
import { translate } from '@/utils/translate'

export type RequestProps = {
  overviewRequest: RequestOverviewModel
}
const props = defineProps<RequestProps>()
const emit = defineEmits<{ deleted: [correlationId: string] }>()

const { createStubBasedOnRequest } = useStubsStore()
const { getRequest, deleteRequest } = useRequestsStore()
const router = useRouter()

// Functions
const getRequestTime = () => props.overviewRequest.requestEndTime
const correlationId = () => props.overviewRequest.correlationId
const getTimeFromNow = () => formatFromNow(getRequestTime())

// Computed
const requestDateTime = computed(() => formatDateTime(getRequestTime()))
const executed = computed(() => !!props.overviewRequest.executingStubId)
const executingStubId = computed(() => props.overviewRequest.executingStubId)

// Data
const timeFromNow = ref(getTimeFromNow())
let refreshTimeFromNowInterval: any
const request = ref<RequestResultModel>(getDefaultRequestResultModel())
const exportRequestOpened = ref(false)

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

// Methods
async function showDetails(){
  if (!request.value.correlationId) {
    try {
      request.value = await getRequest(correlationId())
    } catch (e) {
      handleHttpError(e)
    }
  }
}
async function createStub() {
  try {
    const fullStub = await createStubBasedOnRequest({
      correlationId: correlationId(),
      doNotCreateStub: true,
    })
    setIntermediateStub(yaml.dump(fullStub.stub))
    await router.push({ name: 'StubForm' })
  } catch (e) {
    handleHttpError(e)
  }
}

async function doDeleteRequest() {
  try {
    await deleteRequest(correlationId())
    success(translate('request.requestDeletedSuccessfully'))
    emit('deleted', correlationId())
  } catch (e) {
    handleHttpError(e)
  }
}

const exportRequest = async () => {
  exportRequestOpened.value = !exportRequestOpened.value
}
</script>

<style scoped>
.request-header {
  width: 100%;
}

.request-url {
  overflow-wrap: break-word;
  width: 100%;
}
</style>
