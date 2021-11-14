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
      <button
        class="btn btn-outline-success btn-sm me-2"
        title="Disable the current selection of stubs"
        @click="showDisableStubsModal = true"
      >
        Disable stubs
      </button>
      <modal
        title="Disable the current filtered stubs?"
        bodyText="Only the stubs currently visible in the list will be disabled."
        :yes-click-function="disableStubs"
        :show-modal="showDisableStubsModal"
        @close="showDisableStubsModal = false"
      />

      <button
        class="btn btn-outline-success btn-sm"
        title="Enable the current selection of stubs"
        @click="showEnableStubsModal = true"
      >
        Enable stubs
      </button>
      <modal
        title="Enable the current filtered stubs?"
        bodyText="Only the stubs currently visible in the list will be enabled."
        :yes-click-function="enableStubs"
        :show-modal="showEnableStubsModal"
        @close="showEnableStubsModal = false"
      />
    </div>

    <div class="col-md-12 mb-3">
      <div class="input-group mb-3">
        <input
          type="text"
          class="form-control"
          placeholder="Filter on stub ID or URL..."
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
import { computed, onMounted, ref, watch } from "vue";
import Stub from "@/components/stub/Stub";
import toastr from "toastr";
import { resources } from "@/constants/resources";
import yaml from "js-yaml";
import { handleHttpError } from "@/utils/error";
import { downloadBlob } from "@/utils/download";
import { getStubFilterForm, setStubFilterForm } from "@/utils/session";
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
    const showDisableStubsModal = ref(false);
    const showEnableStubsModal = ref(false);

    const saveSearchFilters = store.getters["general/getSaveSearchFilters"];
    let savedFilter = {};
    if (saveSearchFilters) {
      savedFilter = getStubFilterForm() || {};
    }

    const filter = ref({
      urlStubIdFilter: route.query.filter || savedFilter.urlStubIdFilter || "",
      selectedTenantName:
        route.query.tenant || savedFilter.selectedTenantName || "",
    });

    // Functions
    const filterStubs = (input) => {
      let stubsResult = input;
      const compare = (a, b) => {
        if (a.stub.id < b.stub.id) return -1;
        if (a.stub.id > b.stub.id) return 1;
        return 0;
      };

      if (filter.value.urlStubIdFilter) {
        stubsResult = stubsResult.filter((s) => {
          const stubId = s.stub.id.toLowerCase();
          return stubId && stubId.includes(filter.value.urlStubIdFilter);
        });
      }

      if (filter.value.selectedTenantName) {
        stubsResult = stubsResult.filter(
          (s) => s.stub.tenant === filter.value.selectedTenantName
        );
      }

      stubsResult.sort(compare);
      return stubsResult;
    };

    // Computed
    const filteredStubs = computed(() => filterStubs(stubs.value));

    // Methods
    const loadStubs = async () => {
      try {
        stubs.value = [];
        stubs.value = await store.dispatch("stubs/getStubsOverview");
      } catch (e) {
        handleHttpError(e);
      }
    };
    const loadTenantNames = async () => {
      try {
        tenants.value = await store.dispatch("tenants/getTenantNames");
      } catch (e) {
        handleHttpError(e);
      }
    };
    const loadData = async () => {
      await Promise.all([loadStubs(), loadTenantNames()]);
    };
    const deleteAllStubs = async () => {
      try {
        await store.dispatch("stubs/deleteStubs");
        toastr.success(resources.stubsDeletedSuccessfully);
        await loadData();
      } catch (e) {
        handleHttpError(e);
      }
    };
    const disableStubs = async () => {
      const disableStub = async (stubIdToDisable) => {
        try {
          await store.dispatch("stubs/disableStub", stubIdToDisable);
        } catch (e) {
          handleHttpError(e);
        }
      };
      const stubIds = filteredStubs.value.map((fs) => fs.stub.id);
      const promises = [];
      for (const stubId of stubIds) {
        promises.push(disableStub(stubId));
      }

      await Promise.all(promises);
      toastr.success(resources.stubsDisabledSuccessfully);
      await loadData();
    };
    const enableStubs = async () => {
      const enableStub = async (stubIdToEnable) => {
        try {
          await store.dispatch("stubs/enableStub", stubIdToEnable);
        } catch (e) {
          handleHttpError(e);
        }
      };
      const stubIds = filteredStubs.value.map((fs) => fs.stub.id);
      const promises = [];
      for (const stubId of stubIds) {
        promises.push(enableStub(stubId));
      }

      await Promise.all(promises);
      toastr.success(resources.stubsEnabledSuccessfully);
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
    const filterChanged = () => {
      if (store.getters["general/getSaveSearchFilters"]) {
        setStubFilterForm(filter.value);
      }
    };

    // Watch
    watch(filter, () => filterChanged(), { deep: true });

    // Lifecycle
    onMounted(async () => await loadData());

    return {
      stubs,
      filteredStubs,
      loadData,
      showDeleteAllStubsModal,
      deleteAllStubs,
      tenants,
      filter,
      download,
      showDisableStubsModal,
      disableStubs,
      enableStubs,
      showEnableStubsModal,
    };
  },
};
</script>

<style scoped></style>
