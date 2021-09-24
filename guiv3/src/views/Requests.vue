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
    <div class="accordion" :id="accordionId">
      <Request
        v-for="request of requests"
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
import { onMounted, ref } from "vue";
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

    // Methods
    const loadRequests = async () => {
      requests.value = await store.dispatch("requests/getRequestsOverview");
    };
    const deleteAllRequests = async () => {
      await store.dispatch("requests/clearRequests");
      toastr.success(resources.requestsDeletedSuccessfully);
      await loadRequests();
    };

    // Lifecycle
    onMounted(async () => await loadRequests());

    return { requests, accordionId, loadRequests, deleteAllRequests };
  },
};
</script>

<style scoped></style>
