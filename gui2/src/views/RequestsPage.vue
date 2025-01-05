<script setup lang="ts">
import H1Tag from '@/components/html-elements/H1Tag.vue'
import ButtonComponent from '@/components/html-elements/ButtonComponent.vue'
import { ArrowDownOnSquareStackIcon, ArrowPathIcon, TrashIcon } from '@heroicons/vue/24/outline'
import { useSettingsStore } from '@/stores/settings'
import { useRequestsStore } from '@/stores/requests'
import { useTenantsStore } from '@/stores/tenants'
import { computed, onMounted, ref } from 'vue'
import { handleHttpError } from '@/utils/error'
import { useConfiguration } from '@/composables/useConfiguration'
import ModalComponent from '@/components/modal/ModalComponent.vue'
import { success } from '@/utils/toast.ts'
import { translate } from '@/utils/translate.ts'
import TextInput from '@/components/html-elements/TextInput.vue'
import { useFilters } from '@/composables/useFilters.ts'
import { useSignalR } from '@/composables/useSignalR.ts'
import type { RequestOverviewModel } from '@/domain/request/request-overview-model.ts'
import SelectInput from '@/components/html-elements/SelectInput.vue'
import type { SelectListItem } from '@/domain/ui/select-list-item.ts'
import RequestComponent from '@/components/request/RequestComponent.vue'

const tenantStore = useTenantsStore()
const requestStore = useRequestsStore()
const settingsStore = useSettingsStore()
const { getOldRequestsQueueLength } = useConfiguration()
const { requests, filteredRequests, filter } = useFilters()
useSignalR('/requestHub', 'RequestReceived', onRequestReceived)

// Data
const tenants = ref<string[]>([])
const shouldShowDeleteAllRequestsModal = ref(false)
const requestsPageSize = settingsStore.getRequestsPageSize
const showLoadMoreButton = ref(true)

// Computed
// const shouldShowLoadMoreButton = computed(() => showLoadMoreButton.value && requestsPageSize > 0)
const shouldShowLoadAllRequestsButton = computed(() => requestsPageSize > 0)
const tenantSelectItems = computed<SelectListItem[]>(() =>
  tenants.value.map(
    (t) =>
      <SelectListItem>{
        label: t,
        value: t,
      },
  ),
)

// Functions
async function refresh() {
  await loadRequests(undefined, false)
  showLoadMoreButton.value = true
}

async function loadAllRequests() {
  requests.value = await requestStore.getRequestsOverview()
  showLoadMoreButton.value = false
}

function showDeleteAllRequestsModal() {
  shouldShowDeleteAllRequestsModal.value = true
}

async function loadRequests(fromIdentifier?: string, append?: boolean) {
  try {
    const result = await requestStore.getRequestsOverview(fromIdentifier, requestsPageSize)
    if (append) {
      requests.value = requests.value.concat(result.slice(1))
    } else {
      requests.value = result
    }

    showLoadMoreButton.value = result.length >= requestsPageSize
  } catch (e) {
    handleHttpError(e)
  }
}

async function loadTenantNames() {
  try {
    tenants.value = await tenantStore.getTenantNames()
    if (!tenants.value.find((t) => t === filter.value.selectedTenantName)) {
      filter.value.selectedTenantName = ''
    }
  } catch (e) {
    handleHttpError(e)
  }
}

function onRequestReceived(request: RequestOverviewModel) {
  requests.value.unshift(request)

  // Strip away "old" requests.
  if (getOldRequestsQueueLength.value) {
    requests.value = requests.value.slice(0, getOldRequestsQueueLength.value)
  }
}

async function deleteAllRequests() {
  try {
    await requestStore.clearRequests()
    success(translate('requests.requestsDeletedSuccessfully'))
    await loadRequests()
  } catch (e) {
    handleHttpError(e)
  }
}

// Lifecycle
onMounted(async () => {
  await Promise.all([loadRequests(), loadTenantNames()])
})
</script>

<template>
  <H1Tag>{{ $translate('general.requests') }}</H1Tag>
  <div class="flex justify-start gap-2 flex-wrap mb-2">
    <ButtonComponent type="success" @click="refresh" :dense="true">
      <ArrowPathIcon class="size-6" />
      <span>{{ $translate('general.refresh') }}</span>
    </ButtonComponent>
    <ButtonComponent v-if="shouldShowLoadAllRequestsButton" type="success" @click="loadAllRequests">
      <ArrowDownOnSquareStackIcon class="size-6" />
      <span>{{ $translate('requests.reloadAllRequests') }}</span>
    </ButtonComponent>
    <ButtonComponent type="error" @click="showDeleteAllRequestsModal">
      <TrashIcon class="size-6" />
      <span>{{ $translate('requests.deleteAllRequests') }}</span>
    </ButtonComponent>
    <ModalComponent
      :title="$translate('requests.deleteAllRequestsQuestion')"
      v-model:show-modal="shouldShowDeleteAllRequestsModal"
      :yes-click-function="deleteAllRequests"
    >
      {{ $translate('requests.requestsCantBeRecovered') }}
    </ModalComponent>
  </div>
  <div class="flex flex-col md:flex-row gap-2 w-full md:w-1/2 mb-2">
    <TextInput
      id="urlStubIdFilter"
      :placeholder="$translate('requests.filterPlaceholder')"
      :support-clearing="true"
      v-model="filter.filter"
    />
    <SelectInput
      v-if="tenants.length"
      :items="tenantSelectItems"
      id="selectedTenantName"
      :placeholder="$translate('general.selectStubTenantCategory')"
      :support-clearing="true"
      v-model="filter.selectedTenantName"
    />
  </div>
  <div>
    <RequestComponent
      v-for="request of filteredRequests"
      :overview-request="request"
      :key="request.correlationId"
    />
  </div>
</template>

<style scoped></style>
