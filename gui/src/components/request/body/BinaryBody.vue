<template>
  <div class="card">
    <div class="card-body">
      <div class="row">
        <div class="col-md-12">
          <button class="btn btn-sm btn-primary" @click="download">
            Download
          </button>
          <div v-if="bodyType != bodyTypes.other" class="mt-2">
            <div v-if="bodyType === bodyTypes.image">
              <img :src="imageUrl" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, type PropType } from "vue";
import { type RequestResultModel } from "@/domain/request/request-result-model";
import { downloadBlob } from "@/utils/download";
import { base64ToBlob } from "@/utils/text";
import { imageMimeTypes } from "@/constants/technical";
import mime from "mime-types";

const bodyTypes = {
  image: "image",
  other: "other",
};

export default defineComponent({
  name: "BinaryBody",
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

      return bodyTypes.other;
    });
    const imageUrl = computed(() => {
      return `data:${contentType.value};base64,${body.value}`;
    });

    // Methods
    const download = () => {
      const extension = mime.extension(contentType.value) ?? "bin";
      downloadBlob(`file.${extension}`, base64ToBlob(body.value));
    };

    return {
      download,
      bodyType,
      bodyTypes,
      contentType,
      body,
      imageUrl,
    };
  },
});
</script>

<style scoped lang="scss">
img {
  max-width: 100%;
}
</style>
