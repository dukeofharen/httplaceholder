<template>
  <h1>Scenarios</h1>

  <div class="col-md-12 mb-3">
    <router-link class="btn btn-success me-2" :to="{ name: 'ScenarioForm' }"
      >Add scenario
    </router-link>
    <button class="btn btn-danger" @click="clearAllScenariosModal = true">
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
        v-for="scenario of filteredScenarios"
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
</template>

<script>
import { computed, onMounted, ref } from "vue";
import { useStore } from "vuex";
import { handleHttpError } from "@/utils/error";
import toastr from "toastr";
import { resources } from "@/constants/resources";

export default {
  name: "Scenarios",
  setup() {
    const store = useStore();

    // Data
    const scenarios = ref([]);
    const clearAllScenariosModal = ref(false);

    // Computed
    const filteredScenarios = computed(() => {
      let scenariosResult = scenarios.value;
      const compare = (a, b) => {
        if (a.scenario < b.scenario) return -1;
        if (a.scenario > b.scenario) return 1;
        return 0;
      };
      scenariosResult.sort(compare);
      return scenariosResult;
    });

    // Methods
    const loadScenarios = async () => {
      try {
        scenarios.value = await store.dispatch("scenarios/getAllScenarios");
      } catch (e) {
        handleHttpError(e);
      }
    };
    const clearAllScenarios = async () => {
      try {
        await store.dispatch("scenarios/deleteAllScenarios");
        toastr.success(resources.scenariosDeletedSuccessfully);
        await loadScenarios();
      } catch (e) {
        handleHttpError(e);
      }
    };
    const deleteScenario = async (scenario) => {
      try {
        await store.dispatch("scenarios/deleteScenario", scenario);
        toastr.success(resources.scenarioDeletedSuccessfully);
        await loadScenarios();
      } catch (e) {
        handleHttpError(e);
      }
    };

    // Lifecycle
    onMounted(async () => await loadScenarios());

    return {
      scenarios,
      filteredScenarios,
      clearAllScenariosModal,
      clearAllScenarios,
      deleteScenario,
    };
  },
};
</script>

<style scoped></style>