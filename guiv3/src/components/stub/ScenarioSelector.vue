<template>
  <div class="list-group">
    <button
      v-for="(scenario, index) of scenarios"
      :key="index"
      class="list-group-item list-group-item-action fw-bold"
      @click="scenarioSelected(scenario)"
    >
      {{ scenario }}
    </button>
  </div>
</template>

<script>
import { onMounted, ref } from "vue";
import { useStore } from "vuex";
import { handleHttpError } from "@/utils/error";

export default {
  name: "ScenarioSelector",
  setup() {
    const store = useStore();

    // Data
    const scenarios = ref([]);

    // Methods
    const scenarioSelected = (scenario) => {
      store.commit("stubForm/setScenario", scenario);
      store.commit("stubForm/closeFormHelper");
    };

    // Lifecycle
    onMounted(async () => {
      try {
        const scenariosResult = (
          await store.dispatch("scenarios/getAllScenarios")
        ).map((s) => s.scenario);
        scenariosResult.sort();
        scenariosResult.unshift("default");
        scenarios.value = scenariosResult;
      } catch (e) {
        handleHttpError(e);
      }
    });

    return { scenarios, scenarioSelected };
  },
};
</script>

<style scoped></style>
