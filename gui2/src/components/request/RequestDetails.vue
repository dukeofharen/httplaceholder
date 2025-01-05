<script setup lang="ts">
import { onMounted, type PropType, ref } from 'vue'
import type { RequestOverviewModel } from '@/domain/request/request-overview-model.ts'
import type { RequestResultModel } from '@/domain/request/request-result-model.ts'
import { useRequestsStore } from '@/stores/requests.ts'
import { handleHttpError } from '@/utils/error.ts'
import ButtonComponent from '@/components/html-elements/ButtonComponent.vue'

const props = defineProps({
  overviewRequest: {
    type: Object as PropType<RequestOverviewModel>,
    required: true,
  },
})
const requestStore = useRequestsStore()

// Data
const request = ref<RequestResultModel | undefined>()

// Functions
async function createStub() {}
async function exportRequest() {}
async function deleteRequest() {}

// Lifecycle
onMounted(async () => {
  if (!request.value) {
    try {
      request.value = await requestStore.getRequest(props.overviewRequest.correlationId)
    } catch (e) {
      handleHttpError(e)
    }
  }
})
</script>

<template>
  <template v-if="request">
    <div class="flex flex-row gap-1 mt-1">
      <ButtonComponent
        type="success"
        @click="createStub"
        :dense="true"
        :title="$translate('request.createRequestStubTitle')"
      >
        {{ $translate('request.createRequestStub') }}
      </ButtonComponent>
      <ButtonComponent
        type="success"
        @click="exportRequest"
        :dense="true"
        :title="$translate('request.exportRequestTitle')"
      >
        {{ $translate('request.exportRequest') }}
      </ButtonComponent>
      <ButtonComponent
        type="error"
        @click="deleteRequest"
        :dense="true"
        :title="$translate('request.deleteRequestTitle')"
      >
        {{ $translate('request.deleteRequest') }}
      </ButtonComponent>
    </div>
  </template>
</template>

<style scoped></style>
