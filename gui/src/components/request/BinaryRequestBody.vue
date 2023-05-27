<template>
  <div class="card">
    <div class="card-body">
      <div class="row">
        <div class="col-md-12">
          <button class="btn btn-sm btn-outline-primary mb-2" @click="download">
            Download
          </button>
          <div v-if="bodyType === bodyTypes.image">
            <img :src="'data:' + contentType + ';base64,' + body" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, PropType } from "vue";
import { RequestResultModel } from "@/domain/request/request-result-model";
import { downloadBlob } from "@/utils/download";
import { base64ToBlob } from "@/utils/text";
import { imageMimeTypes, pdfMimeType } from "@/constants/technical";

const bodyTypes = {
  image: "image",
  pdf: "pdf",
  other: "other",
};

export default defineComponent({
  name: "BinaryRequestBody",
  props: {
    request: {
      type: Object as PropType<RequestResultModel>,
      required: true,
    },
  },
  setup(props) {
    // Computed
    const body = computed(() => {
      return props.request.requestParameters.body;
    });
    const contentType = computed(() => {
      const headers = props.request.requestParameters.headers;
      const contentTypeHeaderKey = Object.keys(headers).find(
        (k) => k.toLowerCase() === "content-type"
      );
      if (!contentTypeHeaderKey) {
        return "";
      }

      return headers[contentTypeHeaderKey].toLowerCase().split(";")[0];
    });
    const bodyType = computed(() => {
      const type = contentType.value;
      if (!type) {
        return bodyTypes.other;
      }

      if (imageMimeTypes.find((m) => type.includes(m))) {
        return bodyTypes.image;
      }

      if (type.includes(pdfMimeType)) {
        return bodyTypes.pdf;
      }

      return bodyTypes.other;
    });

    // Methods
    const download = () => {
      downloadBlob("file.bin", base64ToBlob(body.value));
    };

    return { download, bodyType, bodyTypes, contentType, body };
  },
});
</script>

<style scoped lang="scss">
img {
  max-width: 100%;
}
</style>
