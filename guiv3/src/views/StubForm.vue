<template>
  <h1>{{ title }}</h1>

  <div class="row">
    <div class="col-md-12">
      Fill in the stub below in YAML format and click on "Save". For examples,
      visit
      <a href="https://github.com/dukeofharen/httplaceholder" target="_blank"
        >https://github.com/dukeofharen/httplaceholder</a
      >.
    </div>
  </div>

  <div class="row">
    <div class="col-md-12">
      <codemirror v-model="input" :options="cmOptions" />
    </div>
  </div>

  <div class="row mt-2">
    <div class="col-md-12">
      <button class="btn btn-success me-2" @click="save">Save</button>
      <button
        type="button"
        class="btn btn-danger"
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
    </div>
  </div>
</template>

<script>
import { useRoute } from "vue-router";
import { computed, onMounted, ref } from "vue";
import { useStore } from "vuex";
import { resources } from "@/constants/resources";
import { handleHttpError } from "@/utils/error";
import toastr from "toastr";
import yaml from "js-yaml";
import { clearIntermediateStub, getIntermediateStub } from "@/utils/session";

export default {
  name: "StubForm",
  setup() {
    const route = useRoute();
    const store = useStore();

    // Data
    const stubId = ref(route.params.stubId);
    const showResetModal = ref(false);
    const cmOptions = {
      tabSize: 4,
      mode: "text/x-yaml",
      lineNumbers: true,
      line: true,
    };

    // Computed
    const newStub = computed(() => !stubId.value);
    const title = computed(() => (newStub.value ? "Add stub" : "Update stub"));
    const input = computed({
      get: () => store.getters["stubForm/getInput"],
      set: (value) => store.commit("stubForm/setInput", value),
    });

    // Methods
    const save = async () => {
      try {
        const result = await store.dispatch("stubs/addStubs", input.value);
        if (result.length === 1) {
          stubId.value = result[0].stub.id;
        }

        toastr.success(
          newStub.value
            ? resources.stubsAddedSuccessfully
            : resources.stubUpdatedSuccessfully
        );
      } catch (e) {
        handleHttpError(e);
      }
    };
    const reset = () => {
      input.value = resources.defaultStub;
      stubId.value = "";
    };

    // Lifecycle
    onMounted(async () => {
      if (newStub.value) {
        const intermediateStub = getIntermediateStub();
        if (intermediateStub) {
          input.value = intermediateStub;
          clearIntermediateStub();
        } else {
          input.value = resources.defaultStub;
        }
      } else {
        const fullStub = await store.dispatch("stubs/getStub", stubId.value);
        input.value = yaml.dump(fullStub.stub);
      }
    });

    return {
      stubId,
      newStub,
      title,
      input,
      cmOptions,
      save,
      showResetModal,
      reset,
    };
  },
};
</script>

<style scoped></style>
