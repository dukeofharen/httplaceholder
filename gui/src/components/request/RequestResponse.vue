<template>
  <accordion-item @opened="loadResponse">
    <template v-slot:button-text>Sent response</template>
    <template v-slot:accordion-body>
      <div v-if="hasResponse">
        <div class="col-md-12 mb-3" v-if="statusCode">
          <label>HTTP status code</label>
          <span>{{ statusCode }}</span>
        </div>
        <div class="col-md-12 mb-3" v-if="hasHeaders">
          <label>Response headers</label>
          <table class="table">
            <tbody>
              <tr v-for="(value, key) in headers" :key="key">
                <td class="p-1">{{ key }}</td>
                <td class="p-1">{{ value }}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="col-md-12 mb-3" v-if="body">
          <label>Response body</label>
          <button
            class="btn btn-success btn-sm me-2"
            @click="downloadResponse"
            title="Download the response"
          >
            Download
          </button>
          <code-highlight v-if="!bodyIsBinary" :code="body" />
        </div>
      </div>
      <div v-else>
        No response found for this request. Go to
        <router-link :to="{ name: 'Settings' }">Settings</router-link> to enable
        "Store response for request".
      </div>
    </template>
  </accordion-item>
</template>

<script lang="ts">
import { type PropType, defineComponent, ref, computed } from "vue";
import type { RequestResultModel } from "@/domain/request/request-result-model";
import type { ResponseModel } from "@/domain/request/response-model";
import { useRequestsStore } from "@/store/requests";
import type { HashMap } from "@/domain/hash-map";
import { base64ToBlob, fromBase64 } from "@/utils/text";
import { downloadBlob } from "@/utils/download";

export default defineComponent({
  name: "RequestResponse",
  props: {
    request: {
      type: Object as PropType<RequestResultModel>,
      required: true,
    },
  },
  setup(props) {
    const requestStore = useRequestsStore();

    // Data
    const response = ref<ResponseModel>();

    // Computed
    const headers = computed(() =>
      response.value ? response.value?.headers : ({} as HashMap)
    );
    const hasHeaders = computed(() => Object.keys(headers.value).length > 0);
    const bodyIsBinary = computed(() =>
      response.value ? response.value?.bodyIsBinary : false
    );
    const body = computed(() =>
      response.value ? fromBase64(response.value?.body) : ""
    );
    const statusCode = computed(() =>
      response.value ? response.value?.statusCode : null
    );
    const hasResponse = computed(() => props.request.hasResponse);

    // Methods
    const loadResponse = async () => {
      if (hasResponse.value && !response.value) {
        response.value = await requestStore.getResponse(
          props.request.correlationId
        );
      }
    };
    const downloadResponse = () => {
      if (response.value) {
        downloadBlob(
          props.request.correlationId,
          base64ToBlob(response.value.body)
        );
      }
    };

    return {
      loadResponse,
      response,
      headers,
      hasHeaders,
      bodyIsBinary,
      body,
      statusCode,
      downloadResponse,
      hasResponse,
    };
  },
});
</script>

<style lang="scss" scoped>
label {
  display: block;
  font-weight: bold;
}
</style>
