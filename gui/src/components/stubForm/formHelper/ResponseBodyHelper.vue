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

    <div v-if="responseBodyType === ResponseBodyType.base64">
      <div class="hint">
        You can upload a <strong>file</strong> for use in the Base64 response or
        click on "show text input" and insert <strong>plain text</strong> that
        will be encoded to Base64 on inserting.
      </div>
      <upload-button
        button-text="Upload a file"
        @uploaded="onUploaded"
        :result-type="UploadButtonType.Base64"
      />
      <button class="btn btn-primary" @click="showBase64TextInput = true">
        Show text input
      </button>
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
        <VariableHandlerSelector
          :variable-parser-items="variableParserItems"
          @exampleSelected="insertVariableHandlerExample($event)"
        />
      </div>
    </div>

    <div v-if="showResponseBody">
      <codemirror
        ref="codeEditor"
        v-model="responseBody"
        :options="cmOptions"
      />
    </div>

    <div v-if="responseBodyType === ResponseBodyType.json">
      <button class="btn btn-primary me-2" @click="prettifyJson">
        Prettify JSON
      </button>
      <button class="btn btn-primary" @click="minifyJson">Minify JSON</button>
    </div>

    <div v-if="responseBodyType === ResponseBodyType.xml">
      <button class="btn btn-primary me-2" @click="prettifyXml">
        Prettify XML
      </button>
      <button class="btn btn-primary" @click="minifyXml">Minify XML</button>
    </div>

    <div>
      <button class="btn btn-danger" @click="close">Close</button>
    </div>
  </div>
</template>

<script lang="ts">
import {
  computed,
  defineComponent,
  onMounted,
  type PropType,
  ref,
  watch,
} from "vue";
import { handleHttpError } from "@/utils/error";
import { fromBase64, toBase64 } from "@/utils/text";
import { useMetadataStore } from "@/store/metadata";
import { type SetResponseInput, useStubFormStore } from "@/store/stubForm";
import {
  getValues,
  ResponseBodyType,
} from "@/domain/stubForm/response-body-type";
import type { MetadataModel } from "@/domain/metadata/metadata-model";
import type { FileUploadedModel } from "@/domain/file-uploaded-model";
import { elementDescriptions } from "@/domain/stubForm/element-descriptions";
import { UploadButtonType } from "@/domain/upload-button-type";
import { warning } from "@/utils/toast";
import xmlFormatter from "xml-formatter";
import VariableHandlerSelector from "@/components/stubForm/formHelper/VariableHandlerSelector.vue";

export default defineComponent({
  name: "ResponseBodyHelper",
  components: { VariableHandlerSelector },
  props: {
    presetResponseBodyType: {
      type: String as PropType<ResponseBodyType>,
    },
  },
  setup(props) {
    const metadataStore = useMetadataStore();
    const stubFormStore = useStubFormStore();

    // Refs
    const codeEditor = ref<any>();

    // Data
    const responseBodyType = ref<ResponseBodyType>(ResponseBodyType.text);
    const responseBody = ref("");
    const enableDynamicMode = ref<boolean | undefined>();
    const showBase64TextInput = ref(false);
    const responseBodyTypeItems = getValues();
    const metadata = ref<MetadataModel>();
    const selectedVariableHandler = ref("");
    const showExamples = ref(false);
    const cmOptions = ref({
      tabSize: 4,
      mode: "" as any,
      lineNumbers: true,
      line: true,
      htmlMode: false,
    });
    let setInputTimeout: any;

    // Computed
    const showDynamicModeRow = computed(
      () => responseBodyType.value !== ResponseBodyType.base64
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

      return metadata.value.variableHandlers;
    });
    const showResponseBody = computed(
      () =>
        responseBodyType.value !== ResponseBodyType.base64 ||
        showBase64TextInput.value
    );

    // Methods
    const onUploaded = (file: FileUploadedModel) => {
      const regex = /^data:(.+);base64,(.*)$/;
      const matches = file.result.match(regex);
      const contentType = matches[1];
      const body = matches[2];
      stubFormStore.setResponseContentType(contentType);
      stubFormStore.setResponseBody({
        type: ResponseBodyType.base64,
        body,
      } as SetResponseInput);
      responseBody.value = body;
      stubFormStore.closeFormHelper();
      showBase64TextInput.value = false;
    };
    const insertVariableHandlerExample = (example: string) => {
      if (codeEditor.value && codeEditor.value.replaceSelection && example) {
        codeEditor.value.replaceSelection(example);
      }
    };
    const insert = () => {
      let responseBodyResult = responseBody.value;
      if (responseBodyType.value === ResponseBodyType.base64) {
        responseBodyResult = toBase64(responseBodyResult) as string;
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
    const formatJson = (spaces: number) => {
      try {
        responseBody.value = JSON.stringify(
          JSON.parse(responseBody.value),
          null,
          spaces
        );
      } catch (e) {
        warning(`Error occurred while formatting JSON: ${e}`);
      }
    };
    const prettifyJson = () => formatJson(2);
    const minifyJson = () => formatJson(0);

    const formatXml = (minify: boolean) => {
      try {
        const options = minify
          ? {
              indentation: "",
              lineSeparator: "",
            }
          : {};
        responseBody.value = xmlFormatter(responseBody.value, options);
      } catch (e) {
        warning(`Error occurred while formatting XML: ${e}`);
      }
    };
    const prettifyXml = () => formatXml(false);
    const minifyXml = () => formatXml(true);

    // Lifecycle
    onMounted(async () => {
      responseBodyType.value =
        props.presetResponseBodyType || stubFormStore.getResponseBodyType;
      let currentResponseBody = stubFormStore.getResponseBody;
      if (responseBodyType.value === ResponseBodyType.base64) {
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
        case ResponseBodyType.html:
          cmOptions.value.htmlMode = true;
          cmOptions.value.mode = "text/html";
          break;
        case ResponseBodyType.xml:
          cmOptions.value.htmlMode = false;
          cmOptions.value.mode = "application/xml";
          break;
        case ResponseBodyType.json:
          cmOptions.value.mode = { name: "javascript", json: true };
          break;
      }

      insert();
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
      showBase64TextInput,
      showVariableParsers,
      selectedVariableHandler,
      variableParserItems,
      responseBody,
      showResponseBody,
      insert,
      close,
      onUploaded,
      showResponseBodyTypeDropdown,
      cmOptions,
      codeEditor,
      ResponseBodyType,
      elementDescriptions,
      UploadButtonType,
      prettifyJson,
      minifyJson,
      prettifyXml,
      minifyXml,
      showExamples,
      insertVariableHandlerExample,
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
