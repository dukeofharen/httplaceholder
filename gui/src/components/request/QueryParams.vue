<template>
  <accordion-item>
    <template v-slot:button-text>{{ $translate('request.queryParameters') }}</template>
    <template v-slot:accordion-body>
      <table class="table" v-if="queryParameters">
        <tbody>
          <tr v-for="(value, key) in queryParameters" :key="key">
            <td class="p-1">{{ key }}</td>
            <td class="p-1">{{ value }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </accordion-item>
</template>

<script setup lang="ts">
import { parseUrl } from '@/utils/url'
import { computed, type PropType } from 'vue'
import type { RequestResultModel } from '@/domain/request/request-result-model'

const props = defineProps({
  request: {
    type: Object as PropType<RequestResultModel>,
    required: true,
  },
})

// Computed
const requestParams = computed(() => props.request?.requestParameters)
const queryParameters = computed(() => {
  const req = requestParams.value
  return req.url ? parseUrl(req.url) : {}
})
</script>

<style scoped></style>
