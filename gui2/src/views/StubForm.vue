<template>
  <div>
    <h1>{{ title }}</h1>

    <div class="row">
      <div class="col-md-12">
        Fill in the stub below in YAML format and click on "Save". For examples,
        visit
        <a
          href="https://github.com/dukeofharen/httplaceholder"
          target="_blank"
          class="break-word"
          >https://github.com/dukeofharen/httplaceholder</a
        >.
      </div>
    </div>

    <FormHelperSelector v-if="showFormHelperSelector" />

    <div class="row mt-3">
      <div class="col-md-12">
        <button
          class="btn btn-outline btn-sm me-2"
          :class="{
            'btn-outline-success': editorType === editorTypes.codemirror,
          }"
          @click="selectedEditorType = editorTypes.codemirror"
          title="Use advanced editor for editing the stub. The editor has code highlighting but is not suited for updating large stubs."
        >
          Advanced editor
        </button>
        <button
          class="btn btn-outline btn-sm"
          :class="{
            'btn-outline-success': editorType === editorTypes.simple,
          }"
          @click="selectedEditorType = editorTypes.simple"
          title="Use simple editor for editing the stub. The editor has no code highlighting but is suited for updating large stubs."
        >
          Simple editor
        </button>
      </div>
    </div>

    <div class="row mt-3">
      <div class="col-md-12" v-if="editorType === editorTypes.codemirror">
        <codemirror v-model="input" :options="cmOptions" />
      </div>
      <div class="col-md-12" v-if="editorType === editorTypes.simple">
        <simple-editor v-model="input" />
      </div>
    </div>

    <div class="row mt-3">
      <div class="col-md-12">
        <StubFormButtons v-model="stubId" />
      </div>
    </div>
  </div>
</template>

<script>
import { useRoute, useRouter } from "vue-router";
import { computed, onMounted, watch, ref } from "vue";
import { resources } from "@/constants/resources";
import { simpleEditorThreshold } from "@/constants/technical";
import { handleHttpError } from "@/utils/error";
import yaml from "js-yaml";
import { clearIntermediateStub, getIntermediateStub } from "@/utils/session";
import FormHelperSelector from "@/components/stub/FormHelperSelector.vue";
import StubFormButtons from "@/components/stub/StubFormButtons.vue";
import SimpleEditor from "@/components/simpleEditor/SimpleEditor.vue";
import { error } from "@/utils/toast";
import { useStubsStore } from "@/store/stubs";
import { useStubFormStore } from "@/store/stubForm";

const editorTypes = {
  none: "none",
  codemirror: "codemirror",
  simple: "simple",
};

export default {
  name: "StubForm",
  components: { SimpleEditor, FormHelperSelector, StubFormButtons },
  setup() {
    const route = useRoute();
    const router = useRouter();
    const stubStore = useStubsStore();
    const stubFormStore = useStubFormStore();

    // Data
    const cmOptions = {
      tabSize: 4,
      mode: "text/x-yaml",
      lineNumbers: true,
      line: true,
    };
    const selectedEditorType = ref(editorTypes.none);

    // Computed
    const stubId = computed(() => route.params.stubId);
    const newStub = computed(() => !route.params.stubId);
    const title = computed(() => (newStub.value ? "Add stub" : "Update stub"));
    const input = computed({
      get: () => stubFormStore.getInput,
      set: (value) => stubFormStore.setInput(value),
    });
    const showFormHelperSelector = computed(
      () => !stubFormStore.getInputHasMultipleStubs
    );
    const editorType = computed(() => {
      if (selectedEditorType.value !== editorTypes.none) {
        return selectedEditorType.value;
      }

      return stubFormStore.getInputLength > simpleEditorThreshold
        ? editorTypes.simple
        : editorTypes.codemirror;
    });

    // Functions
    const initialize = async () => {
      stubFormStore.closeFormHelper();
      if (newStub.value) {
        let intermediateStub = getIntermediateStub();
        if (intermediateStub) {
          const deserializedStub = yaml.load(intermediateStub);
          if (
            Array.isArray(deserializedStub) &&
            deserializedStub.length === 1
          ) {
            // When the intermediate stub is an array that contains only 1 stub, make it an object for easier editing.
            intermediateStub = yaml.dump(deserializedStub[0]);
          }

          input.value = intermediateStub;
          clearIntermediateStub();
        } else {
          input.value = resources.defaultStub;
        }
      } else {
        try {
          const fullStub = await stubStore.getStub(stubId.value);
          input.value = yaml.dump(fullStub.stub);
        } catch (e) {
          if (e.status === 404) {
            error(resources.stubNotFound.format(stubId.value));
            await router.push({ name: "StubForm" });
          } else {
            handleHttpError(e);
          }
        }
      }
    };

    // Lifecycle
    onMounted(async () => await initialize());

    // Watch
    watch(stubId, async () => {
      await initialize();
    });

    return {
      stubId,
      newStub,
      title,
      input,
      cmOptions,
      showFormHelperSelector,
      editorTypes,
      selectedEditorType,
      editorType,
    };
  },
};
</script>

<style scoped></style>
