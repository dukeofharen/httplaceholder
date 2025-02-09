<template>
  <div class="response-body-form">
    <div v-if="showResponseBodyTypeDropdown">
      <div class="hint">{{ $translate("stubForm.responseBodyHint") }}</div>

      <div class="mt-2">
        <select class="form-select" v-model="responseBodyType">
          <option value="">
            {{ $translate("stubForm.responseBodySelectType") }}
          </option>
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
        <p v-html="$translateWithMarkdown('stubForm.responseBodyBase64Hint')" />
      </div>
      <upload-button
        :button-text="$translate('stubForm.responseBodyUploadAFile')"
        @uploaded="onUploaded"
        :result-type="UploadButtonType.Base64"
      />
      <button class="btn btn-primary" @click="showBase64TextInput = true">
        {{ $translate("stubForm.responseBodyShowTextInput") }}
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
        <label class="form-check-label" for="enableDynamicMode">{{
          $translate("stubForm.responseBodyEnableDynamicMode")
        }}</label>
      </div>
      <div v-if="showVariableParsers" class="mt-2">
        <div class="hint mb-2">
          {{ $translate("stubFormHelperDescriptions.dynamicMode") }}
        </div>
        <VariableHandlerSelector
          :variable-parser-items="variableParserItems"
          @exampleSelected="insertVariableHandlerExample($event)"
        />
      </div>
    </div>

    <div v-if="showResponseBody">
      <code-editor
        ref="codeEditor"
        :language="language"
        v-model="responseBody"
      />
    </div>

    <div v-if="responseBodyType === ResponseBodyType.json">
      <button class="btn btn-primary me-2" @click="prettifyJson">
        {{ $translate("stubForm.responseBodyPrettifyJson") }}
      </button>
      <button class="btn btn-primary" @click="minifyJson">
        {{ $translate("stubForm.responseBodyMinifyJson") }}
      </button>
    </div>

    <div v-if="responseBodyType === ResponseBodyType.xml">
      <button class="btn btn-primary me-2" @click="prettifyXml">
        {{ $translate("stubForm.responseBodyPrettifyXml") }}
      </button>
      <button class="btn btn-primary" @click="minifyXml">
        {{ $translate("stubForm.responseBodyMinifyXml") }}
      </button>
    </div>

    <div>
      <button class="btn btn-danger" @click="close">
        {{ $translate("general.close") }}
      </button>
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
import { UploadButtonType } from "@/domain/upload-button-type";
import { warning } from "@/utils/toast";
import xmlFormatter from "xml-formatter";
import VariableHandlerSelector from "@/components/stubForm/formHelper/VariableHandlerSelector.vue";
import { vsprintf } from "sprintf-js";
import { translate } from "@/utils/translate";

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
    const showBase64TextInput = ref(false);
    const responseBodyTypeItems = getValues();
    const metadata = ref<MetadataModel>();
    const selectedVariableHandler = ref("");
    const showExamples = ref(false);
    const language = ref("");
    let setInputTimeout: any;

    // Computed
    const showDynamicModeRow = computed(
      () => responseBodyType.value !== ResponseBodyType.base64,
    );
    const showVariableParsers = computed(() => showDynamicModeRow.value);
    const showResponseBodyTypeDropdown = computed(
      () => !props.presetResponseBodyType,
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
        showBase64TextInput.value,
    );
    const enableDynamicMode = computed({
      get: () => stubFormStore.getDynamicMode,
      set: (value) => stubFormStore.setDynamicMode(value),
    });

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
      if (example && codeEditor.value && codeEditor.value.replaceSelection) {
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
          spaces,
        );
      } catch (e) {
        warning(vsprintf(translate("errors.errorFormattingJson"), [e]));
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
        warning(vsprintf(translate("errors.errorFormattingXml"), [e]));
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
      switch (responseBodyType.value) {
        case ResponseBodyType.html:
          language.value = "html";
          break;
        case ResponseBodyType.xml:
          language.value = "xml";
          break;
        case ResponseBodyType.json:
          language.value = "json";
          break;
        default:
          language.value = "";
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
      codeEditor,
      ResponseBodyType,
      UploadButtonType,
      prettifyJson,
      minifyJson,
      prettifyXml,
      minifyXml,
      showExamples,
      insertVariableHandlerExample,
      language,
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
