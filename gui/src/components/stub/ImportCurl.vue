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

<script>
import { computed, onMounted, onUnmounted, ref } from "vue";
import { useStore } from "vuex";
import { handleHttpError } from "@/utils/error";
import yaml from "js-yaml";
import { resources } from "@/constants/resources";
import { setIntermediateStub } from "@/utils/session";
import { shouldSave } from "@/utils/event";
import { useRouter } from "vue-router";
import { error, success } from "@/utils/toast";

export default {
  name: "ImportCurl",
  setup() {
    const store = useStore();
    const router = useRouter();

    // Data
    const input = ref("");
    const stubsYaml = ref("");
    const howToOpen = ref(false);
    const tenant = ref("");

    // Computed
    const importButtonEnabled = computed(() => !!input.value);
    const stubsPreviewOpened = computed(() => !!stubsYaml.value);

    // Methods
    const importCommands = async () => {
      try {
        const result = await store.dispatch("importModule/importCurlCommands", {
          commands: input.value,
          doNotCreateStub: true,
          tenant: tenant.value,
        });
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
        await store.dispatch("importModule/importCurlCommands", {
          commands: input.value,
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
    const insertExample = () => {
      input.value = resources.exampleCurlInput;
      howToOpen.value = false;
    };
    const onUploaded = (file) => {
      input.value = file.result;
    };

    // Lifecycle
    const handleSave = async (e) => {
      if (shouldSave(e)) {
        e.preventDefault();
        if (!stubsYaml.value) {
          await importCommands();
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

img {
  width: 100%;
}
</style>
