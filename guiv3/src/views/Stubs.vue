<template>
  <div>
    <h1>Stubs</h1>

    <div class="col-md-12 mb-3">
      <button type="button" class="btn btn-success me-2" @click="loadData">
        Refresh
      </button>
      <router-link :to="{ name: 'StubForm' }" class="btn btn-success me-2"
        >Add stubs</router-link
      >
      <button
        class="btn btn-success me-2"
        @click="download"
        title="Download the (filtered) stubs as YAML file."
      >
        Download stubs as YAML
      </button>
      <UploadStubs class="me-2" @uploaded="loadData" />
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

    <div class="col-md-12 mb-3">
      <div class="input-group mb-3">
        <input
          type="text"
          class="form-control"
          placeholder="Filter on stub ID or URL..."
          v-model="urlStubIdFilter"
        />
        <button
          class="btn btn-danger fw-bold"
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
          class="btn btn-danger fw-bold"
          type="button"
          title="Reset"
          @click="selectedTenantName = ''"
        >
          <em class="bi-x"></em>
        </button>
      </div>
    </div>

    <accordion>
      <Stub
        v-for="stub of filteredStubs"
        :key="stub.stub.id"
        :overview-stub="stub"
        @deleted="loadData"
      />
    </accordion>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { useRoute } from "vue-router";
import { computed, onMounted, ref } from "vue";
import Stub from "@/components/stub/Stub";
import toastr from "toastr";
import { resources } from "@/constants/resources";
import yaml from "js-yaml";
import { handleHttpError } from "@/utils/error";
import { downloadBlob } from "@/utils/download";
import UploadStubs from "@/components/stub/UploadStubs";

export default {
  name: "Stubs",
  components: { Stub, UploadStubs },
  setup() {
    const store = useStore();
    const route = useRoute();

    // Data
    const stubs = ref([]);
    const showDeleteAllStubsModal = ref(false);
    const tenants = ref([]);
    const urlStubIdFilter = ref(route.query.filter || "");
    const selectedTenantName = ref(route.query.tenant || "");

    // Functions
    const filterStubs = (input) => {
      let stubsResult = input;
      const compare = (a, b) => {
        if (a.stub.id < b.stub.id) return -1;
        if (a.stub.id > b.stub.id) return 1;
        return 0;
      };

      if (urlStubIdFilter.value) {
        stubsResult = stubsResult.filter((s) => {
          const stubId = s.stub.id.toLowerCase();
          return stubId && stubId.includes(urlStubIdFilter.value);
        });
      }

      if (selectedTenantName.value) {
        stubsResult = stubsResult.filter(
          (s) => s.stub.tenant === selectedTenantName.value
        );
      }

      stubsResult.sort(compare);
      return stubsResult;
    };

    // Computed
    const filteredStubs = computed(() => filterStubs(stubs.value));

    // Methods
    const loadStubs = async () => {
      stubs.value = await store.dispatch("stubs/getStubsOverview");
    };
    const loadTenantNames = async () => {
      tenants.value = await store.dispatch("tenants/getTenantNames");
    };
    const loadData = async () => {
      await Promise.all([loadStubs(), loadTenantNames()]);
    };
    const deleteAllStubs = async () => {
      await store.dispatch("stubs/deleteStubs");
      toastr.success(resources.stubsDeletedSuccessfully);
      await loadData();
    };
    const download = async () => {
      try {
        const stubs = filterStubs(await store.dispatch("stubs/getStubs")).map(
          (fs) => fs.stub
        );
        const downloadString = `${resources.downloadStubsHeader}\n${yaml.dump(
          stubs
        )}`;
        downloadBlob("stubs.yml", downloadString);
      } catch (e) {
        handleHttpError(e);
      }
    };

    // Lifecycle
    onMounted(async () => await loadData());

    return {
      stubs,
      filteredStubs,
      loadData,
      showDeleteAllStubsModal,
      deleteAllStubs,
      tenants,
      urlStubIdFilter,
      selectedTenantName,
      download,
    };
  },
};
</script>

<style scoped></style>
