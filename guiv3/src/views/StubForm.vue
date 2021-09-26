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

  <div class="row">
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
import toastr from "toastr";

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
        await store.dispatch("stubs/addStubs", input.value);
        if (newStub.value) {
          toastr.success(resources.stubsAddedSuccessfully);
        } else {
          toastr.success(resources.stubUpdatedSuccessfully);
        }
      } catch (e) {
        console.log(e);
      }
    };
    const reset = () => {
      input.value = resources.defaultStub;
    };

    // TODO reset button
    // TODO save button
    // TODO intermediate stub
    // TODO check HTTP 400 situations
    // Lifecycle
    onMounted(async () => {
      if (newStub.value) {
        input.value = resources.defaultStub;
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
