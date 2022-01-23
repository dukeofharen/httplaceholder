<template>
  <div class="mb-2 col-md-6">
    Using this form, you can create stubs based on an HTTP archive (or HAR).
    Most modern browsers allow you to download a HAR file with the request and
    response definitions of the recently made requests.
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
            To get the HTTP archive of the requests from your browser, you need
            to open the developer tools and open the "Network" tab.
          </p>
          <p>
            In Firefox, you can right click on the request in the "Network" tab
            and select "Copy all as HAR".
          </p>
          <p>
            In Chrome, you can also click "Copy all as HAR", but this does not
            copy the response contents. To get the full responses, you need to
            click "Save all as HAR with content" to get the full HAR.
          </p>
          <p>You can copy the full HAR file below.</p>
        </div>
      </div>
      <div class="row mb-2">
        <div class="col-md-4">
          <img src="@/assets/har_copy_firefox.png" />
          <em>Example in Firefox</em>
        </div>
        <div class="col-md-4">
          <img src="@/assets/har_copy_chrome.png" />
          <em>Example in Chrome. </em>
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
        @click="importHar"
        :disabled="!importButtonEnabled"
      >
        Import HTTP archive
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
import { computed, onMounted, onUnmounted, ref } from "vue";
import { handleHttpError } from "@/utils/error";
import { useStore } from "vuex";
import yaml from "js-yaml";
import { resources } from "@/constants/resources";
import { useRouter } from "vue-router";
import { setIntermediateStub } from "@/utils/session";
import { shouldSave } from "@/utils/event";
import { success } from "@/utils/toast";

export default {
  name: "ImportHar",
  setup() {
    const store = useStore();
    const router = useRouter();

    // Data
    const input = ref("");
    const howToOpen = ref(false);
    const stubsYaml = ref("");
    const tenant = ref("");

    // Computed
    const importButtonEnabled = computed(() => !!input.value);
    const stubsPreviewOpened = computed(() => !!stubsYaml.value);

    // Methods
    const insertExample = () => {
      input.value = resources.exampleHarInput;
      howToOpen.value = false;
    };
    const importHar = async () => {
      try {
        const result = await store.dispatch("importModule/importHar", {
          har: input.value,
          doNotCreateStub: true,
          tenant: tenant.value,
        });

        const filteredResult = result.map((r) => r.stub);
        stubsYaml.value = yaml.dump(filteredResult);
      } catch (e) {
        handleHttpError(e);
      }
    };
    const saveStubs = async () => {
      try {
        await store.dispatch("importModule/importHar", {
          har: input.value,
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
    const onUploaded = (file) => {
      input.value = file.result;
    };

    // Lifecycle
    const handleSave = async (e) => {
      if (shouldSave(e)) {
        e.preventDefault();
        if (!stubsYaml.value) {
          await importHar();
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
      howToOpen,
      insertExample,
      stubsYaml,
      input,
      importHar,
      importButtonEnabled,
      saveStubs,
      editBeforeSaving,
      reset,
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
