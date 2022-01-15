<template>
  <div>
    <h1>Requests</h1>
    <div class="col-md-12 mb-3">
      <button type="button" class="btn btn-success me-2" @click="loadRequests">
        Refresh
      </button>
      <button
        type="button"
        class="btn btn-danger"
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
    <accordion>
      <Request
        v-for="request of filteredRequests"
        :key="request.correlationId"
        :overview-request="request"
        @deleted="loadRequests"
      />
    </accordion>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { useRoute } from "vue-router";
import { computed, onMounted, onUnmounted, ref, watch } from "vue";
import Request from "@/components/request/Request";
import { resources } from "@/constants/resources";
import toastr from "toastr";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { handleHttpError } from "@/utils/error";
import { getRequestFilterForm, setRequestFilterForm } from "@/utils/session";

export default {
  name: "Requests",
  components: { Request },
  setup() {
    const store = useStore();
    const route = useRoute();

    // Data
    const requests = ref([]);
    const tenants = ref([]);
    const showDeleteAllRequestsModal = ref(false);
    let signalrConnection = null;

    const saveSearchFilters = store.getters["general/getSaveSearchFilters"];
    let savedFilter = {};
    if (saveSearchFilters) {
      savedFilter = getRequestFilterForm() || {};
    }

    const filter = ref({
      urlStubIdFilter: route.query.filter || savedFilter.urlStubIdFilter || "",
      selectedTenantName:
        route.query.tenant || savedFilter.selectedTenantName || "",
    });

    // Functions
    const initializeSignalR = () => {
      signalrConnection = new HubConnectionBuilder()
        .withUrl("/requestHub")
        .build();
      signalrConnection.on("RequestReceived", (request) =>
        requests.value.unshift(request)
      );
      signalrConnection
        .start()
        .then(() => {})
        .catch((err) => console.log(err.toString()));
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

    // Methods
    const loadRequests = async () => {
      try {
        requests.value = await store.dispatch("requests/getRequestsOverview");
      } catch (e) {
        handleHttpError(e);
      }
    };
    const loadTenantNames = async () => {
      try {
        tenants.value = await store.dispatch("tenants/getTenantNames");
        if (!tenants.value.find((t) => t === filter.value.selectedTenantName)) {
          filter.value.selectedTenantName = "";
        }
      } catch (e) {
        handleHttpError(e);
      }
    };
    const deleteAllRequests = async () => {
      try {
        await store.dispatch("requests/clearRequests");
        toastr.success(resources.requestsDeletedSuccessfully);
        await loadRequests();
      } catch (e) {
        handleHttpError(e);
      }
    };
    const filterChanged = () => {
      if (store.getters["general/getSaveSearchFilters"]) {
        setRequestFilterForm(filter.value);
      }
    };

    // Watch
    watch(filter, () => filterChanged(), { deep: true });

    // Lifecycle
    onMounted(async () => {
      await Promise.all([loadRequests(), loadTenantNames()]);
      initializeSignalR();
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
    };
  },
};
</script>

<style scoped></style>
