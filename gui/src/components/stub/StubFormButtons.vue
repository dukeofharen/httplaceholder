<template>
  <button class="btn btn-success me-2 btn-mobile" @click="save">Save</button>
  <button
    v-if="showSaveAsNewStubButton"
    class="btn btn-success me-2 btn-mobile"
    @click="addStub"
  >
    Save as new stub
  </button>
  <button
    type="button"
    class="btn btn-danger btn-mobile"
    @click="showResetModal = true"
  >
    Reset
  </button>
  <modal
    title="Reset to defaults?"
    :yes-click-function="reset"
    :show-modal="showResetModal"
    @close="showResetModal = false"
  />
</template>

<script lang="ts">
import { computed, onMounted, onUnmounted, ref } from "vue";
import { resources } from "@/constants/resources";
import { handleHttpError } from "@/utils/error";
import { useRoute, useRouter } from "vue-router";
import { shouldSave } from "@/utils/event";
import { success } from "@/utils/toast";
import { useStubsStore } from "@/store/stubs";
import { useStubFormStore } from "@/store/stubForm";
import { defineComponent } from "vue";
import { error } from "@/utils/toast";
import { FormHelperKey } from "@/domain/stubForm/form-helper-key";
import { vsprintf } from "sprintf-js";

export default defineComponent({
  name: "StubFormButtons",
  props: {
    modelValue: {
      type: String,
      default: "",
    },
  },
  setup(props, { emit }) {
    const stubStore = useStubsStore();
    const stubFormStore = useStubFormStore();
    const route = useRoute();
    const router = useRouter();

    // Data
    const showResetModal = ref(false);

    // Computed
    const input = computed({
      get: () => stubFormStore.getInput,
      set: (value) => stubFormStore.setInput(value),
    });
    const newStub = computed(() => !route.params.stubId);
    const stubId = computed(() => route.params.stubId as string);
    const showSaveAsNewStubButton = computed(
      () => !stubFormStore.getInputHasMultipleStubs && !newStub.value
    );

    // Methods
    const reset = async () => {
      input.value = resources.defaultStub;
      emit("update:modelValue", "");
      await router.push({ name: "StubForm" });
    };
    const addStub = async () => {
      try {
        const result = await stubStore.addStubs(input.value);
        if (result.length === 1) {
          const addedStubId = result[0].stub.id;
          if (stubId.value !== addedStubId) {
            await router.push({
              name: "StubForm",
              params: { stubId: addedStubId },
            });
          }
        }

        success(resources.stubsAddedSuccessfully);
      } catch (e) {
        if (!handleHttpError(e)) {
          error(vsprintf(resources.errorDuringParsingOfYaml, [e]));
        }
      }
    };
    const updateStub = async () => {
      try {
        await stubStore.updateStub({
          stubId: stubId.value,
          input: input.value,
        });
        success(resources.stubUpdatedSuccessfully);
        const currentStubId = stubFormStore.getStubId;
        if (stubId.value !== currentStubId) {
          await router.push({
            name: "StubForm",
            params: { stubId: currentStubId },
          });
        }
      } catch (e) {
        if (!handleHttpError(e)) {
          error(vsprintf(resources.errorDuringParsingOfYaml, [e]));
        }
      }
    };
    const save = async () => {
      if (newStub.value || !showSaveAsNewStubButton.value) {
        await addStub();
      } else {
        await updateStub();
      }
    };
    const checkSave = async (e: KeyboardEvent) => {
      const currentSelectedFormHelper =
        stubFormStore.getCurrentSelectedFormHelper;
      if (
        shouldSave(e) &&
        currentSelectedFormHelper !== FormHelperKey.ResponseBody
      ) {
        e.preventDefault();
        await save();
      }
    };

    // Lifecycle
    const keydownEventListener = async (e: KeyboardEvent) => await checkSave(e);
    onMounted(() => document.addEventListener("keydown", keydownEventListener));
    onUnmounted(() =>
      document.removeEventListener("keydown", keydownEventListener)
    );

    return {
      showResetModal,
      reset,
      stubId,
      save,
      newStub,
      addStub,
      showSaveAsNewStubButton,
    };
  },
});
</script>

<style scoped></style>
