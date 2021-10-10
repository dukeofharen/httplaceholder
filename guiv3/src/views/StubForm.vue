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

  <FormHelperSelector v-if="showFormHelperSelector" />

  <div class="row mt-3">
    <div class="col-md-12">
      <codemirror v-model="input" :options="cmOptions" />
    </div>
  </div>

  <div class="row mt-3">
    <div class="col-md-12">
      <StubFormButtons v-model="stubId" />
    </div>
  </div>
</template>

<script>
import { useRoute } from "vue-router";
import { computed, onBeforeMount, onMounted } from "vue";
import { useStore } from "vuex";
import { resources } from "@/constants/resources";
import { handleHttpError } from "@/utils/error";
import yaml from "js-yaml";
import { clearIntermediateStub, getIntermediateStub } from "@/utils/session";
import FormHelperSelector from "@/components/stub/FormHelperSelector";
import StubFormButtons from "@/components/stub/StubFormButtons";

export default {
  name: "StubForm",
  components: { FormHelperSelector, StubFormButtons },
  setup() {
    const route = useRoute();
    const store = useStore();

    // Data
    const cmOptions = {
      tabSize: 4,
      mode: "text/x-yaml",
      lineNumbers: true,
      line: true,
    };

    // Computed
    const stubId = computed(() => route.params.stubId);
    const newStub = computed(() => !route.params.stubId);
    const title = computed(() => (newStub.value ? "Add stub" : "Update stub"));
    const input = computed({
      get: () => store.getters["stubForm/getInput"],
      set: (value) => store.commit("stubForm/setInput", value),
    });
    const showFormHelperSelector = computed(
      () => input.value.indexOf("- ") !== 0
    );

    // Lifecycle
    onBeforeMount(() => {
      if (store.getters["general/getDarkTheme"]) {
        cmOptions.theme = "material-darker";
      }
    });
    onMounted(async () => {
      store.commit("stubForm/closeFormHelper");
      if (newStub.value) {
        const intermediateStub = getIntermediateStub();
        if (intermediateStub) {
          input.value = intermediateStub;
          clearIntermediateStub();
        } else {
          input.value = resources.defaultStub;
        }
      } else {
        try {
          const fullStub = await store.dispatch("stubs/getStub", stubId.value);
          input.value = yaml.dump(fullStub.stub);
        } catch (e) {
          handleHttpError(e);
        }
      }
    });

    return {
      stubId,
      newStub,
      title,
      input,
      cmOptions,
      showFormHelperSelector,
    };
  },
};
</script>

<style scoped></style>
