<template>
  <accordion-item @opened="loadResponse">
    <template v-slot:button-text>{{
      $translate("request.sentResponse")
    }}</template>
    <template v-slot:accordion-body>
      <div v-if="hasResponse">
        <div class="col-md-12 mb-3" v-if="statusCode">
          <label>{{ $translate("request.httpStatusCode") }}</label>
          <span>{{ statusCode }}</span>
        </div>
        <div class="col-md-12 mb-3" v-if="hasHeaders">
          <label>{{ $translate("request.responseHeaders") }}</label>
          <table class="table">
            <tbody>
              <tr v-for="(value, key) in headers" :key="key">
                <td class="p-1">{{ key }}</td>
                <td class="p-1">{{ value }}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="col-md-12 mb-3" v-if="bodyRenderModel">
          <label>{{ $translate("request.responseBody") }}</label>
          <RequestResponseBody :render-model="bodyRenderModel" />
        </div>
      </div>
      <div v-else>
        <div
          v-html="
            $vsprintf($translateWithMarkdown('request.noResponseFound'), [
              settingsUrl,
            ])
          "
        />
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
import RequestResponseBody from "@/components/request/body/RequestResponseBody.vue";
import { type RequestResponseBodyRenderModel } from "@/domain/request/request-response-body-render-model";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "RequestResponse",
  components: { RequestResponseBody },
  props: {
    request: {
      type: Object as PropType<RequestResultModel>,
      required: true,
    },
  },
  setup(props) {
    const requestStore = useRequestsStore();
    const router = useRouter();

    // Data
    const response = ref<ResponseModel>({
      body: "",
      headers: {} as HashMap,
      statusCode: 0,
      bodyIsBinary: false,
    });

    // Computed
    const headers = computed(() =>
      response.value ? response.value?.headers : ({} as HashMap),
    );
    const hasHeaders = computed(() => Object.keys(headers.value).length > 0);
    const statusCode = computed(() =>
      response.value ? response.value?.statusCode : null,
    );
    const hasResponse = computed(() => props.request.hasResponse);
    const bodyRenderModel = computed<RequestResponseBodyRenderModel>(() => {
      return {
        body: response.value.body,
        base64DecodeNotBinary: true,
        bodyIsBinary: response.value.bodyIsBinary,
        headers: response.value.headers,
      };
    });
    const settingsUrl = computed(
      () => router.resolve({ name: "Settings" }).href,
    );

    // Methods
    const loadResponse = async () => {
      if (hasResponse.value && !response.value.statusCode) {
        response.value = await requestStore.getResponse(
          props.request.correlationId,
        );
      }
    };

    return {
      loadResponse,
      response,
      headers,
      hasHeaders,
      statusCode,
      hasResponse,
      bodyRenderModel,
      settingsUrl,
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
