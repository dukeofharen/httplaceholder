<template>
  <TextBody v-if="!renderModel.bodyIsBinary" :renderModel="renderModel" />
  <BinaryBody v-else :renderModel="renderModel" />
</template>

<script setup lang="ts">
import { type PropType } from 'vue'
import BinaryBody from '@/components/request/body/BinaryBody.vue'
import TextBody from '@/components/request/body/TextBody.vue'
import type { RequestResponseBodyRenderModel } from '@/domain/request/request-response-body-render-model'

const props = defineProps({
  renderModel: {
    type: Object as PropType<RequestResponseBodyRenderModel>,
    required: true,
  },
})
</script>

<style scoped></style>
