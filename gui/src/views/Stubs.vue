<template>
  <div>
    <h1>{{ $translate("stubs.stubs") }}</h1>

    <div class="col-md-12 mb-3">
      <button
        type="button"
        class="btn btn-success me-2 btn-mobile full-width"
        @click="loadData"
      >
        {{ $translate("general.refresh") }}
      </button>
      <router-link
        :to="{ name: 'StubForm' }"
        class="btn btn-success me-2 btn-mobile full-width"
        >{{ $translate("stubs.addStubs") }}
      </router-link>
      <button
        class="btn btn-success me-2 btn-mobile full-width"
        @click="download"
        :title="$translate('stubs.downloadStubsDescription')"
      >
        {{ $translate("stubs.downloadStubs") }}
      </button>
      <router-link
        :to="{ name: 'ImportStubs' }"
        class="btn btn-success me-2 btn-mobile full-width"
        >{{ $translate("stubs.importStubs") }}
      </router-link>
      <button
        type="button"
        class="btn btn-danger btn-mobile full-width"
        @click="showDeleteAllStubsModal = true"
      >
        {{ $translate("stubs.deleteAllStubs") }}
      </button>
      <modal
        :title="$translate('stubs.deleteAllStubsQuestion')"
        :bodyText="$translate('stubs.stubsCantBeRecovered')"
        :yes-click-function="deleteAllStubs"
        :show-modal="showDeleteAllStubsModal"
        @close="showDeleteAllStubsModal = false"
      />
    </div>

    <div class="col-md-12 mb-3">
      <button
        class="btn btn-outline-success btn-sm me-2 btn-mobile"
        :title="$translate('stubs.disableStubsDescription')"
        @click="showDisableStubsModal = true"
        :disabled="disableMutationButtons"
      >
        {{ $translate("stubs.disableStubs") }}
      </button>
      <modal
        :title="$translate('stubs.disableStubsQuestion')"
        :bodyText="$translate('stubs.disableStubsModalBody')"
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
        {{ $translate("stubs.enableStubs") }}
      </button>
      <modal
        :title="$translate('stubs.enableStubsQuestion')"
        :bodyText="$translate('stubs.enableStubsModalBody')"
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
        {{ $translate("stubs.deleteSelectedStubs") }}
      </button>
      <modal
        :title="$translate('stubs.deleteSelectedStubsQuestion')"
        :bodyText="$translate('stubs.deleteSelectedStubsModalBody')"
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
          :placeholder="$translate('stubs.filterPlaceholder')"
          v-model="filter.stubFilter"
        />
        <button
          class="btn btn-danger fw-bold"
          type="button"
          :title="$translate('general.reset')"
          @click="filter.stubFilter = ''"
        >
          <em class="bi-x"></em>
        </button>
      </div>
      <div v-if="tenants.length" class="input-group">
        <select class="form-select" v-model="filter.selectedTenantName">
          <option value="" selected>
            {{ $translate("general.selectStubTenantCategory") }}
          </option>
          <option v-for="tenant of tenants" :key="tenant">{{ tenant }}</option>
        </select>
        <button
          class="btn btn-danger fw-bold"
          type="button"
          :title="$translate('general.reset')"
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
        >{{ $translate("general.stubId") }}:
        <strong>{{ filter.stubFilter }} &times;</strong></span
      >
      <span
        class="badge bg-secondary clear-filter me-2"
        v-if="filter.selectedTenantName"
        @click="filter.selectedTenantName = ''"
        >{{ $translate("general.tenant") }}:
        <strong>{{ filter.selectedTenantName }} &times;</strong></span
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
      <router-link :to="{ name: 'StubForm' }"
        >{{ $translate("stubs.addStubs") }}
      </router-link>
      <span>&nbsp;</span>
      <router-link :to="{ name: 'ImportStubs' }"
        >{{ $translate("stubs.importStubs") }}
      </router-link>
      <span></span>
    </div>
  </div>
</template>

<script lang="ts">
import { useRoute } from "vue-router";
import {
  computed,
  defineComponent,
  onMounted,
  onUnmounted,
  ref,
  watch,
} from "vue";
import Stub from "@/components/stub/Stub.vue";
import yaml from "js-yaml";
import { handleHttpError } from "@/utils/error";
import { downloadBlob } from "@/utils/download";
import { getStubFilterForm, setStubFilterForm } from "@/utils/session";
import { success } from "@/utils/toast";
import { useTenantsStore } from "@/store/tenants";
import { useStubsStore } from "@/store/stubs";
import { useSettingsStore } from "@/store/settings";
import type { FullStubOverviewModel } from "@/domain/stub/full-stub-overview-model";
import type { StubSavedFilterModel } from "@/domain/stub-saved-filter-model";
import dayjs from "dayjs";
import { vsprintf } from "sprintf-js";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { getRootUrl } from "@/utils/config";
import { translate } from "@/utils/translate";

export default defineComponent({
  name: "Stubs",
  components: { Stub },
  setup() {
    const tenantStore = useTenantsStore();
    const stubStore = useStubsStore();
    const generalStore = useSettingsStore();
    const route = useRoute();

    // Data
    const stubs = ref<FullStubOverviewModel[]>([]);
    const showDeleteAllStubsModal = ref(false);
    const tenants = ref<string[]>([]);
    const showDisableStubsModal = ref(false);
    const showEnableStubsModal = ref(false);
    const showDeleteStubsModal = ref(false);
    let signalrConnection: HubConnection;
    let reloadStubsTimeout: number;

    const saveSearchFilters = generalStore.getSaveSearchFilters;
    let savedFilter: StubSavedFilterModel = {
      stubFilter: "",
      selectedTenantName: "",
    };
    if (saveSearchFilters) {
      savedFilter = getStubFilterForm();
    }

    const filter = ref({
      stubFilter:
        (route.query.filter as string) || savedFilter?.stubFilter || "",
      selectedTenantName:
        (route.query.tenant as string) || savedFilter?.selectedTenantName || "",
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
          (s) => s.stub.tenant === filter.value.selectedTenantName,
        );
      }

      stubsResult.sort(compare);
      return stubsResult;
    };

    const initializeSignalR = async () => {
      signalrConnection = new HubConnectionBuilder()
        .withUrl(`${getRootUrl()}/stubHub`)
        .build();
      signalrConnection.on("StubAdded", (stub: FullStubOverviewModel) => {
        if (!stubs.value.find((s) => s.stub.id === stub.stub.id)) {
          stubs.value.push(stub);
        }
      });
      signalrConnection.on("StubDeleted", (stubId: string) => {
        const stub = stubs.value.find((s) => s.stub.id === stubId);
        if (stub) {
          stubs.value.splice(stubs.value.indexOf(stub), 1);
        }
      });
      signalrConnection.on("ReloadStubs", () => {
        if (reloadStubsTimeout) {
          clearTimeout(reloadStubsTimeout);
        }

        reloadStubsTimeout = setTimeout(async () => {
          await loadStubs();
        }, 700);
      });
      try {
        await signalrConnection.start();
      } catch (err: any) {
        console.log(err.toString());
      }
    };

    // Computed
    const filteredStubs = computed(() => filterStubs(stubs.value));
    const filteredNonReadOnlyStubs = computed(() =>
      filteredStubs.value.filter((s) => !s.metadata.readOnly),
    );
    const disableMutationButtons = computed(
      () => !filteredNonReadOnlyStubs.value.length,
    );
    const showFilterBadges = computed(
      () => filter.value.stubFilter || filter.value.selectedTenantName,
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
        success(translate("stubs.stubsDeletedSuccessfully"));
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
      for (const stubId of stubIds) {
        await disableStub(stubId);
        success(
          vsprintf(translate("stubs.stubDisabledSuccessfully"), [stubId]),
        );
      }

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
      for (const stubId of stubIds) {
        await enableStub(stubId);
        success(vsprintf(translate("stubs.stubEnabledSuccessfully"), [stubId]));
      }

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
      success(translate("stubs.filteredStubsDeletedSuccessfully"));
      await loadData();
    };
    const download = async () => {
      try {
        const stubs = filterStubs(await stubStore.getStubs()).map(
          (fs) => fs.stub,
        );
        const downloadString = `${translate("stubs.downloadStubsHeader")}\n${yaml.dump(
          stubs,
        )}`;
        const dateTime = dayjs().format("YYYY-MM-DD_HH-mm");
        downloadBlob(`${dateTime}-stubs.yml`, downloadString);
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
    onMounted(async () => {
      await Promise.all([loadData(), initializeSignalR()]);
    });
    onUnmounted(() => {
      if (signalrConnection) {
        signalrConnection.stop();
      }
    });

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
