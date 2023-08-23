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

<script lang="ts">
import { onMounted, ref } from "vue";
import { handleHttpError } from "@/utils/error";
import { useScenariosStore } from "@/store/scenarios";
import { useStubFormStore } from "@/store/stubForm";
import { defineComponent } from "vue";

export default defineComponent({
  name: "ScenarioSelector",
  setup() {
    const scenarioStore = useScenariosStore();
    const stubFormStore = useStubFormStore();

    // Data
    const scenarios = ref<string[]>([]);
    const scenario = ref("");

    // Methods
    const scenarioSelected = (scenario: string) => {
      stubFormStore.setScenario(scenario);
      stubFormStore.closeFormHelper();
    };

    // Lifecycle
    onMounted(async () => {
      try {
        const scenariosResult = (await scenarioStore.getAllScenarios()).map(
          (s) => s.scenario,
        );
        scenariosResult.sort();
        scenarios.value = scenariosResult;
      } catch (e) {
        handleHttpError(e);
      }
    });

    return { scenarios, scenarioSelected, scenario };
  },
});
</script>

<style scoped></style>
