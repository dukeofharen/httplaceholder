<template>
  <div class="response-body-form">
    <div v-if="showResponseBodyTypeDropdown">
      <div class="hint">
        Select a type of response and fill in the actual response that should be
        returned and press "Insert".
      </div>

      <div class="mt-2">
        <select class="form-select" v-model="responseBodyType">
          <option value="">Select a response type...</option>
          <option
            v-for="item in responseBodyTypeItems"
            :key="item"
            :value="item"
          >
            {{ item }}
          </option>
        </select>
      </div>
    </div>

    <div v-if="responseBodyType === responseBodyTypes.base64">
      <div>
        <div class="hint">
          You can upload a <strong>file</strong> for use in the Base64 response
          or click on "show text input" and insert
          <strong>plain text</strong> that will be encoded to Base64 on
          inserting.
        </div>
        <upload-button
          button-text="Upload a file"
          @uploaded="onUploaded"
          result-type="base64"
        />
        <button class="btn btn-primary" @click="showBase64TextInput = true">
          Show text input
        </button>
      </div>
    </div>

    <div v-if="showDynamicModeRow">
      <div class="form-check mt-2">
        <input
          class="form-check-input"
          type="checkbox"
          v-model="enableDynamicMode"
          id="enableDynamicMode"
        />
        <label class="form-check-label" for="enableDynamicMode"
          >Enable dynamic mode</label
        >
      </div>
      <div v-if="showVariableParsers" class="mt-2">
        <div class="hint mb-2">{{ elementDescriptions.dynamicMode }}</div>
        <select
          class="form-select"
          v-model="selectedVariableHandler"
          @change="insertVariableHandler"
        >
          <option value="">
            Select a variable handler to insert in the response...
          </option>
          <option
            v-for="item of variableParserItems"
            :key="item.key"
            :value="item.key"
          >
            {{ item.name }}
          </option>
        </select>
      </div>
    </div>

    <div v-if="showResponseBody">
      <codemirror
        ref="codeEditor"
        v-model="responseBody"
        :options="cmOptions"
      />
    </div>

    <div>
      <button class="btn btn-danger" @click="close">Close</button>
    </div>
  </div>
</template>

<script>
import {
  elementDescriptions,
  responseBodyTypes,
} from "@/constants/stubFormResources";
import { computed, onMounted, ref, watch } from "vue";
import { handleHttpError } from "@/utils/error";
import { fromBase64, toBase64 } from "@/utils/text";
import { useMetadataStore } from "@/store/metadata";
import { useStubFormStore } from "@/store/stubForm";
import { defineComponent } from "vue";

export default defineComponent({
  name: "ResponseBodyHelper",
  props: {
    presetResponseBodyType: {
      type: String,
    },
  },
  setup(props) {
    const metadataStore = useMetadataStore();
    const stubFormStore = useStubFormStore();

    // Refs
    const codeEditor = ref(null);

    // Data
    const responseBodyType = ref("");
    const responseBody = ref("");
    const enableDynamicMode = ref(null);
    const showBase64TextInput = ref(false);
    const responseBodyTypeItems = Object.keys(responseBodyTypes).map(
      (k) => responseBodyTypes[k]
    );
    const metadata = ref(null);
    const selectedVariableHandler = ref("");
    const cmOptions = ref({
      tabSize: 4,
      mode: "",
      lineNumbers: true,
      line: true,
    });
    let setInputTimeout = null;

    // Computed
    const showDynamicModeRow = computed(
      () => responseBodyType.value !== responseBodyTypes.base64
    );
    const showVariableParsers = computed(
      () => showDynamicModeRow.value && enableDynamicMode.value
    );
    const showResponseBodyTypeDropdown = computed(
      () => !props.presetResponseBodyType
    );
    const variableParserItems = computed(() => {
      if (!metadata.value || !metadata.value.variableHandlers) {
        return [];
      }

      const result = metadata.value.variableHandlers.map((h) => ({
        key: h.name,
        name: h.fullName,
        example: h.example,
      }));

      result.sort((a, b) => {
        if (a.name > b.name) return 1;
        if (a.name < b.name) return -1;
        return 0;
      });

      return result;
    });
    const showResponseBody = computed(
      () =>
        responseBodyType.value !== responseBodyTypes.base64 ||
        showBase64TextInput.value
    );

    // Methods
    const onUploaded = (file) => {
      const regex = /^data:(.+);base64,(.*)$/;
      const matches = file.result.match(regex);
      const contentType = matches[1];
      const body = matches[2];
      stubFormStore.setResponseContentType(contentType);
      stubFormStore.setResponseBody({
        type: responseBodyTypes.base64,
        body,
      });
      responseBody.value = body;
      stubFormStore.closeFormHelper();
      showBase64TextInput.value = false;
    };
    const insertVariableHandler = () => {
      if (codeEditor.value && codeEditor.value.replaceSelection) {
        const handler = metadata.value.variableHandlers.find(
          (h) => h.name === selectedVariableHandler.value
        );
        setTimeout(() => (selectedVariableHandler.value = ""), 10);
        codeEditor.value.replaceSelection(handler.example);
      }
    };
    const insert = () => {
      let responseBodyResult = responseBody.value;
      if (responseBodyType.value === responseBodyTypes.base64) {
        responseBodyResult = toBase64(responseBodyResult);
      }

      stubFormStore.setResponseBody({
        type: responseBodyType.value,
        body: responseBodyResult,
      });
      stubFormStore.setDynamicMode(enableDynamicMode.value);
    };
    const close = () => {
      insert();
      stubFormStore.closeFormHelper();
    };

    // Lifecycle
    onMounted(async () => {
      responseBodyType.value =
        props.presetResponseBodyType || stubFormStore.getResponseBodyType;
      let currentResponseBody = stubFormStore.getResponseBody;
      if (responseBodyType.value === responseBodyTypes.base64) {
        const decodedBase64 = fromBase64(currentResponseBody);
        if (decodedBase64) {
          currentResponseBody = decodedBase64;
        }
      }

      responseBody.value = currentResponseBody;
      try {
        metadata.value = await metadataStore.getMetadata();
      } catch (e) {
        handleHttpError(e);
      }

      enableDynamicMode.value = stubFormStore.getDynamicMode;
    });

    // Watch
    watch(responseBodyType, () => {
      cmOptions.value.htmlMode = false;
      cmOptions.value.mode = "";
      switch (responseBodyType.value) {
        case responseBodyTypes.html:
          cmOptions.value.htmlMode = true;
          cmOptions.value.mode = "text/html";
          break;
        case responseBodyTypes.xml:
          cmOptions.value.htmlMode = false;
          cmOptions.value.mode = "application/xml";
          break;
        case responseBodyTypes.json:
          cmOptions.value.mode = { name: "javascript", json: true };
          break;
      }
    });
    watch(responseBody, () => {
      if (setInputTimeout) {
        clearTimeout(setInputTimeout);
      }

      setInputTimeout = setTimeout(() => insert(), 100);
    });

    return {
      responseBodyType,
      enableDynamicMode,
      responseBodyTypeItems,
      showDynamicModeRow,
      responseBodyTypes,
      elementDescriptions,
      showBase64TextInput,
      showVariableParsers,
      selectedVariableHandler,
      variableParserItems,
      responseBody,
      showResponseBody,
      insert,
      insertVariableHandler,
      close,
      onUploaded,
      showResponseBodyTypeDropdown,
      cmOptions,
      codeEditor,
    };
  },
});
</script>

<style scoped>
.hint {
  font-size: 0.9em;
}

.response-body-form > div:not(:first-child) {
  margin-top: 1rem !important;
}
</style>
