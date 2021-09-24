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
        data-bs-toggle="modal"
        data-bs-target="#deleteRequestsModal"
      >
        Delete all requests
      </button>
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
    <div class="accordion" :id="accordionId">
      <Request
        v-for="request of filteredRequests"
        :key="request.correlationId"
        :overview-request="request"
        :accordion-id="accordionId"
      />
    </div>
    <div
      class="modal fade"
      id="deleteRequestsModal"
      tabindex="-1"
      aria-labelledby="deleteRequestsModalLabel"
      aria-hidden="true"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="deleteRequestsModalLabel">
              Delete all requests?
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">The requests can't be recovered.</div>
          <div class="modal-footer">
            <button type="button" class="btn" data-bs-dismiss="modal">
              No
            </button>
            <button
              type="button"
              class="btn btn-danger"
              @click="deleteAllRequests"
              data-bs-dismiss="modal"
            >
              Yes
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { computed, onMounted, ref } from "vue";
import Request from "@/components/request/Request";
import { resources } from "@/constants/resources";
import toastr from "toastr";

export default {
  name: "Requests",
  components: { Request },
  setup() {
    const store = useStore();

    // Data
    const accordionId = "requests-accordion";
    const requests = ref([]);
    const tenants = ref([]);
    const urlStubIdFilter = ref("");
    const selectedTenantName = ref("");

    // Computed
    const filteredRequests = computed(() => {
      let result = requests.value;
      if (urlStubIdFilter.value) {
        const searchTerm = urlStubIdFilter.value.toLowerCase();
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
      accordionId,
      loadRequests,
      deleteAllRequests,
      urlStubIdFilter,
      filteredRequests,
      tenants,
      selectedTenantName,
    };
  },
};
</script>

<style scoped></style>
