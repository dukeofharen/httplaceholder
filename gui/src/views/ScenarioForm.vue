<template>
  <div @keyup.enter="save">
    <h1>{{ title }}</h1>

    <div class="row">
      <div class="col-md-12 mb-2">
        <div class="input-group">
          <input
            type="text"
            class="form-control"
            :placeholder="`${$translate('scenarioForm.scenarioName')} (${$translate('general.required')})`"
            v-model="scenarioForm.scenario"
          />
        </div>
      </div>
      <div class="col-md-12 mb-2">
        <div class="input-group">
          <input
            type="text"
            class="form-control"
            :placeholder="`${$translate('scenarioForm.scenarioState')} (${$translate('general.optional')})`"
            v-model="scenarioForm.state"
          />
        </div>
      </div>
      <div class="col-md-12 mb-2">
        <div class="input-group">
          <input
            type="text"
            class="form-control"
            :placeholder="`${$translate('scenarioForm.scenarioHitCount')} (${$translate('general.optional')})`"
            v-model="scenarioForm.hitCount"
          />
        </div>
      </div>
      <div class="col-md-12 mb-2">
        <button class="btn btn-success" @click="save" :disabled="saveDisabled">
          {{ $translate("general.save") }}
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { useRoute, useRouter } from "vue-router";
import {
  computed,
  defineComponent,
  onMounted,
  onUnmounted,
  ref,
  watch,
} from "vue";
import { handleHttpError } from "@/utils/error";
import { shouldSave } from "@/utils/event";
import { success } from "@/utils/toast";
import { type ScenarioInputModel, useScenariosStore } from "@/store/scenarios";
import { translate } from "@/utils/translate";

export default defineComponent({
  name: "ScenarioForm",
  setup() {
    const route = useRoute();
    const router = useRouter();
    const scenarioStore = useScenariosStore();

    // Data
    const scenarioForm = ref<ScenarioInputModel>({
      scenario: "",
      state: "",
      hitCount: undefined,
    });

    // Computed
    const scenarioName = computed(() => route.params.scenario as string);
    const newScenario = computed(() => !scenarioName.value);
    const title = computed(() =>
      newScenario.value
        ? translate("scenarioForm.addScenario")
        : translate("scenarioForm.updateScenario"),
    );
    const saveDisabled = computed(() => !scenarioForm.value.scenario);

    // Methods
    const save = async () => {
      try {
        if (saveDisabled.value) {
          return;
        }

        if (!scenarioForm.value.hitCount) {
          scenarioForm.value.hitCount = 0;
        }

        await scenarioStore.setScenario(scenarioForm.value);
        success(translate("scenarioForm.scenarioSetSuccessfully"));
        await router.push({ name: "Scenarios" });
      } catch (e) {
        handleHttpError(e);
      }
    };
    const checkSave = async (e: KeyboardEvent) => {
      if (shouldSave(e)) {
        e.preventDefault();
        await save();
      }
    };

    // Lifecycle
    const keydownEventListener = async (e: KeyboardEvent) => await checkSave(e);
    onMounted(async () => {
      document.addEventListener("keydown", keydownEventListener);
      if (scenarioName.value) {
        scenarioForm.value.scenario = scenarioName.value;
      }

      if (!newScenario.value) {
        try {
          scenarioForm.value = await scenarioStore.getScenario(
            scenarioName.value,
          );
        } catch (e: any) {
          if (e.status !== 404) {
            handleHttpError(e);
          }
        }
      }
    });
    onUnmounted(() =>
      document.removeEventListener("keydown", keydownEventListener),
    );

    // Watches
    watch(
      scenarioForm,
      () => {
        let hitCount = scenarioForm.value.hitCount as any;
        if (typeof hitCount !== "string") {
          hitCount = hitCount + "";
        }

        const regex = /[^0-9]+/gi;
        const cleanedHitCount = hitCount.replace(regex, "");
        const parsedHitCount = parseInt(cleanedHitCount);
        scenarioForm.value.hitCount = isNaN(parsedHitCount)
          ? undefined
          : parsedHitCount;
      },
      { deep: true },
    );

    return { title, scenarioForm, save, saveDisabled };
  },
});
</script>

<style scoped></style>
