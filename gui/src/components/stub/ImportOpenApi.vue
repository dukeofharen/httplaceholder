<template>
  <div class="mb-2 col-md-6">
    Using this form, you can create stubs based on an OpenAPI (or Swagger)
    definition. This definition can be provided in both JSON and YAML format.
    Many APIs are accompanied by an OpenAPI definition, so this is a great way
    to create stubs for the API.
  </div>
  <div class="mb-2">
    <button class="btn btn-outline-primary btn-sm" @click="insertExample">
      Insert example
    </button>
  </div>
  <div class="mb-2" v-if="!stubsYaml">
    <upload-button button-text="Upload file" @uploaded="onUploaded" />
  </div>
  <div class="mb-2" v-if="!stubsYaml">
    <textarea class="form-control" v-model="openApiInput"></textarea>
  </div>
  <div v-if="!stubsYaml" class="mb-2">
    <button
      class="btn btn-success"
      @click="importOpenApi"
      :disabled="!importButtonEnabled"
    >
      Import OpenAPI definition
    </button>
  </div>
  <div v-if="stubsYaml" class="mb-2">The following stubs will be added.</div>
  <div v-if="stubsYaml" class="mb-2">
    <button class="btn btn-success me-2" @click="saveStubs">Save stubs</button>
    <button class="btn btn-success me-2" @click="editBeforeSaving">
      Edit stubs before saving
    </button>
    <button class="btn btn-danger me-2" @click="reset">Reset</button>
  </div>
  <div v-if="stubsYaml" class="mb-2">
    <pre ref="codeBlock" class="language-yaml">{{ stubsYaml }}</pre>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { useRouter } from "vue-router";
import { computed, onMounted, onUnmounted, ref } from "vue";
import { resources } from "@/constants/resources";
import yaml from "js-yaml";
import hljs from "highlight.js/lib/core";
import { handleHttpError } from "@/utils/error";
import toastr from "toastr";
import { setIntermediateStub } from "@/utils/session";
import { shouldSave } from "@/utils/event";

export default {
  name: "ImportOpenApi",
  setup() {
    const store = useStore();
    const router = useRouter();

    // Refs
    const codeBlock = ref(null);

    // Data
    const openApiInput = ref("");
    const stubsYaml = ref("");

    // Computed
    const importButtonEnabled = computed(() => !!openApiInput.value);

    // Methods
    const insertExample = () => {
      openApiInput.value = resources.exampleOpenApiInput;
    };
    const importOpenApi = async () => {
      try {
        const result = await store.dispatch("importModule/importOpenApi", {
          openapi: openApiInput.value,
          doNotCreateStub: true,
        });

        const filteredResult = result.map((r) => r.stub);
        stubsYaml.value = yaml.dump(filteredResult);
        setTimeout(() => {
          console.log(codeBlock.value);
          if (codeBlock.value) {
            hljs.highlightElement(codeBlock.value);
          }
        }, 10);
      } catch (e) {
        handleHttpError(e);
      }
    };
    const onUploaded = (file) => {
      openApiInput.value = file.result;
    };
    const saveStubs = async () => {
      try {
        await store.dispatch("importModule/importOpenApi", {
          openapi: openApiInput.value,
          doNotCreateStub: false,
        });
        toastr.success(resources.stubsAddedSuccessfully);
        await router.push({ name: "Stubs" });
      } catch (e) {
        handleHttpError(e);
      }
    };
    const editBeforeSaving = () => {
      setIntermediateStub(stubsYaml.value);
      router.push({ name: "StubForm" });
    };
    const reset = () => {
      openApiInput.value = "";
      stubsYaml.value = "";
    };

    // Lifecycle
    const handleSave = async (e) => {
      if (shouldSave(e)) {
        e.preventDefault();
        if (!stubsYaml.value) {
          await importOpenApi();
        } else {
          await saveStubs();
        }
      }
    };
    const keydownEventListener = async (e) => await handleSave(e);
    onMounted(() => document.addEventListener("keydown", keydownEventListener));
    onUnmounted(() =>
      document.removeEventListener("keydown", keydownEventListener)
    );

    return {
      codeBlock,
      openApiInput,
      stubsYaml,
      insertExample,
      onUploaded,
      importButtonEnabled,
      importOpenApi,
      saveStubs,
      editBeforeSaving,
      reset,
    };
  },
};
</script>

<style scoped>
textarea {
  font-family: monospace;
  white-space: pre;
  overflow-wrap: normal;
  overflow-x: scroll;
  min-height: 300px;
}
</style>
