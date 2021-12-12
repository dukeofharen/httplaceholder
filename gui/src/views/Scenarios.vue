<template>
  <h1>Scenarios</h1>
  <div class="col-md-12">
    <div class="list-group">
      <router-link
        v-for="scenario of filteredScenarios"
        :key="scenario.scenario"
        class="list-group-item list-group-item-action"
        :to="{
          name: 'ScenarioForm',
          params: { scenario: scenario.scenario },
        }"
      >
        <span class="fw-bold">{{ scenario.scenario }}</span
        ><br />
        State: {{ scenario.state }}<br />
        Hit count: {{ scenario.hitCount }}
      </router-link>
    </div>
  </div>
</template>

<script>
import { computed, onMounted, ref } from "vue";
import { useStore } from "vuex";
import { handleHttpError } from "@/utils/error";

export default {
  name: "Scenarios",
  setup() {
    const store = useStore();

    // Data
    const scenarios = ref([]);

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

    // Lifecycle
    onMounted(async () => {
      try {
        scenarios.value = await store.dispatch("scenarios/getAllScenarios");
      } catch (e) {
        handleHttpError(e);
      }
    });

    return { scenarios, filteredScenarios };
  },
};
</script>

<style scoped></style>
