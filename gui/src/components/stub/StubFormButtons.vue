<template>
  <button class="btn btn-success me-2" @click="save">Save</button>
  <button
    v-if="showSaveAsNewStubButton"
    class="btn btn-success me-2"
    @click="addStub"
  >
    Save as new stub
  </button>
  <button type="button" class="btn btn-danger" @click="showResetModal = true">
    Reset
  </button>
  <modal
    title="Reset to defaults?"
    :yes-click-function="reset"
    :show-modal="showResetModal"
    @close="showResetModal = false"
  />
</template>

<script>
import { computed, onMounted, onUnmounted, ref } from "vue";
import { resources } from "@/constants/resources";
import { useStore } from "vuex";
import toastr from "toastr";
import { handleHttpError } from "@/utils/error";
import { useRoute, useRouter } from "vue-router";
import { shouldSave } from "@/utils/event";
import { formHelperKeys } from "@/constants/stubFormResources";

export default {
  name: "StubFormButtons",
  props: {
    modelValue: {
      type: String,
      default: "",
    },
  },
  setup(props, { emit }) {
    const store = useStore();
    const route = useRoute();
    const router = useRouter();

    // Data
    const showResetModal = ref(false);

    // Computed
    const input = computed({
      get: () => store.getters["stubForm/getInput"],
      set: (value) => store.commit("stubForm/setInput", value),
    });
    const newStub = computed(() => !route.params.stubId);
    const stubId = computed(() => route.params.stubId);
    const showSaveAsNewStubButton = computed(
      () =>
        !store.getters["stubForm/getInputHasMultipleStubs"] && !newStub.value
    );

    // Methods
    const reset = async () => {
      input.value = resources.defaultStub;
      emit("update:modelValue", "");
      await router.push({ name: "StubForm" });
    };
    const addStub = async () => {
      try {
        const result = await store.dispatch("stubs/addStubs", input.value);
        if (result.length === 1) {
          const addedStubId = result[0].stub.id;
          if (stubId.value !== addedStubId) {
            await router.push({
              name: "StubForm",
              params: { stubId: addedStubId },
            });
          }
        }

        toastr.success(resources.stubsAddedSuccessfully);
      } catch (e) {
        handleHttpError(e);
      }
    };
    const updateStub = async () => {
      try {
        await store.dispatch("stubs/updateStub", {
          stubId: stubId.value,
          input: input.value,
        });
        toastr.success(resources.stubUpdatedSuccessfully);
        const currentStubId = store.getters["stubForm/getStubId"];
        if (stubId.value !== currentStubId) {
          await router.push({
            name: "StubForm",
            params: { stubId: currentStubId },
          });
        }
      } catch (e) {
        handleHttpError(e);
      }
    };
    const save = async () => {
      if (newStub.value || !showSaveAsNewStubButton.value) {
        await addStub();
      } else {
        await updateStub();
      }
    };
    const checkSave = async (e) => {
      const currentSelectedFormHelper =
        store.getters["stubForm/getCurrentSelectedFormHelper"];
      if (
        shouldSave(e) &&
        currentSelectedFormHelper.value !== formHelperKeys.responseBody
      ) {
        e.preventDefault();
        await save();
      }
    };

    // Lifecycle
    const keydownEventListener = async (e) => await checkSave(e);
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
};
</script>

<style scoped></style>
