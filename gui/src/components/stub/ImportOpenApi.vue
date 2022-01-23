<template>
  <div class="mb-2 col-md-6">
    Using this form, you can create stubs based on an OpenAPI (or Swagger)
    definition. This definition can be provided in both JSON and YAML format.
    Many APIs are accompanied by an OpenAPI definition, so this is a great way
    to create stubs for the API.
  </div>
  <div v-if="!stubsPreviewOpened">
    <div class="mb-2">
      <button class="btn btn-outline-primary btn-sm" @click="insertExample">
        Insert example
      </button>
    </div>
    <div class="mb-2">
      <upload-button button-text="Upload file" @uploaded="onUploaded" />
    </div>
    <div class="mb-2">
      <input
        type="text"
        class="form-control"
        placeholder="Fill in a tenant to group the generated stubs... (if no tenant is provided, a tenant name will be generated)"
        v-model="tenant"
      />
    </div>
    <div class="mb-2">
      <textarea class="form-control" v-model="input"></textarea>
    </div>
    <div class="mb-2">
      <button
        class="btn btn-success"
        @click="importOpenApi"
        :disabled="!importButtonEnabled"
      >
        Import OpenAPI definition
      </button>
    </div>
  </div>
  <div v-else>
    <div class="mb-2">The following stubs will be added.</div>
    <div class="mb-2">
      <button class="btn btn-success me-2" @click="saveStubs">
        Save stubs
      </button>
      <button class="btn btn-success me-2" @click="editBeforeSaving">
        Edit stubs before saving
      </button>
      <button class="btn btn-danger me-2" @click="reset">Reset</button>
    </div>
    <div class="mb-2">
      <code-highlight language="yaml" :code="stubsYaml" />
    </div>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { useRouter } from "vue-router";
import { computed, onMounted, onUnmounted, ref } from "vue";
import { resources } from "@/constants/resources";
import yaml from "js-yaml";
import { handleHttpError } from "@/utils/error";
import { setIntermediateStub } from "@/utils/session";
import { shouldSave } from "@/utils/event";
import { success } from "@/utils/toast";

export default {
  name: "ImportOpenApi",
  setup() {
    const store = useStore();
    const router = useRouter();

    // Data
    const input = ref("");
    const stubsYaml = ref("");
    const tenant = ref("");

    // Computed
    const importButtonEnabled = computed(() => !!input.value);
    const stubsPreviewOpened = computed(() => !!stubsYaml.value);

    // Methods
    const insertExample = () => {
      input.value = resources.exampleOpenApiInput;
    };
    const importOpenApi = async () => {
      try {
        const result = await store.dispatch("importModule/importOpenApi", {
          openapi: input.value,
          doNotCreateStub: true,
          tenant: tenant.value,
        });

        const filteredResult = result.map((r) => r.stub);
        stubsYaml.value = yaml.dump(filteredResult);
      } catch (e) {
        handleHttpError(e);
      }
    };
    const onUploaded = (file) => {
      input.value = file.result;
    };
    const saveStubs = async () => {
      try {
        await store.dispatch("importModule/importOpenApi", {
          openapi: input.value,
          doNotCreateStub: false,
          tenant: tenant.value,
        });
        success(resources.stubsAddedSuccessfully);
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
      input.value = "";
      stubsYaml.value = "";
      tenant.value = "";
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
      input,
      stubsYaml,
      insertExample,
      onUploaded,
      importButtonEnabled,
      importOpenApi,
      saveStubs,
      editBeforeSaving,
      reset,
      tenant,
      stubsPreviewOpened,
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
