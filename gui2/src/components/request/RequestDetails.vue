<script setup lang="ts">
import type { PropType } from 'vue'
import type { RequestOverviewModel } from '@/domain/request/request-overview-model.ts'

const props = defineProps({
  overviewRequest: {
    type: Object as PropType<RequestOverviewModel>,
    required: true,
  },
})
</script>

<template>
  {{ props.overviewRequest }}
</template>

<style scoped></style>
