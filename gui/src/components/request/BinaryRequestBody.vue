<template>
  <div class="card">
    <div class="card-body">
      <div class="row">
        <div class="col-md-12">
          <button class="btn btn-sm btn-outline-primary" @click="download">
            Download
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, PropType } from "vue";
import { RequestResultModel } from "@/domain/request/request-result-model";
import { downloadBlob } from "@/utils/download";
import { base64ToBlob } from "@/utils/text";

export default defineComponent({
  name: "BinaryRequestBody",
  props: {
    request: {
      type: Object as PropType<RequestResultModel>,
      required: true,
    },
  },
  setup(props) {
    // Methods
    const download = () => {
      downloadBlob(
        "file.bin",
        base64ToBlob(props.request.requestParameters.body)
      );
    };

    return { download };
  },
});
</script>

<style scoped></style>
