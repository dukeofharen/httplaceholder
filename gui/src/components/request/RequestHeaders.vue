<template>
  <accordion-item>
    <template v-slot:button-text>{{ $translate('request.requestHeaders') }}</template>
    <template v-slot:accordion-body>
      <table class="table" v-if="requestParams.headers">
        <tbody>
          <tr v-for="(value, key) in requestParams.headers" :key="key">
            <td class="p-1">{{ key }}</td>
            <td class="p-1">{{ value }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </accordion-item>
</template>

<script lang="ts">
import { computed } from 'vue'
import { defineComponent, type PropType } from 'vue'
import type { RequestResultModel } from '@/domain/request/request-result-model'

export default defineComponent({
  name: 'RequestHeaders',
  props: {
    request: {
      type: Object as PropType<RequestResultModel>,
      required: true,
    },
  },
  setup(props) {
    // Computed
    const requestParams = computed(() => props.request?.requestParameters)

    return { requestParams }
  },
})
</script>

<style scoped></style>
