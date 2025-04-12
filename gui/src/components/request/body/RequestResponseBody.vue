<template>
  <TextBody v-if="!renderModel.bodyIsBinary" :renderModel="renderModel" />
  <BinaryBody v-else :renderModel="renderModel" />
</template>

<script setup lang="ts">
import type { RequestResponseBodyRenderModel } from '@/domain/request/request-response-body-render-model'

export type RequestResponseBodyProps = {
  renderModel: RequestResponseBodyRenderModel
}

defineProps<RequestResponseBodyProps>()
</script>

<style scoped></style>
