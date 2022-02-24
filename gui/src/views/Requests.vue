<template>
  <div>
    <h1>Requests</h1>
    <div class="col-md-12 mb-3">
      <button
        type="button"
        class="btn btn-success me-2 btn-mobile full-width"
        @click="loadRequests"
      >
        Refresh
      </button>
      <button
        type="button"
        class="btn btn-danger btn-mobile full-width"
        @click="showDeleteAllRequestsModal = true"
      >
        Delete all requests
      </button>
      <modal
        title="Delete all requests?"
        bodyText="The requests can't be recovered."
        :yes-click-function="deleteAllRequests"
        :show-modal="showDeleteAllRequestsModal"
        @close="showDeleteAllRequestsModal = false"
      />
    </div>
    <div class="col-md-12 mb-3">
      <div class="input-group mb-3">
        <input
          type="text"
          class="form-control"
          placeholder="Filter on stub ID, request ID or URL..."
          v-model="filter.urlStubIdFilter"
        />
        <button
          class="btn btn-danger fw-bold"
          type="button"
          title="Reset"
          @click="filter.urlStubIdFilter = ''"
        >
          <em class="bi-x"></em>
        </button>
      </div>
      <div v-if="tenants.length" class="input-group">
        <select class="form-select" v-model="filter.selectedTenantName">
          <option value="" selected>
            Select stub tenant / category name...
          </option>
          <option v-for="tenant of tenants" :key="tenant">{{ tenant }}</option>
        </select>
        <button
          class="btn btn-danger fw-bold"
          type="button"
          title="Reset"
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
        >Stub ID / req.ID / URL:
        <strong>{{ filter.urlStubIdFilter }} &times;</strong></span
      >
      <span
        class="badge bg-secondary clear-filter me-2"
        v-if="filter.selectedTenantName"
        @click="filter.selectedTenantName = ''"
        >Tenant: <strong>{{ filter.selectedTenantName }} &times;</strong></span
      >
    </div>
    <accordion v-if="requests.length">
      <Request
        v-for="request of filteredRequests"
        :key="request.correlationId"
        :overview-request="request"
        @deleted="loadRequests"
      />
    </accordion>
    <div v-else>
      No requests have been made to HttPlaceholder yet. Perform HTTP requests
      and you will see the requests appearing on this page.
    </div>
  </div>
</template>

<script lang="ts">
import { useRoute } from "vue-router";
import { computed, onMounted, onUnmounted, ref, watch } from "vue";
import Request from "@/components/request/Request.vue";
import { resources } from "@/constants/resources";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { handleHttpError } from "@/utils/error";
import { getRequestFilterForm, setRequestFilterForm } from "@/utils/session";
import { success } from "@/utils/toast";
import { useTenantsStore } from "@/store/tenants";
import { useRequestsStore } from "@/store/requests";
import { useGeneralStore } from "@/store/general";
import { defineComponent } from "vue";
import type { RequestOverviewModel } from "@/domain/request/request-overview-model";
import type { RequestSavedFilterModel } from "@/domain/request-saved-filter-model";

export default defineComponent({
  name: "Requests",
  components: { Request },
  setup() {
    const tenantStore = useTenantsStore();
    const requestStore = useRequestsStore();
    const generalStore = useGeneralStore();
    const route = useRoute();

    // Data
    const requests = ref<RequestOverviewModel[]>([]);
    const tenants = ref<string[]>([]);
    const showDeleteAllRequestsModal = ref(false);
    let signalrConnection: HubConnection;

    const saveSearchFilters = generalStore.getSaveSearchFilters;
    let savedFilter: RequestSavedFilterModel = {
      urlStubIdFilter: "",
      selectedTenantName: "",
    };
    if (saveSearchFilters) {
      savedFilter = getRequestFilterForm() || {};
    }

    const filter = ref<RequestSavedFilterModel>({
      urlStubIdFilter:
        (route.query.filter as string) || savedFilter.urlStubIdFilter || "",
      selectedTenantName:
        (route.query.tenant as string) || savedFilter.selectedTenantName || "",
    });

    // Functions
    const initializeSignalR = async () => {
      signalrConnection = new HubConnectionBuilder()
        .withUrl("/requestHub")
        .build();
      signalrConnection.on("RequestReceived", (request: RequestOverviewModel) =>
        requests.value.unshift(request)
      );
      try {
        await signalrConnection.start();
      } catch (err: any) {
        console.log(err.toString());
      }
    };

    // Computed
    const filteredRequests = computed(() => {
      let result = requests.value;
      if (filter.value.urlStubIdFilter) {
        const searchTerm = filter.value.urlStubIdFilter.toLowerCase().trim();
        result = result.filter((r) => {
          const stubId = r.executingStubId
            ? r.executingStubId.toLowerCase()
            : "";
          const url = r.url.toLowerCase();
          const correlationId = (r.correlationId || "").toLowerCase();
          return (
            (stubId && stubId.includes(searchTerm)) ||
            url.includes(searchTerm) ||
            correlationId === searchTerm
          );
        });
      }

      if (filter.value.selectedTenantName) {
        result = result.filter(
          (r) => r.stubTenant === filter.value.selectedTenantName
        );
      }

      return result;
    });
    const showFilterBadges = computed(
      () => filter.value.urlStubIdFilter || filter.value.selectedTenantName
    );

    // Methods
    const loadRequests = async () => {
      try {
        requests.value = await requestStore.getRequestsOverview();
      } catch (e) {
        handleHttpError(e);
      }
    };
    const loadTenantNames = async () => {
      try {
        tenants.value = await tenantStore.getTenantNames();
        if (!tenants.value.find((t) => t === filter.value.selectedTenantName)) {
          filter.value.selectedTenantName = "";
        }
      } catch (e) {
        handleHttpError(e);
      }
    };
    const deleteAllRequests = async () => {
      try {
        await requestStore.clearRequests();
        success(resources.requestsDeletedSuccessfully);
        await loadRequests();
      } catch (e) {
        handleHttpError(e);
      }
    };
    const filterChanged = () => {
      if (generalStore.getSaveSearchFilters) {
        setRequestFilterForm(filter.value);
      }
    };

    // Watch
    watch(filter, () => filterChanged(), { deep: true });

    // Lifecycle
    onMounted(async () => {
      await Promise.all([
        loadRequests(),
        loadTenantNames(),
        initializeSignalR(),
      ]);
    });
    onUnmounted(() => {
      if (signalrConnection) {
        signalrConnection.stop();
      }
    });

    return {
      requests,
      loadRequests,
      deleteAllRequests,
      filteredRequests,
      tenants,
      filter,
      showDeleteAllRequestsModal,
      showFilterBadges,
    };
  },
});
</script>

<style scoped>
.clear-filter {
  cursor: pointer;
  max-width: 100%;
  overflow-x: hidden;
}
</style>
