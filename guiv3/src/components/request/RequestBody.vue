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
      <div class="row mt-3">
        <div class="col-md-12" v-if="showRenderedBody">
          <pre>{{ renderedBody }}</pre>
        </div>
        <div class="col-md-12" v-if="!showRenderedBody">
          {{ rawBody }}
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

<script>
import { computed, onMounted, ref } from "vue";
import xmlFormatter from "xml-formatter";
import toastr from "toastr";
import { formFormat } from "@/utils/form";
import { copyTextToClipboard } from "@/utils/clipboard";
import { resources } from "@/constants/resources";

const bodyTypes = {
  xml: "XML",
  json: "JSON",
  form: "Form",
};

export default {
  name: "RequestBody",
  props: {
    request: {
      type: Object,
      required: true,
    },
  },
  setup(props) {
    // Data
    const showRenderedBody = ref(false);

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
    const rawBody = computed(() => getBody());

    // Methods
    const viewRenderedBody = () => (showRenderedBody.value = true);
    const viewRawBody = () => (showRenderedBody.value = false);
    const copy = () =>
      copyTextToClipboard(getBody()).then(() =>
        toastr.success(resources.requestBodyCopiedToClipboard)
      );

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
    };
  },
};
</script>

<style scoped>
.copy {
  font-size: 2em;
  cursor: pointer;
}
</style>
