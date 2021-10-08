<template>
  <div class="hint">
    Select a type of response and fill in the actual response that should be
    returned and press "Insert".
  </div>

  <div class="mt-3">
    <select class="form-select" v-model="responseBodyType">
      <option value="">Select a response type...</option>
      <option v-for="item in responseBodyTypeItems" :key="item" :value="item">
        {{ item }}
      </option>
    </select>
  </div>

  <div v-if="responseBodyType === responseBodyTypes.base64" class="mt-3">
    <div>
      <input type="file" name="file" ref="uploadField" @change="upload" />
      <div class="hint">
        You can upload a <strong>file</strong> for use in the Base64 response or
        click on "show text input" and insert <strong>plain text</strong> that
        will be encoded to Base64 on inserting.
      </div>
      <button class="btn btn-primary me-2" @click="uploadClick">
        Upload a file
      </button>
      <button class="btn btn-primary" @click="showBase64TextInput = true">
        Show text input
      </button>
    </div>
  </div>

  <div class="mt-3" v-if="showDynamicModeRow">
    <div class="hint">{{ elementDescriptions.dynamicMode }}</div>
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

  <div v-if="showResponseBody" class="mt-3">
    <textarea
      class="form-control"
      v-model="responseBody"
      ref="responseBodyField"
      placeholder="Fill in the response..."
    ></textarea>
  </div>

  <div class="mt-3">
    <button class="btn btn-success me-2" @click="insert">Insert</button>
    <button class="btn btn-danger" @click="close">Close</button>
  </div>
</template>

<script>
import {
  responseBodyTypes,
  elementDescriptions,
} from "@/constants/stubFormResources";
import { computed, onMounted, ref } from "vue";
import { useStore } from "vuex";
import { handleHttpError } from "@/utils/error";

export default {
  name: "ResponseBodyHelper",
  setup() {
    const store = useStore();

    // Refs
    const uploadField = ref(null);
    const responseBodyField = ref(null);

    // Data
    const responseBodyType = ref("");
    const responseBody = ref("");
    const enableDynamicMode = ref(false);
    const showBase64TextInput = ref(false);
    const responseBodyTypeItems = Object.keys(responseBodyTypes).map(
      (k) => responseBodyTypes[k]
    );
    const metadata = ref(null);
    const selectedVariableHandler = ref("");

    // Computed
    const showDynamicModeRow = computed(
      () => responseBodyType.value !== responseBodyTypes.base64
    );
    const showVariableParsers = computed(
      () => showDynamicModeRow.value && enableDynamicMode.value
    );
    const variableParserItems = computed(() => {
      if (!metadata.value || !metadata.value.variableHandlers) {
        return [];
      }

      return metadata.value.variableHandlers.map((h) => ({
        key: h.name,
        name: h.fullName,
      }));
    });
    const showResponseBody = computed(
      () =>
        responseBodyType.value !== responseBodyTypes.base64 ||
        showBase64TextInput.value
    );

    // Methods
    const upload = (ev) => {
      const files = Array.from(ev.target.files);
      const file = files[0];
      const reader = new FileReader();
      const regex = /^data:(.+);base64,(.*)$/;
      reader.onload = (e) => {
        const matches = e.target.result.match(regex);
        const contentType = matches[1];
        const body = matches[2];
        store.commit("stubForm/setResponseContentType", contentType);
        store.commit("stubForm/setResponseBody", {
          type: responseBodyTypes.base64,
          body,
        });
        responseBody.value = body;
        store.commit("stubForm/closeFormHelper");
        showBase64TextInput.value = false;
      };
      reader.readAsDataURL(file);
    };
    const uploadClick = () => {
      uploadField.value.click();
    };
    const insertVariableHandler = () => {
      const handler = metadata.value.variableHandlers.find(
        (h) => h.name === selectedVariableHandler.value
      );
      setTimeout(() => (selectedVariableHandler.value = ""), 10);
      const cursorPosition = responseBodyField.value.selectionStart;
      responseBody.value = [
        responseBody.value.slice(0, cursorPosition),
        handler.example,
        responseBody.value.slice(cursorPosition),
      ].join("");
    };
    const close = () => {
      store.commit("stubForm/closeFormHelper");
    };
    const insert = () => {
      let responseBodyResult = responseBody.value;
      if (responseBodyType.value === responseBodyTypes.base64) {
        responseBodyResult = btoa(responseBodyResult);
      }

      store.commit("stubForm/setResponseBody", {
        type: responseBodyType.value,
        body: responseBodyResult,
      });
      store.commit("stubForm/setDynamicMode", enableDynamicMode.value);
      showBase64TextInput.value = false;
      close();
    };

    // Lifecycle
    onMounted(async () => {
      responseBodyType.value = store.getters["stubForm/getResponseBodyType"];
      let currentResponseBody = store.getters["stubForm/getResponseBody"];
      if (responseBodyType.value === responseBodyTypes.base64) {
        currentResponseBody = atob(currentResponseBody);
      }

      responseBody.value = currentResponseBody;
      try {
        metadata.value = await store.dispatch("metadata/getMetadata");
      } catch (e) {
        handleHttpError(e);
      }
      enableDynamicMode.value = store.getters["stubForm/getDynamicMode"];
    });

    return {
      responseBodyType,
      enableDynamicMode,
      responseBodyTypeItems,
      showDynamicModeRow,
      responseBodyTypes,
      uploadField,
      upload,
      elementDescriptions,
      showBase64TextInput,
      uploadClick,
      showVariableParsers,
      selectedVariableHandler,
      variableParserItems,
      responseBody,
      showResponseBody,
      insert,
      insertVariableHandler,
      responseBodyField,
    };
  },
};
</script>

<style scoped>
input[type="file"] {
  display: none;
}

.hint {
  font-size: 0.9em;
}
</style>
