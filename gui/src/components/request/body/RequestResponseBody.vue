<template>
  <TextRequestBody
    v-if="!renderModel.bodyIsBinary"
    :renderModel="renderModel"
  />
  <BinaryRequestBody v-else :renderModel="renderModel" />
</template>

<script lang="ts">
import { defineComponent, PropType } from "vue";
import BinaryBody from "@/components/request/body/BinaryBody.vue";
import TextBody from "@/components/request/body/TextBody.vue";
import type { RequestResponseBodyRenderModel } from "@/domain/request/request-response-body-render-model";

export default defineComponent({
  name: "RequestResponseBody",
  components: { TextRequestBody: TextBody, BinaryRequestBody: BinaryBody },
  props: {
    renderModel: {
      type: Object as PropType<RequestResponseBodyRenderModel>,
      required: true,
    },
  },
});
</script>

<style scoped></style>
