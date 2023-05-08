<template>
  <accordion-item>
    <template v-slot:button-text>Query parameters</template>
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

<script lang="ts">
import { parseUrl } from "@/utils/url";
import { defineComponent, computed, type PropType } from "vue";
import type { RequestResultModel } from "@/domain/request/request-result-model";

export default defineComponent({
  name: "QueryParams",
  props: {
    request: {
      type: Object as PropType<RequestResultModel>,
      required: true,
    },
  },
  setup(props) {
    // Computed
    const requestParams = computed(() => props.request?.requestParameters);
    const queryParameters = computed(() => {
      const req = requestParams.value;
      return req.url ? parseUrl(req.url) : {};
    });

    return { requestParams, queryParameters };
  },
});
</script>

<style scoped></style>
