<template>
  <TextRequestBody
    v-if="!request.requestParameters.bodyIsBinary"
    :request="request"
  />
  <BinaryRequestBody v-else :request="request" />
</template>

<script lang="ts">
import { defineComponent, PropType } from "vue";
import BinaryRequestBody from "@/components/request/body/BinaryRequestBody.vue";
import TextRequestBody from "@/components/request/body/TextRequestBody.vue";
import { RequestResultModel } from "@/domain/request/request-result-model";

export default defineComponent({
  components: { TextRequestBody, BinaryRequestBody },
  props: {
    request: {
      type: Object as PropType<RequestResultModel>,
      required: true,
    },
  },
});
</script>

<style scoped></style>
