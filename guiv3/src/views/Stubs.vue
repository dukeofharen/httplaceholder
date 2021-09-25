<template>
  <div>
    <h1>Stubs</h1>

    <div class="col-md-12 mb-3">
      <button type="button" class="btn btn-success me-2" @click="loadData">
        Refresh
      </button>
      <button
        type="button"
        class="btn btn-danger"
        @click="showDeleteAllStubsModal = true"
      >
        Delete all stubs
      </button>
      <modal
        title="Delete all stubs?"
        bodyText="The stubs can't be recovered."
        :yes-click-function="deleteAllStubs"
        :show-modal="showDeleteAllStubsModal"
        @close="showDeleteAllStubsModal = false"
      />
    </div>

    <div class="accordion" :id="accordionId">
      <Stub
        v-for="stub of filteredStubs"
        :key="stub.stub.id"
        :overview-stub="stub"
        :accordion-id="accordionId"
        @deleted="loadData"
      />
    </div>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { computed, onMounted, ref } from "vue";
import Stub from "@/components/stub/Stub";
import toastr from "toastr";
import { resources } from "@/constants/resources";

export default {
  name: "Stubs",
  components: { Stub },
  setup() {
    const store = useStore();

    // Data
    const accordionId = "stubs-accordion";
    const stubs = ref([]);
    const showDeleteAllStubsModal = ref(false);

    // Computed
    const filteredStubs = computed(() => {
      let stubsResult = stubs.value;
      const compare = (a, b) => {
        if (a.stub.id < b.stub.id) return -1;
        if (a.stub.id > b.stub.id) return 1;
        return 0;
      };

      stubsResult.sort(compare);
      return stubsResult;
    });

    // Methods
    const loadStubs = async () => {
      stubs.value = await store.dispatch("stubs/getStubsOverview");
    };
    const loadData = async () => {
      await Promise.all([loadStubs()]);
    };
    const deleteAllStubs = async () => {
      await store.dispatch("stubs/deleteStubs");
      toastr.success(resources.stubsDeletedSuccessfully);
      await loadData();
    };

    // Lifecycle
    onMounted(async () => await loadData());

    return {
      accordionId,
      stubs,
      filteredStubs,
      loadData,
      showDeleteAllStubsModal,
      deleteAllStubs,
    };
  },
};
</script>

<style scoped></style>
