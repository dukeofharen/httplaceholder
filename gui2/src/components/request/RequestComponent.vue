<script setup lang="ts">
import type { PropType } from 'vue'
import type { RequestOverviewModel } from '@/domain/request/request-overview-model.ts'
import RequestMethod from '@/components/request/RequestMethod.vue'

const props = defineProps({
  overviewRequest: {
    type: Object as PropType<RequestOverviewModel>,
    required: true,
  },
})
</script>

<template>
  <div class="border border-gray-500 p-2">
    <div>
      <RequestMethod :method="props.overviewRequest.method" />
    </div>
  </div>
</template>

<style scoped></style>
