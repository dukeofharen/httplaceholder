<template>
  <div>
    <h1>{{ $translate('general.requests') }}</h1>
    <div class="col-md-12 mb-3">
      <button type="button" class="btn btn-success me-2 btn-mobile full-width" @click="refresh">
        {{ $translate('general.refresh') }}
      </button>
      <button
        v-if="shouldShowLoadAllRequestsButton"
        type="button"
        class="btn btn-success me-2 btn-mobile full-width"
        @click="loadAllRequests"
      >
        {{ $translate('requests.reloadAllRequests') }}
      </button>
      <button
        type="button"
        class="btn btn-danger btn-mobile full-width"
        @click="showDeleteAllRequestsModal = true"
      >
        {{ $translate('requests.deleteAllRequests') }}
      </button>
      <modal
        :title="$translate('requests.deleteAllRequestsQuestion')"
        :bodyText="$translate('requests.requestsCantBeRecovered')"
        :show-modal="showDeleteAllRequestsModal"
        @close="showDeleteAllRequestsModal = false"
        @yes-click="deleteAllRequests"
      />
    </div>
    <div class="col-md-12 mb-3">
      <div class="input-group mb-3">
        <input
          type="text"
          class="form-control"
          :placeholder="$translate('requests.filterPlaceholder')"
          v-model="filter.urlStubIdFilter"
        />
        <button
          class="btn btn-danger fw-bold"
          type="button"
          :title="$translate('general.reset')"
          @click="filter.urlStubIdFilter = ''"
        >
          <em class="bi-x"></em>
        </button>
      </div>
      <div v-if="tenants.length" class="input-group">
        <select class="form-select" v-model="filter.selectedTenantName">
          <option value="" selected>
            {{ $translate('general.selectStubTenantCategory') }}
          </option>
          <option v-for="tenant of tenants" :key="tenant">{{ tenant }}</option>
        </select>
        <button
          class="btn btn-danger fw-bold"
          type="button"
          :title="$translate('general.reset')"
          @click="filter.selectedTenantName = ''"
        >
          <em class="bi-x"></em>
        </button>
      </div>
    </div>
    <div v-if="showFilterBadges" class="col-md-12 mb-3">
      <span
        class="badge bg-secondary clear-filter me-2"
        v-if="filter.urlStubIdFilter"
        @click="filter.urlStubIdFilter = ''"
        >{{ $translate('requests.filterLabel') }}:
        <strong>{{ filter.urlStubIdFilter }} &times;</strong></span
      >
      <span
        class="badge bg-secondary clear-filter me-2"
        v-if="filter.selectedTenantName"
        @click="filter.selectedTenantName = ''"
        >{{ $translate('general.tenant') }}:
        <strong>{{ filter.selectedTenantName }} &times;</strong></span
      >
    </div>
    <accordion v-if="requests.length">
      <Request
        v-for="request of filteredRequests"
        :key="request.correlationId"
        :overview-request="request"
        @deleted="requestDeleted"
      />
      <accordion-item
        v-if="shouldShowLoadMoreButton"
        :opened="false"
        @buttonClicked="loadMoreRequests"
      >
        <template v-slot:button-text>{{ $translate('requests.loadMoreRequests') }}</template>
      </accordion-item>
    </accordion>
    <div v-else>
      {{ $translate('requests.noRequestsYet') }}
    </div>
  </div>
</template>

<script lang="ts">
import { useRoute } from 'vue-router'
import { computed, onMounted, onUnmounted, ref, watch } from 'vue'
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'
import { handleHttpError } from '@/utils/error'
import { getRequestFilterForm, setRequestFilterForm } from '@/utils/session'
import { success } from '@/utils/toast'
import { useTenantsStore } from '@/store/tenants'
import { useRequestsStore } from '@/store/requests'
import { useSettingsStore } from '@/store/settings'
import { defineComponent } from 'vue'
import type { RequestOverviewModel } from '@/domain/request/request-overview-model'
import type { RequestSavedFilterModel } from '@/domain/request-saved-filter-model'
import { useConfigurationStore } from '@/store/configuration'
import type { ConfigurationModel } from '@/domain/stub/configuration-model'
import { getRootUrl } from '@/utils/config'
import { translate } from '@/utils/translate'

export default defineComponent({
  name: 'Requests',
  setup() {
    const tenantStore = useTenantsStore()
    const requestStore = useRequestsStore()
    const generalStore = useSettingsStore()
    const configStore = useConfigurationStore()
    const route = useRoute()

    // Data
    const requests = ref<RequestOverviewModel[]>([])
    const tenants = ref<string[]>([])
    const showDeleteAllRequestsModal = ref(false)
    const showLoadMoreButton = ref(true)
    let signalrConnection: HubConnection
    let configuration: ConfigurationModel[] = []
    let oldRequestsQueueLength = 0
    const requestsPageSize = generalStore.getRequestsPageSize

    const saveSearchFilters = generalStore.getSaveSearchFilters
    let savedFilter: RequestSavedFilterModel = {
      urlStubIdFilter: '',
      selectedTenantName: '',
    }
    if (saveSearchFilters) {
      savedFilter = getRequestFilterForm()
    }

    const filter = ref<RequestSavedFilterModel>({
      urlStubIdFilter: (route.query.filter as string) || savedFilter?.urlStubIdFilter || '',
      selectedTenantName: (route.query.tenant as string) || savedFilter?.selectedTenantName || '',
    })

    // Functions
    const initializeSignalR = async () => {
      signalrConnection = new HubConnectionBuilder().withUrl(`${getRootUrl()}/requestHub`).build()
      signalrConnection.on('RequestReceived', (request: RequestOverviewModel) => {
        requests.value.unshift(request)

        // Strip away "old" requests.
        if (oldRequestsQueueLength) {
          requests.value = requests.value.slice(0, oldRequestsQueueLength)
        }
      })
      try {
        await signalrConnection.start()
      } catch (err: any) {
        console.log(err.toString())
      }
    }

    // Computed
    const filteredRequests = computed(() => {
      let result = requests.value
      if (filter.value.urlStubIdFilter) {
        const searchTerm = filter.value.urlStubIdFilter.toLowerCase().trim()
        result = result.filter((r) => {
          const stubId = r.executingStubId ? r.executingStubId.toLowerCase() : ''
          const url = r.url.toLowerCase()
          const correlationId = (r.correlationId || '').toLowerCase()
          return (
            (stubId && stubId.includes(searchTerm)) ||
            url.includes(searchTerm) ||
            correlationId === searchTerm
          )
        })
      }

      if (filter.value.selectedTenantName) {
        result = result.filter((r) => r.stubTenant === filter.value.selectedTenantName)
      }

      return result
    })
    const showFilterBadges = computed(
      () => filter.value.urlStubIdFilter || filter.value.selectedTenantName,
    )
    const shouldShowLoadMoreButton = computed(
      () => showLoadMoreButton.value && requestsPageSize > 0,
    )
    const shouldShowLoadAllRequestsButton = computed(() => requestsPageSize > 0)

    // Methods
    const loadRequests = async (fromIdentifier?: string, append?: boolean) => {
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
    const refresh = async () => {
      await loadRequests(undefined, false)
      showLoadMoreButton.value = true
    }
    const loadTenantNames = async () => {
      try {
        tenants.value = await tenantStore.getTenantNames()
        if (!tenants.value.find((t) => t === filter.value.selectedTenantName)) {
          filter.value.selectedTenantName = ''
        }
      } catch (e) {
        handleHttpError(e)
      }
    }
    const deleteAllRequests = async () => {
      try {
        await requestStore.clearRequests()
        success(translate('requests.requestsDeletedSuccessfully'))
        await loadRequests()
      } catch (e) {
        handleHttpError(e)
      }
    }
    const filterChanged = () => {
      if (generalStore.getSaveSearchFilters) {
        setRequestFilterForm(filter.value)
      }
    }
    const loadConfiguration = async () => {
      configuration = await configStore.getConfiguration()
      const foundOldRequestsQueueLength = configuration.find(
        (c) => c.key === 'oldRequestsQueueLength',
      )
      oldRequestsQueueLength = foundOldRequestsQueueLength
        ? parseInt(foundOldRequestsQueueLength.value)
        : 0
    }
    const loadMoreRequests = async () => {
      const lastCorrelationId = requests.value[requests.value.length - 1].correlationId
      await loadRequests(lastCorrelationId, true)
    }
    const requestDeleted = (correlationId: string) => {
      requests.value = requests.value.filter((r) => r.correlationId !== correlationId)
    }
    const loadAllRequests = async () => {
      requests.value = await requestStore.getRequestsOverview()
      showLoadMoreButton.value = false
    }

    // Watch
    watch(filter, () => filterChanged(), { deep: true })

    // Lifecycle
    onMounted(async () => {
      await Promise.all([
        loadRequests(),
        loadTenantNames(),
        initializeSignalR(),
        loadConfiguration(),
      ])
    })
    onUnmounted(() => {
      if (signalrConnection) {
        signalrConnection.stop()
      }
    })

    return {
      requests,
      loadRequests,
      deleteAllRequests,
      filteredRequests,
      tenants,
      filter,
      showDeleteAllRequestsModal,
      showFilterBadges,
      showLoadMoreButton,
      loadMoreRequests,
      requestDeleted,
      refresh,
      shouldShowLoadMoreButton,
      shouldShowLoadAllRequestsButton,
      loadAllRequests,
    }
  },
})
</script>

<style scoped>
.clear-filter {
  cursor: pointer;
  max-width: 100%;
  overflow-x: hidden;
}
</style>
