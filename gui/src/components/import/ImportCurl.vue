<template>
  <div class="mb-2 col-md-6">{{ $translate("importCurl.intro") }}</div>
  <div v-if="!stubsPreviewOpened">
    <div class="mb-2">
      <button
        class="btn btn-outline-primary btn-sm"
        @click="howToOpen = !howToOpen"
      >
        {{ $translate("importStubs.howTo") }}
      </button>
    </div>
    <div v-if="howToOpen">
      <div class="row">
        <div class="col-md-12">
          <p>{{ $translate("importCurl.howToLine1") }}</p>
          <p>{{ $translate("importCurl.howToLine2") }}</p>
        </div>
      </div>
      <div class="row mb-2">
        <div class="col-md-4">
          <img src="@/assets/curl_copy_firefox.png" />
          <em>{{ $translate("importCurl.howToImage1") }}</em>
        </div>
        <div class="col-md-4">
          <img src="@/assets/curl_copy_chrome.png" />
          <em>{{ $translate("importCurl.howToImage2") }}</em>
        </div>
        <div class="col-md-12 mb-2 mt-2">
          <button class="btn btn-outline-primary btn-sm" @click="insertExample">
            {{ $translate("importStubs.insertExample") }}
          </button>
        </div>
      </div>
    </div>
    <div class="mb-2">
      <upload-button
        :button-text="$translate('importStubs.uploadFile')"
        @uploaded="onUploaded"
      />
    </div>
    <div class="mb-2">
      <input
        type="text"
        class="form-control"
        :placeholder="$translate('importStubs.tenantPlaceholder')"
        v-model="tenant"
      />
    </div>
    <div class="mb-2">
      <input
        type="text"
        class="form-control"
        :placeholder="$translate('importStubs.stubIdPrefixPlaceholder')"
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
        {{ $translate("importCurl.importCurlCommands") }}
      </button>
    </div>
  </div>
  <div v-else>
    <div v-if="stubsYaml" class="mb-2">
      {{ $translate("importStubs.stubsWillBeAdded") }}
    </div>
    <div v-if="stubsYaml" class="mb-2">
      <button class="btn btn-success me-2" @click="saveStubs">
        {{ $translate("importStubs.saveStubs") }}
      </button>
      <button class="btn btn-success me-2" @click="editBeforeSaving">
        {{ $translate("importStubs.editStubsBeforeSaving") }}
      </button>
      <button class="btn btn-danger me-2" @click="reset">
        {{ $translate("general.reset") }}
      </button>
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
import { setIntermediateStub } from "@/utils/session";
import { shouldSave } from "@/utils/event";
import { useRouter } from "vue-router";
import { error, success } from "@/utils/toast";
import { type ImportInputModel, useImportStore } from "@/store/import";
import { defineComponent } from "vue";
import type { FileUploadedModel } from "@/domain/file-uploaded-model";
import { translate } from "@/utils/translate";
import { exampleCurlInput } from "@/strings/examples";

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
          error(translate("importCurl.noCurlStubsFound"));
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
        success(translate("importStubs.stubsAddedSuccessfully"));
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
      input.value = exampleCurlInput;
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
  min-height: 300px;
}

img {
  width: 100%;
}
</style>
