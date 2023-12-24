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
      <input
        type="text"
        class="form-control"
        placeholder="Fill in a stub ID prefix here if desired... (every stub ID will be prefixed with this text)"
        v-model="stubIdPrefix"
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

<script lang="ts">
import { useRouter } from "vue-router";
import { computed, onMounted, onUnmounted, ref } from "vue";
import { resources } from "@/constants/resources";
import yaml from "js-yaml";
import { handleHttpError } from "@/utils/error";
import { setIntermediateStub } from "@/utils/session";
import { shouldSave } from "@/utils/event";
import { success } from "@/utils/toast";
import { type ImportInputModel, useImportStore } from "@/store/import";
import { defineComponent } from "vue";
import type { FileUploadedModel } from "@/domain/file-uploaded-model";

export default defineComponent({
  name: "ImportOpenApi",
  setup() {
    const importStore = useImportStore();
    const router = useRouter();

    // Data
    const input = ref("");
    const stubsYaml = ref("");
    const tenant = ref("");
    const stubIdPrefix = ref("");

    // Computed
    const importButtonEnabled = computed(() => !!input.value);
    const stubsPreviewOpened = computed(() => !!stubsYaml.value);

    // Functions
    const buildInputModel = (doNotCreateStub: boolean): ImportInputModel => {
      return {
        doNotCreateStub: doNotCreateStub,
        tenant: tenant.value,
        input: input.value,
        stubIdPrefix: stubIdPrefix.value,
      };
    };

    // Methods
    const insertExample = () => {
      input.value = resources.exampleOpenApiInput;
    };
    const importOpenApi = async () => {
      try {
        const importInput = buildInputModel(true);
        const result = await importStore.importOpenApi(importInput);

        const filteredResult = result.map((r) => r.stub);
        stubsYaml.value = yaml.dump(filteredResult);
      } catch (e) {
        handleHttpError(e);
      }
    };
    const onUploaded = (file: FileUploadedModel) => {
      input.value = file.result;
    };
    const saveStubs = async () => {
      try {
        const importInput = buildInputModel(false);
        await importStore.importOpenApi(importInput);
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
    const handleSave = async (e: KeyboardEvent) => {
      if (shouldSave(e)) {
        e.preventDefault();
        if (!stubsYaml.value) {
          await importOpenApi();
        } else {
          await saveStubs();
        }
      }
    };
    const keydownEventListener = async (e: KeyboardEvent) =>
      await handleSave(e);
    onMounted(() => document.addEventListener("keydown", keydownEventListener));
    onUnmounted(() =>
      document.removeEventListener("keydown", keydownEventListener),
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
      stubIdPrefix,
    };
  },
});
</script>

<style scoped>
textarea {
  min-height: 300px;
}
</style>
