<template>
  <accordion-item @buttonClicked="showDetails" :opened="accordionOpened">
    <template v-slot:button-text>
      <span>
        <Method :method="overviewRequest.method" />
        <span class="ms-sm-1">{{ overviewRequest.url }}</span>
        <span class="ms-sm-1">
          <span>(</span>
          <span
            class="fw-bold"
            :class="{ 'text-success': executed, 'text-danger': !executed }"
            >{{ executed ? "executed" : "not executed" }}</span
          >
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
            title="Create a stub based on the request parameters of this request"
          >
            Create stub
          </button>
          <button
            class="btn btn-danger btn-sm"
            @click="deleteRequest"
            title="Delete this request"
          >
            Delete
          </button>
        </div>
      </div>
      <RequestDetails :request="request" />
    </template>
  </accordion-item>
</template>

<script>
import { computed, ref, onMounted, onUnmounted } from "vue";
import { useStore } from "vuex";
import toastr from "toastr";
import { formatDateTime, formatFromNow } from "@/utils/datetime";
import { handleHttpError } from "@/utils/error";
import { setIntermediateStub } from "@/utils/session";
import Method from "@/components/request/Method";
import RequestDetails from "@/components/request/RequestDetails";
import { resources } from "@/constants/resources";
import yaml from "js-yaml";
import { useRouter } from "vue-router";

export default {
  name: "Request",
  components: { Method, RequestDetails },
  props: {
    overviewRequest: {
      type: Object,
      required: true,
    },
  },
  setup(props, { emit }) {
    const store = useStore();
    const router = useRouter();

    // Functions
    const getRequestTime = () => props.overviewRequest.requestEndTime;
    const correlationId = () => props.overviewRequest.correlationId;
    const executed = () => props.overviewRequest.executingStubId;
    const getTimeFromNow = () => formatFromNow(getRequestTime());

    // Computed
    const requestDateTime = computed(() => formatDateTime(getRequestTime()));

    // Data
    const timeFromNow = ref(getTimeFromNow());
    const refreshTimeFromNowInterval = ref(null);
    const request = ref({});
    const accordionOpened = ref(false);

    // Lifecycle
    onMounted(() => {
      refreshTimeFromNowInterval.value = setInterval(() => {
        timeFromNow.value = getTimeFromNow();
      }, 60000);
    });
    onUnmounted(() => {
      if (refreshTimeFromNowInterval.value) {
        clearInterval(refreshTimeFromNowInterval.value);
      }
    });

    // Methods
    const showDetails = async () => {
      if (Object.keys(request.value).length === 0) {
        try {
          request.value = await store.dispatch(
            "requests/getRequest",
            correlationId()
          );
          accordionOpened.value = true;
        } catch (e) {
          handleHttpError(e);
        }
      } else {
        accordionOpened.value = !accordionOpened.value;
      }
    };
    const createStub = async () => {
      try {
        const fullStub = await store.dispatch(
          "stubs/createStubBasedOnRequest",
          {
            correlationId: correlationId(),
            doNotCreateStub: true,
          }
        );
        setIntermediateStub(yaml.dump(fullStub.stub));
        await router.push({ name: "StubForm" });
      } catch (e) {
        handleHttpError(e);
      }
    };
    const deleteRequest = async () => {
      try {
        await store.dispatch("requests/deleteRequest", correlationId());
        toastr.success(resources.requestDeletedSuccessfully);
        emit("deleted");
      } catch (e) {
        handleHttpError(e);
      }
    };

    return {
      executed: executed(),
      requestDateTime,
      timeFromNow,
      refreshTimeFromNowInterval,
      request,
      showDetails,
      accordionOpened,
      createStub,
      deleteRequest,
    };
  },
};
</script>

<style scoped></style>
