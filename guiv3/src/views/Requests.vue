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
          placeholder="Filter on stub ID or URL..."
          v-model="urlStubIdFilter"
        />
        <button
          class="btn btn-outline-danger fw-bold"
          type="button"
          title="Reset"
          @click="urlStubIdFilter = ''"
        >
          <em class="bi-x"></em>
        </button>
      </div>
      <div v-if="tenants.length" class="input-group">
        <select class="form-select" v-model="selectedTenantName">
          <option value="" selected>
            Select stub tenant / category name...
          </option>
          <option v-for="tenant of tenants" :key="tenant">{{ tenant }}</option>
        </select>
        <button
          class="btn btn-outline-danger fw-bold"
          type="button"
          title="Reset"
          @click="selectedTenantName = ''"
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
import { computed, onMounted, ref } from "vue";
import Request from "@/components/request/Request";
import { resources } from "@/constants/resources";
import toastr from "toastr";

export default {
  name: "Requests",
  components: { Request },
  setup() {
    const store = useStore();
    const route = useRoute();

    // Data
    const requests = ref([]);
    const tenants = ref([]);
    const urlStubIdFilter = ref(route.query.filter || "");
    const selectedTenantName = ref("");
    const showDeleteAllRequestsModal = ref(false);

    // Computed
    const filteredRequests = computed(() => {
      let result = requests.value;
      if (urlStubIdFilter.value) {
        const searchTerm = urlStubIdFilter.value.toLowerCase().trim();
        result = result.filter((r) => {
          const stubId = r.executingStubId
            ? r.executingStubId.toLowerCase()
            : "";
          const url = r.url.toLowerCase();
          return (
            (stubId && stubId.includes(searchTerm)) || url.includes(searchTerm)
          );
        });
      }

      if (selectedTenantName.value) {
        result = result.filter(
          (r) => r.stubTenant === selectedTenantName.value
        );
      }

      return result;
    });

    // Methods
    const loadRequests = async () => {
      requests.value = await store.dispatch("requests/getRequestsOverview");
    };
    const loadTenantNames = async () => {
      tenants.value = await store.dispatch("tenants/getTenantNames");
    };
    const deleteAllRequests = async () => {
      await store.dispatch("requests/clearRequests");
      toastr.success(resources.requestsDeletedSuccessfully);
      await loadRequests();
    };

    // Lifecycle
    onMounted(
      async () => await Promise.all([loadRequests(), loadTenantNames()])
    );

    return {
      requests,
      loadRequests,
      deleteAllRequests,
      urlStubIdFilter,
      filteredRequests,
      tenants,
      selectedTenantName,
      showDeleteAllRequestsModal,
    };
  },
};
</script>

<style scoped></style>
