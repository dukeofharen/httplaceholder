<template>
  <div class="row">
    <div class="col-md-12">
      <strong>Insert new scenario name</strong>
      <input
        type="text"
        class="form-control mt-2"
        v-model="scenario"
        @keyup.enter="scenarioSelected(scenario)"
      />
      <button class="btn btn-success mt-2" @click="scenarioSelected(scenario)">
        Add
      </button>
    </div>
    <div class="col-md-12 mt-3" v-if="scenarios.length">
      <strong>Select existing scenario</strong>
      <div class="list-group mt-2">
        <button
          v-for="(scenario, index) of scenarios"
          :key="index"
          class="list-group-item list-group-item-action fw-bold"
          @click="scenarioSelected(scenario)"
        >
          {{ scenario }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { onMounted, ref } from "vue";
import { useStore } from "vuex";
import { handleHttpError } from "@/utils/error";
import { useScenariosStore } from "@/store/scenarios";

export default {
  name: "ScenarioSelector",
  setup() {
    const store = useStore();
    const scenarioStore = useScenariosStore();

    // Data
    const scenarios = ref([]);
    const scenario = ref([]);

    // Methods
    const scenarioSelected = (scenario) => {
      store.commit("stubForm/setScenario", scenario);
      store.commit("stubForm/closeFormHelper");
    };

    // Lifecycle
    onMounted(async () => {
      try {
        const scenariosResult = (await scenarioStore.getAllScenarios()).map(
          (s) => s.scenario
        );
        scenariosResult.sort();
        scenarios.value = scenariosResult;
      } catch (e) {
        handleHttpError(e);
      }
    });

    return { scenarios, scenarioSelected, scenario };
  },
};
</script>

<style scoped></style>
