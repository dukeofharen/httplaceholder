<template>
  <div>
    <h1>Scenarios</h1>

    <div class="col-md-12 mb-3">
      Scenarios can be used to make stubs stateful. On this page, you can manage
      the scenarios in HttPlaceholder. To read more about scenarios, go to
      <a :href="docsUrl" target="_blank">the documentation</a>.
    </div>

    <div class="col-md-12 mb-3">
      <router-link
        class="btn btn-success me-2 btn-mobile full-width"
        :to="{ name: 'ScenarioForm' }"
        >Add scenario
      </router-link>
      <button
        class="btn btn-danger btn-mobile full-width"
        @click="clearAllScenariosModal = true"
      >
        Clear all scenarios
      </button>
      <modal
        title="Clear all scenarios?"
        bodyText="The scenarios can't be recovered."
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
            State: {{ scenario.state }}<br />
            Hit count: {{ scenario.hitCount }}
          </div>
          <div>
            <router-link
              class="btn btn-success btn-sm me-2"
              :to="{
                name: 'ScenarioForm',
                params: { scenario: scenario.scenario },
              }"
            >
              Update
            </router-link>
            <button
              class="btn btn-danger btn-sm"
              @click="deleteScenario(scenario.scenario)"
            >
              Delete
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
import { renderDocLink, resources } from "@/constants/resources";
import { success } from "@/utils/toast";
import { useScenariosStore } from "@/store/scenarios";
import { defineComponent } from "vue";
import type { ScenarioModel } from "@/domain/scenario/scenario-model";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";

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
      let scenariosResult = scenarios.value;
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
        .withUrl("/scenarioHub")
        .build();
      signalrConnection.on("ScenarioSet", (scenario: ScenarioModel) => {
        const foundScenario = scenarios.value.find(
          (s) => s.scenario === scenario.scenario
        );
        if (foundScenario) {
          scenarios.value = scenarios.value.filter((s) => s !== foundScenario);
        }

        scenarios.value.push(scenario);
      });
      signalrConnection.on("ScenarioDeleted", (scenarioName: string) => {
        const foundScenario = scenarios.value.find(
          (s) => s.scenario === scenarioName
        );
        if (foundScenario) {
          scenarios.value = scenarios.value.filter(
            (s) => s.scenario !== scenarioName
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
        success(resources.scenariosDeletedSuccessfully);
        await loadScenarios();
      } catch (e) {
        handleHttpError(e);
      }
    };
    const deleteScenario = async (scenario: string) => {
      try {
        await scenarioStore.deleteScenario(scenario);
        success(resources.scenarioDeletedSuccessfully);
        await loadScenarios();
      } catch (e) {
        handleHttpError(e);
      }
    };

    // Lifecycle
    onMounted(
      async () => await Promise.all([loadScenarios(), initializeSignalR()])
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
