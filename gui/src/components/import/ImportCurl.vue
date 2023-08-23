<template>
  <div class="mb-2 col-md-6">
    Using this form, you can create stubs based on cURL commands. You can either
    use a cURL command you have lying around or you can copy/paste a cURL
    command from the developer console from your browser.
  </div>
  <div v-if="!stubsPreviewOpened">
    <div class="mb-2">
      <button
        class="btn btn-outline-primary btn-sm"
        @click="howToOpen = !howToOpen"
      >
        How to
      </button>
    </div>
    <div v-if="howToOpen">
      <div class="row">
        <div class="col-md-12">
          <p>
            You can copy/paste a cURL command from your browser. In most popular
            web browsers, you can do this by going to the developer tools, going
            to the "Network" tab and selecting the request where you would like
            to have the cURL request for.
          </p>
          <p>
            When copying cURL requests from a browser on Windows, make sure you
            select "Copy as cURL (bash)" or "Copy all as cURL (bash)" on Chrome
            or "Copy as cURL (POSIX)" in Firefox. The Windows formatting of cURL
            commands is currently not supported in HttPlaceholder.
          </p>
        </div>
      </div>
      <div class="row mb-2">
        <div class="col-md-4">
          <img src="@/assets/curl_copy_firefox.png" />
          <em>Example in Firefox</em>
        </div>
        <div class="col-md-4">
          <img src="@/assets/curl_copy_chrome.png" />
          <em
            >Example in Chrome. In Chrome, you can either select "Copy as cURL"
            or "Copy all as cURL".
          </em>
        </div>
        <div class="col-md-12 mb-2 mt-2">
          <button class="btn btn-outline-primary btn-sm" @click="insertExample">
            Insert example
          </button>
        </div>
      </div>
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
        @click="importCommands"
        :disabled="!importButtonEnabled"
      >
        Import cURL command(s)
      </button>
    </div>
  </div>
  <div v-else>
    <div v-if="stubsYaml" class="mb-2">The following stubs will be added.</div>
    <div v-if="stubsYaml" class="mb-2">
      <button class="btn btn-success me-2" @click="saveStubs">
        Save stubs
      </button>
      <button class="btn btn-success me-2" @click="editBeforeSaving">
        Edit stubs before saving
      </button>
      <button class="btn btn-danger me-2" @click="reset">Reset</button>
    </div>
    <div v-if="stubsYaml" class="mb-2">
      <code-highlight language="yaml" :code="stubsYaml" />
    </div>
  </div>
</template>

<script lang="ts">
import { computed, onMounted, onUnmounted, ref } from "vue";
import { handleHttpError } from "@/utils/error";
import yaml from "js-yaml";
import { resources } from "@/constants/resources";
import { setIntermediateStub } from "@/utils/session";
import { shouldSave } from "@/utils/event";
import { useRouter } from "vue-router";
import { error, success } from "@/utils/toast";
import { type ImportInputModel, useImportStore } from "@/store/import";
import { defineComponent } from "vue";
import type { FileUploadedModel } from "@/domain/file-uploaded-model";

export default defineComponent({
  name: "ImportCurl",
  setup() {
    const importStore = useImportStore();
    const router = useRouter();

    // Data
    const input = ref("");
    const stubsYaml = ref("");
    const howToOpen = ref(false);
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
    const importCommands = async () => {
      try {
        const importInput: ImportInputModel = buildInputModel(true);
        const result = await importStore.importCurlCommands(importInput);
        if (!result.length) {
          error(resources.noCurlStubsFound);
          return;
        }

        const filteredResult = result.map((r) => r.stub);
        stubsYaml.value = yaml.dump(filteredResult);
      } catch (e) {
        handleHttpError(e);
      }
    };
    const saveStubs = async () => {
      try {
        const importInput = buildInputModel(false);
        await importStore.importCurlCommands(importInput);
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
    const insertExample = () => {
      input.value = resources.exampleCurlInput;
      howToOpen.value = false;
    };
    const onUploaded = (file: FileUploadedModel) => {
      input.value = file.result;
    };

    // Lifecycle
    const handleSave = async (e: KeyboardEvent) => {
      if (shouldSave(e)) {
        e.preventDefault();
        if (!stubsYaml.value) {
          await importCommands();
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
      importCommands,
      stubsYaml,
      saveStubs,
      editBeforeSaving,
      reset,
      handleSave,
      importButtonEnabled,
      howToOpen,
      insertExample,
      onUploaded,
      tenant,
      stubsPreviewOpened,
      stubIdPrefix,
    };
  },
});
</script>

<style scoped>
textarea {
  font-family: monospace;
  white-space: pre;
  overflow-wrap: normal;
  overflow-x: scroll;
  min-height: 300px;
}

img {
  width: 100%;
}
</style>
