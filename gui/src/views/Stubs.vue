<template>
  <div>
    <h1>Stubs</h1>

    <div class="col-md-12 mb-3">
      <button
        type="button"
        class="btn btn-success me-2 btn-mobile full-width"
        @click="loadData"
      >
        Refresh
      </button>
      <router-link
        :to="{ name: 'StubForm' }"
        class="btn btn-success me-2 btn-mobile full-width"
        >Add stubs
      </router-link>
      <button
        class="btn btn-success me-2 btn-mobile full-width"
        @click="download"
        title="Download the (filtered) stubs as YAML file."
      >
        Download stubs as YAML
      </button>
      <router-link
        :to="{ name: 'ImportStubs' }"
        class="btn btn-success me-2 btn-mobile full-width"
        >Import stubs
      </router-link>
      <button
        type="button"
        class="btn btn-danger btn-mobile full-width"
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
        class="btn btn-outline-success btn-sm me-2 btn-mobile"
        title="Disable the current selection of stubs"
        @click="showDisableStubsModal = true"
        :disabled="disableMutationButtons"
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
        class="btn btn-outline-success btn-sm me-2 btn-mobile"
        title="Enable the current selection of stubs"
        @click="showEnableStubsModal = true"
        :disabled="disableMutationButtons"
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

      <button
        class="btn btn-outline-success btn-sm me-2 btn-mobile"
        title="Delete the current selection of stubs"
        @click="showDeleteStubsModal = true"
        :disabled="disableMutationButtons"
      >
        Delete stubs
      </button>
      <modal
        title="Delete the current filtered stubs?"
        bodyText="The stubs can't be recovered. Only the stubs currently visible in the list will be deleted."
        :yes-click-function="deleteStubs"
        :show-modal="showDeleteStubsModal"
        @close="showDeleteStubsModal = false"
      />
    </div>

    <div class="col-md-12 mb-3">
      <div class="input-group mb-3">
        <input
          type="text"
          class="form-control"
          placeholder="Filter on stub ID..."
          v-model="filter.stubFilter"
        />
        <button
          class="btn btn-danger fw-bold"
          type="button"
          title="Reset"
          @click="filter.stubFilter = ''"
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
        v-if="filter.stubFilter"
        @click="filter.stubFilter = ''"
        >Stub ID: <strong>{{ filter.stubFilter }} &times;</strong></span
      >
      <span
        class="badge bg-secondary clear-filter me-2"
        v-if="filter.selectedTenantName"
        @click="filter.selectedTenantName = ''"
        >Tenant: <strong>{{ filter.selectedTenantName }} &times;</strong></span
      >
    </div>

    <accordion v-if="stubs.length">
      <Stub
        v-for="stub of filteredStubs"
        :key="stub.stub.id"
        :overview-stub="stub"
        @deleted="loadData"
      />
    </accordion>
    <div v-else>
      No stubs have been added yet. Add a new stub by going to
      <router-link :to="{ name: 'StubForm' }">Add stubs</router-link>
      or
      <router-link :to="{ name: 'ImportStubs' }">Import stubs</router-link>
      .
    </div>
  </div>
</template>

<script lang="ts">
import { useRoute } from "vue-router";
import { computed, onMounted, ref, watch } from "vue";
import Stub from "@/components/stub/Stub.vue";
import { resources } from "@/constants/resources";
import yaml from "js-yaml";
import { handleHttpError } from "@/utils/error";
import { downloadBlob } from "@/utils/download";
import { getStubFilterForm, setStubFilterForm } from "@/utils/session";
import { success } from "@/utils/toast";
import { useTenantsStore } from "@/store/tenants";
import { useStubsStore } from "@/store/stubs";
import { useGeneralStore } from "@/store/general";
import { defineComponent } from "vue";
import type { FullStubOverviewModel } from "@/domain/stub/full-stub-overview-model";
import type { StubSavedFilterModel } from "@/domain/stub-saved-filter-model";

export default defineComponent({
  name: "Stubs",
  components: { Stub },
  setup() {
    const tenantStore = useTenantsStore();
    const stubStore = useStubsStore();
    const generalStore = useGeneralStore();
    const route = useRoute();

    // Data
    const stubs = ref<FullStubOverviewModel[]>([]);
    const showDeleteAllStubsModal = ref(false);
    const tenants = ref<string[]>([]);
    const showDisableStubsModal = ref(false);
    const showEnableStubsModal = ref(false);
    const showDeleteStubsModal = ref(false);

    const saveSearchFilters = generalStore.getSaveSearchFilters;
    let savedFilter: StubSavedFilterModel = {
      stubFilter: "",
      selectedTenantName: "",
    };
    if (saveSearchFilters) {
      savedFilter = getStubFilterForm() || {};
    }

    const filter = ref({
      stubFilter:
        (route.query.filter as string) || savedFilter.stubFilter || "",
      selectedTenantName:
        (route.query.tenant as string) || savedFilter.selectedTenantName || "",
    });

    // Functions
    const filterStubs = (input: FullStubOverviewModel[]) => {
      let stubsResult = input;
      const compare = (a: FullStubOverviewModel, b: FullStubOverviewModel) => {
        if (a.stub.id < b.stub.id) return -1;
        if (a.stub.id > b.stub.id) return 1;
        return 0;
      };

      if (filter.value.stubFilter) {
        stubsResult = stubsResult.filter((s) => {
          const stubId = s.stub.id.toLowerCase();
          return stubId && stubId.includes(filter.value.stubFilter);
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
    const filteredNonReadOnlyStubs = computed(() =>
      filteredStubs.value.filter((s) => !s.metadata.readOnly)
    );
    const disableMutationButtons = computed(
      () => !filteredNonReadOnlyStubs.value.length
    );
    const showFilterBadges = computed(
      () => filter.value.stubFilter || filter.value.selectedTenantName
    );

    // Methods
    const loadStubs = async () => {
      try {
        stubs.value = [];
        stubs.value = await stubStore.getStubsOverview();
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
    const loadData = async () => {
      await Promise.all([loadStubs(), loadTenantNames()]);
    };
    const deleteAllStubs = async () => {
      try {
        await stubStore.deleteStubs();
        success(resources.stubsDeletedSuccessfully);
        await loadData();
      } catch (e) {
        handleHttpError(e);
      }
    };
    const disableStubs = async () => {
      const disableStub = async (stubIdToDisable: string) => {
        try {
          await stubStore.disableStub(stubIdToDisable);
        } catch (e) {
          handleHttpError(e);
        }
      };
      const stubIds = filteredNonReadOnlyStubs.value.map((fs) => fs.stub.id);
      const promises = [];
      for (const stubId of stubIds) {
        promises.push(disableStub(stubId));
      }

      await Promise.all(promises);
      success(resources.stubsDisabledSuccessfully);
      await loadData();
    };
    const enableStubs = async () => {
      const enableStub = async (stubIdToEnable: string) => {
        try {
          await stubStore.enableStub(stubIdToEnable);
        } catch (e) {
          handleHttpError(e);
        }
      };
      const stubIds = filteredNonReadOnlyStubs.value.map((fs) => fs.stub.id);
      const promises = [];
      for (const stubId of stubIds) {
        promises.push(enableStub(stubId));
      }

      await Promise.all(promises);
      success(resources.stubsEnabledSuccessfully);
      await loadData();
    };
    const deleteStubs = async () => {
      const deleteStub = async (stubIdToDelete: string) => {
        try {
          await stubStore.deleteStub(stubIdToDelete);
        } catch (e) {
          handleHttpError(e);
        }
      };
      const stubIds = filteredNonReadOnlyStubs.value.map((fs) => fs.stub.id);
      const promises = [];
      for (const stubId of stubIds) {
        promises.push(deleteStub(stubId));
      }

      await Promise.all(promises);
      success(resources.filteredStubsDeletedSuccessfully);
      await loadData();
    };
    const download = async () => {
      try {
        const stubs = filterStubs(await stubStore.getStubs()).map(
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
      if (generalStore.getSaveSearchFilters) {
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
      showDeleteStubsModal,
      deleteStubs,
      disableMutationButtons,
      filteredNonReadOnlyStubs,
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
