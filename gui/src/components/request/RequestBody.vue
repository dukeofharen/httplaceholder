<template>
  <div class="card">
    <div class="card-body">
      <div class="row">
        <div class="col-md-12" v-if="bodyType">
          <button
            class="btn btn-sm me-2"
            :class="{
              'btn-outline-primary': showRenderedBody,
              'btn-outline-secondary': !showRenderedBody,
            }"
            @click="viewRenderedBody"
          >
            {{ bodyType }}
          </button>
          <button
            class="btn btn-sm me-2"
            :class="{
              'btn-outline-primary': !showRenderedBody,
              'btn-outline-secondary': showRenderedBody,
            }"
            @click="viewRawBody"
          >
            Raw
          </button>
        </div>
      </div>
      <div class="row mt-3" :class="{ 'show-more': showMore }">
        <div class="col-md-12" v-if="showRenderedBody">
          <code-highlight :language="language" :code="renderedBody" />
        </div>
        <div class="col-md-12" v-if="!showRenderedBody">
          <code-highlight :code="rawBody" />
        </div>
        <div v-if="showMore" @click="showMoreClick" class="show-more-button">
          <p class="text-center">
            <i class="bi bi-arrow-down-circle">&nbsp;</i>
          </p>
        </div>
      </div>
      <div class="row mt-3">
        <div class="col-md-12">
          <i
            class="bi bi-clipboard copy"
            title="Copy request body"
            @click="copy"
            >&nbsp;</i
          >
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, onMounted, type PropType, ref } from "vue";
import xmlFormatter from "xml-formatter";
import { formFormat } from "@/utils/form";
import { copyTextToClipboard } from "@/utils/clipboard";
import { resources } from "@/constants/resources";
import { success } from "@/utils/toast";
import { defineComponent } from "vue";
import type { RequestResultModel } from "@/domain/request/request-result-model";
import { countNewlineCharacters } from "@/utils/text";
import { requestBodyLineLimit } from "@/constants/technical";

const bodyTypes = {
  xml: "XML",
  json: "JSON",
  form: "Form",
};

export default defineComponent({
  name: "RequestBody",
  props: {
    request: {
      type: Object as PropType<RequestResultModel>,
      required: true,
    },
  },
  setup(props) {
    // Data
    const showRenderedBody = ref(false);
    const showMoreClicked = ref(false);

    // Functions
    const getHeaders = () => props.request.requestParameters.headers;
    const getBody = () => props.request.requestParameters.body;

    // Computed
    const bodyType = computed(() => {
      const headers = getHeaders();
      const contentTypeHeaderKey = Object.keys(headers).find(
        (k) => k.toLowerCase() === "content-type"
      );
      if (!contentTypeHeaderKey) {
        return "";
      }

      const contentType = headers[contentTypeHeaderKey]
        .toLowerCase()
        .split(";")[0];
      switch (contentType) {
        case "text/xml":
        case "application/xml":
        case "application/soap+xml":
          return bodyTypes.xml;
        case "application/json":
          return bodyTypes.json;
        case "application/x-www-form-urlencoded":
          return bodyTypes.form;
        default:
          return "";
      }
    });
    const renderedBody = computed(() => {
      if (bodyType.value === bodyTypes.xml) {
        return xmlFormatter(getBody());
      } else if (bodyType.value === bodyTypes.json) {
        try {
          const json = JSON.parse(getBody());
          return JSON.stringify(json, null, 2);
        } catch (e) {
          return "";
        }
      } else if (bodyType.value === bodyTypes.form) {
        return formFormat(getBody());
      }

      return "";
    });
    const language = computed(() => {
      switch (bodyType.value) {
        case bodyTypes.json:
          return "json";
        case bodyTypes.xml:
          return "xml";
        default:
          return "";
      }
    });
    const rawBody = computed(() => getBody());
    const showMoreButtonEnabled = computed(() => {
      const newlineCount = countNewlineCharacters(
        showRenderedBody.value ? renderedBody.value : rawBody.value
      );
      return newlineCount >= requestBodyLineLimit;
    });
    const showMore = computed(() => {
      return showMoreButtonEnabled.value && !showMoreClicked.value;
    });

    // Methods
    const viewRenderedBody = () => {
      showRenderedBody.value = true;
    };
    const viewRawBody = () => (showRenderedBody.value = false);
    const copy = () =>
      copyTextToClipboard(getBody()).then(() =>
        success(resources.requestBodyCopiedToClipboard)
      );
    const showMoreClick = () => {
      showMoreClicked.value = true;
    };

    // Lifecycle
    onMounted(() => {
      showRenderedBody.value = !!bodyType.value;
    });

    return {
      bodyType,
      renderedBody,
      showRenderedBody,
      viewRenderedBody,
      viewRawBody,
      rawBody,
      copy,
      language,
      showMoreButtonEnabled,
      showMore,
      showMoreClick,
    };
  },
});
</script>

<style scoped lang="scss">
@import "node_modules/bootstrap/scss/functions";
@import "node_modules/bootstrap/scss/variables";
@import "node_modules/bootstrap/scss/mixins";

.copy {
  font-size: 2em;
  cursor: pointer;
}

.show-more {
  height: 1200px;
  overflow: hidden;
  position: relative;
}

.show-more .show-more-button {
  width: 100%;
  height: 80px;
  position: absolute;
  bottom: 0;
  left: 0;
  cursor: pointer;
}
.show-more .show-more-button i {
  font-size: 3em;
  text-align: center;
}

.light-theme .show-more .show-more-button {
  background-image: linear-gradient(rgba(255, 0, 0, 0), #fff);
}
.dark-theme .show-more .show-more-button {
  background-image: linear-gradient(rgba(255, 0, 0, 0), $gray-900);
}
</style>
