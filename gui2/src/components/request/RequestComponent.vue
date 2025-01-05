<script setup lang="ts">
import { type PropType, ref } from 'vue'
import type { RequestOverviewModel } from '@/domain/request/request-overview-model.ts'
import RequestHeading from '@/components/request/RequestHeading.vue'
import RequestDetails from '@/components/request/RequestDetails.vue'

const props = defineProps({
  overviewRequest: {
    type: Object as PropType<RequestOverviewModel>,
    required: true,
  },
})

// Data
const showDetails = ref(false)

// Functions
function onRequestHeadingClick() {
  showDetails.value = !showDetails.value
}
</script>

<template>
  <div class="border font-mono border-gray-500 p-2">
    <RequestHeading
      :overview-request="props.overviewRequest"
      class="cursor-pointer"
      @click="onRequestHeadingClick"
    />
    <RequestDetails v-if="showDetails" :overview-request="props.overviewRequest" />
  </div>
</template>

<style scoped></style>
