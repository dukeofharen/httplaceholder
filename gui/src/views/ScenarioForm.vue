<template>
  <h1>{{ title }}</h1>

  <div class="row">
    <div class="col-md-12 mb-2">
      <div class="input-group">
        <input
          type="text"
          class="form-control"
          placeholder="Scenario name (required)"
          v-model="scenarioForm.scenario"
        />
      </div>
    </div>
    <div class="col-md-12 mb-2">
      <div class="input-group">
        <input
          type="text"
          class="form-control"
          placeholder="Scenario state (optional)"
          v-model="scenarioForm.state"
        />
      </div>
    </div>
    <div class="col-md-12 mb-2">
      <div class="input-group">
        <input
          type="text"
          class="form-control"
          placeholder="Scenario hit count (optional)"
          v-model="scenarioForm.hitCount"
        />
      </div>
    </div>
    <div class="col-md-12 mb-2">
      <button class="btn btn-success" @click="save" :disabled="saveDisabled">
        Save
      </button>
    </div>
  </div>
</template>

<script>
import { useRoute, useRouter } from "vue-router";
import { computed, onMounted, onUnmounted, ref } from "vue";
import { useStore } from "vuex";
import { handleHttpError } from "@/utils/error";
import toastr from "toastr";
import { resources } from "@/constants/resources";
import { shouldSave } from "@/utils/event";

export default {
  name: "ScenarioForm",
  setup() {
    const route = useRoute();
    const router = useRouter();
    const store = useStore();

    // Data
    const scenarioForm = ref({
      scenario: "",
      state: "",
      hitCount: "",
    });

    // Computed
    const scenarioName = computed(() => route.params.scenario);
    const newScenario = computed(() => !scenarioName.value);
    const title = computed(() =>
      newScenario.value ? "Add scenario" : "Update scenario"
    );
    const saveDisabled = computed(() => !scenarioForm.value.scenario);

    // Methods
    const save = async () => {
      try {
        await store.dispatch("scenarios/setScenario", scenarioForm.value);
        toastr.success(resources.scenarioSetSuccessfully);
        await router.push({ name: "Scenarios" });
      } catch (e) {
        handleHttpError(e);
      }
    };
    const checkSave = async (e) => {
      if (shouldSave(e) && !saveDisabled.value) {
        e.preventDefault();
        await save();
      }
    };

    // Lifecycle
    const keydownEventListener = async (e) => await checkSave(e);
    onMounted(async () => {
      document.addEventListener("keydown", keydownEventListener);
      if (!newScenario.value) {
        try {
          scenarioForm.value = await store.dispatch(
            "scenarios/getScenario",
            scenarioName.value
          );
        } catch (e) {
          handleHttpError(e);
        }
      }
    });
    onUnmounted(() =>
      document.removeEventListener("keydown", keydownEventListener)
    );

    return { title, scenarioForm, save, saveDisabled };
  },
};
</script>

<style scoped></style>
