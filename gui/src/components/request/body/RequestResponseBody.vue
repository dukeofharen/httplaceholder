<template>
  <TextRequestBody
    v-if="!request.requestParameters.bodyIsBinary"
    :request="request"
  />
  <BinaryRequestBody v-else :request="request" />
</template>

<script lang="ts">
import { defineComponent, PropType } from "vue";
import BinaryBody from "@/components/request/body/BinaryBody.vue";
import TextBody from "@/components/request/body/TextBody.vue";
import { RequestResultModel } from "@/domain/request/request-result-model";

export default defineComponent({
  name: "RequestResponseBody",
  components: { TextRequestBody: TextBody, BinaryRequestBody: BinaryBody },
  props: {
    request: {
      type: Object as PropType<RequestResultModel>,
      required: true,
    },
  },
});
</script>

<style scoped></style>
