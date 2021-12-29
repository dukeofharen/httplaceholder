<template>
  <div class="mb-2">
    Using this form, you can create stubs based on an HTTP archive (or HAR).
    Most modern browsers allow you to download a HAR file with the request and
    response definitions of the recently made requests.
  </div>
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
          To get the HTTP archive of the requests from your browser, you need to
          open the developer tools and open the "Network" tab.
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
        <em
          >Example in Chrome. In Chrome, you can either select "Copy as cURL" or
          "Copy all as cURL".
        </em>
      </div>
      <div class="col-md-12 mb-2 mt-2">
        <button class="btn btn-outline-primary btn-sm" @click="insertExample">
          Insert example
        </button>
      </div>
    </div>
  </div>
  <div class="mb-2" v-if="!stubsYaml">
    <textarea class="form-control" v-model="harInput"></textarea>
  </div>
  <div v-if="!stubsYaml" class="mb-2">
    <button
      class="btn btn-success"
      @click="importHar"
      :disabled="!importButtonEnabled"
    >
      Import HTTP archive
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
import { computed, ref } from "vue";
import { handleHttpError } from "@/utils/error";
import { useStore } from "vuex";
import yaml from "js-yaml";
import hljs from "highlight.js/lib/core";
import toastr from "toastr";
import { resources } from "@/constants/resources";
import { useRouter } from "vue-router";
import { setIntermediateStub } from "@/utils/session";

export default {
  name: "ImportHar",
  setup() {
    const store = useStore();
    const router = useRouter();

    // Refs
    const codeBlock = ref(null);

    // Data
    const harInput = ref("");
    const howToOpen = ref(false);
    const stubsYaml = ref("");

    // Computed
    const importButtonEnabled = computed(() => !!harInput.value);

    // Methods
    const insertExample = () => {
      harInput.value = resources.exampleHarInput;
    };
    const importHar = async () => {
      try {
        const result = await store.dispatch("importModule/importHar", {
          har: harInput.value,
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
    const saveStubs = async () => {
      try {
        await store.dispatch("importModule/importHar", {
          har: harInput.value,
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
      harInput.value = "";
      stubsYaml.value = "";
    };

    return {
      howToOpen,
      insertExample,
      stubsYaml,
      harInput,
      importHar,
      importButtonEnabled,
      saveStubs,
      editBeforeSaving,
      reset,
      codeBlock,
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
