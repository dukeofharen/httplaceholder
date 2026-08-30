<template>
  <accordion-item>
    <template v-slot:button-text>{{ $translate('request.requestHeaders') }}</template>
    <template v-slot:accordion-body>
      <table class="table" v-if="request.requestParameters.headers">
        <tbody>
          <tr v-for="(value, key) in request.requestParameters.headers" :key="key">
            <td class="p-1">{{ key }}</td>
            <td class="p-1">{{ value }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </accordion-item>
</template>

<script setup lang="ts">
import type { RequestResultModel } from '@/domain/request/request-result-model'

export type RequestHeadersProps = {
  request: RequestResultModel
}
defineProps<RequestHeadersProps>()
</script>

<style scoped></style>
