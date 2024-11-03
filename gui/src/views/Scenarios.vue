<template>
  <div>
    <h1>{{ $translate("scenarios.scenarios") }}</h1>

    <div
      class="col-md-12 mb-3"
      v-html="
        $vsprintf(
          $translateWithMarkdown('scenarios.description', {
            linkTarget: '_blank',
          }),
          [docsUrl],
        )
      "
    />

    <div class="col-md-12 mb-3">
      <router-link
        class="btn btn-success me-2 btn-mobile full-width"
        :to="{ name: 'ScenarioForm' }"
        >{{ $translate("scenarios.addScenario") }}
      </router-link>
      <button
        class="btn btn-danger btn-mobile full-width"
        @click="clearAllScenariosModal = true"
      >
        {{ $translate("scenarios.clearAllScenarios") }}
      </button>
      <modal
        :title="$translate('scenarios.clearAllScenariosQuestion')"
        :bodyText="$translate('scenarios.scenariosCantBeRecovered')"
        :yes-click-function="clearAllScenarios"
        :show-modal="clearAllScenariosModal"
        @close="clearAllScenariosModal = false"
      />
    </div>

    <div class="col-md-12">
      <ul class="list-group">
        <li
          v-for="scenario of sortedScenarios"
          :key="scenario.scenario"
          class="list-group-item list-group-item-action"
        >
          <div>
            <span class="fw-bold">{{ scenario.scenario }}</span
            ><br />
            {{ $translate("scenarios.state") }}: {{ scenario.state }}<br />
            {{ $translate("scenarios.hitCount") }}: {{ scenario.hitCount }}
          </div>
          <div>
            <router-link
              class="btn btn-success btn-sm me-2"
              :to="{
                name: 'ScenarioForm',
                params: { scenario: scenario.scenario },
              }"
            >
              {{ $translate("general.update") }}
            </router-link>
            <button
              class="btn btn-danger btn-sm"
              @click="deleteScenario(scenario.scenario)"
            >
              {{ $translate("general.delete") }}
            </button>
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, onMounted, onUnmounted, ref } from "vue";
import { handleHttpError } from "@/utils/error";
import { renderDocLink } from "@/constants/resources";
import { success } from "@/utils/toast";
import { useScenariosStore } from "@/store/scenarios";
import { defineComponent } from "vue";
import type { ScenarioModel } from "@/domain/scenario/scenario-model";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { getRootUrl } from "@/utils/config";
import { translate } from "@/utils/translate";

export default defineComponent({
  name: "Scenarios",
  setup() {
    const scenarioStore = useScenariosStore();

    // Data
    const scenarios = ref<ScenarioModel[]>([]);
    const clearAllScenariosModal = ref(false);
    const docsUrl = renderDocLink("request-scenario");
    let signalrConnection: HubConnection;

    // Computed
    const sortedScenarios = computed(() => {
      const scenariosResult = scenarios.value;
      const compare = (a: ScenarioModel, b: ScenarioModel) => {
        if (a.scenario < b.scenario) return -1;
        if (a.scenario > b.scenario) return 1;
        return 0;
      };
      scenariosResult.sort(compare);
      return scenariosResult;
    });

    // Functions
    const initializeSignalR = async () => {
      signalrConnection = new HubConnectionBuilder()
        .withUrl(`${getRootUrl()}/scenarioHub`)
        .build();
      signalrConnection.on("ScenarioSet", (scenario: ScenarioModel) => {
        const foundScenario = scenarios.value.find(
          (s) => s.scenario === scenario.scenario,
        );
        if (foundScenario) {
          scenarios.value = scenarios.value.filter((s) => s !== foundScenario);
        }

        scenarios.value.push(scenario);
      });
      signalrConnection.on("ScenarioDeleted", (scenarioName: string) => {
        const foundScenario = scenarios.value.find(
          (s) => s.scenario === scenarioName,
        );
        if (foundScenario) {
          scenarios.value = scenarios.value.filter(
            (s) => s.scenario !== scenarioName,
          );
        }
      });
      signalrConnection.on("AllScenariosDeleted", () => (scenarios.value = []));
      try {
        await signalrConnection.start();
      } catch (err: any) {
        console.log(err.toString());
      }
    };

    // Methods
    const loadScenarios = async () => {
      try {
        scenarios.value = await scenarioStore.getAllScenarios();
      } catch (e) {
        handleHttpError(e);
      }
    };
    const clearAllScenarios = async () => {
      try {
        await scenarioStore.deleteAllScenarios();
        success(translate("scenarios.scenariosDeletedSuccessfully"));
        await loadScenarios();
      } catch (e) {
        handleHttpError(e);
      }
    };
    const deleteScenario = async (scenario: string) => {
      try {
        await scenarioStore.deleteScenario(scenario);
        success(translate("scenarios.scenarioDeletedSuccessfully"));
        await loadScenarios();
      } catch (e) {
        handleHttpError(e);
      }
    };

    // Lifecycle
    onMounted(
      async () => await Promise.all([loadScenarios(), initializeSignalR()]),
    );
    onUnmounted(() => {
      if (signalrConnection) {
        signalrConnection.stop();
      }
    });

    return {
      scenarios,
      sortedScenarios,
      clearAllScenariosModal,
      clearAllScenarios,
      deleteScenario,
      docsUrl,
    };
  },
});
</script>

<style scoped></style>
