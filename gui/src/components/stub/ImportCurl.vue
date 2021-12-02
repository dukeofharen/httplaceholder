<template>
  <div class="mb-2">
    Using this form, you can create stubs based on cURL commands. You can either
    use a cURL command you have lying around or you can copy/paste a cURL
    command from the developer console from your browser.
  </div>
  <div class="mb-2" v-if="!stubsYaml">
    <textarea class="form-control" v-model="curlInput"></textarea>
  </div>
  <div v-if="!stubsYaml" class="mb-2">
    <button class="btn btn-success" @click="importCommands">
      Import cURL command(s)
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
import { onMounted, onUnmounted, ref } from "vue";
import { useStore } from "vuex";
import { handleHttpError } from "@/utils/error";
import yaml from "js-yaml";
import hljs from "highlight.js/lib/core";
import router from "@/router";
import toastr from "toastr";
import { resources } from "@/constants/resources";
import { setIntermediateStub } from "@/utils/session";
import { shouldSave } from "@/utils/event";

export default {
  name: "ImportCurl",
  setup() {
    const store = useStore();

    // Refs
    const codeBlock = ref(null);

    // Data
    const curlInput = ref("");
    const stubsYaml = ref("");

    // Methods
    const importCommands = async () => {
      try {
        const result = await store.dispatch("importModule/importCurlCommands", {
          commands: curlInput.value,
          doNotCreateStub: true,
        });
        const filteredResult = result.map((r) => r.stub);
        stubsYaml.value = yaml.dump(filteredResult);
        setTimeout(() => {
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
        await store.dispatch("importModule/importCurlCommands", {
          commands: curlInput.value,
          doNotCreateStub: false,
        });
        toastr.success(resources.stubsAddedSuccessfully);
        router.push({ name: "Stubs" });
      } catch (e) {
        handleHttpError(e);
      }
    };
    const editBeforeSaving = () => {
      setIntermediateStub(stubsYaml.value);
      router.push({ name: "StubForm" });
    };
    const reset = () => {
      curlInput.value = "";
      stubsYaml.value = "";
    };
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

    // Lifecycle
    const keydownEventListener = async (e) => await handleSave(e);
    onMounted(() => document.addEventListener("keydown", keydownEventListener));
    onUnmounted(() =>
      document.removeEventListener("keydown", keydownEventListener)
    );

    return {
      curlInput,
      importCommands,
      stubsYaml,
      codeBlock,
      saveStubs,
      editBeforeSaving,
      reset,
      handleSave,
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
