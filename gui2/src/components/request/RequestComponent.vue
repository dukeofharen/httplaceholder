<script setup lang="ts">
import type { PropType } from 'vue'
import type { RequestOverviewModel } from '@/domain/request/request-overview-model.ts'
import RequestHeading from '@/components/request/RequestHeading.vue'

const props = defineProps({
  overviewRequest: {
    type: Object as PropType<RequestOverviewModel>,
    required: true,
  },
})
</script>

<template>
  <div class="border font-mono border-gray-500 p-2">
    <RequestHeading :overview-request="props.overviewRequest" />
  </div>
</template>

<style scoped></style>
