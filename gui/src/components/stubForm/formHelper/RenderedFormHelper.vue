<template>
  <ExampleSelector v-if="currentSelectedFormHelper === FormHelperKey.Example" />
  <HttpMethodSelector
    v-if="currentSelectedFormHelper === FormHelperKey.HttpMethod"
  />
  <TenantSelector v-if="currentSelectedFormHelper === FormHelperKey.Tenant" />
  <HttpStatusCodeSelector
    v-if="currentSelectedFormHelper === FormHelperKey.StatusCode"
  />
  <ResponseBodyHelper
    v-if="currentSelectedFormHelper === FormHelperKey.ResponseBody"
  />
  <ResponseBodyHelper
    v-if="currentSelectedFormHelper === FormHelperKey.ResponseBodyPlainText"
    :preset-response-body-type="ResponseBodyType.text"
  />
  <ResponseBodyHelper
    v-if="currentSelectedFormHelper === FormHelperKey.ResponseBodyJson"
    :preset-response-body-type="ResponseBodyType.json"
  />
  <ResponseBodyHelper
    v-if="currentSelectedFormHelper === FormHelperKey.ResponseBodyXml"
    :preset-response-body-type="ResponseBodyType.xml"
  />
  <ResponseBodyHelper
    v-if="currentSelectedFormHelper === FormHelperKey.ResponseBodyHtml"
    :preset-response-body-type="ResponseBodyType.html"
  />
  <ResponseBodyHelper
    v-if="currentSelectedFormHelper === FormHelperKey.ResponseBodyBase64"
    :preset-response-body-type="ResponseBodyType.base64"
  />
  <RedirectSelector
    v-if="currentSelectedFormHelper === FormHelperKey.Redirect"
  />
  <LineEndingSelector
    v-if="currentSelectedFormHelper === FormHelperKey.LineEndings"
  />
  <ScenarioSelector
    v-if="currentSelectedFormHelper === FormHelperKey.Scenario"
  />
  <SetDynamicMode
    v-if="currentSelectedFormHelper === FormHelperKey.DynamicMode"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.Description"
    :value-getter="() => stubFormStore.getDescription"
    :value-setter="(v: string) => stubFormStore.setDescription(v)"
    title="Description"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.Priority"
    :value-getter="() => stubFormStore.getPriority"
    :value-setter="(v: string) => stubFormStore.setPriority(v)"
    input-type="numeric"
    title="Set a stub priority (the higher the number, the higher the stub priority)"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.Path"
    :value-getter="() => stubFormStore.getUrlPath"
    :value-setter="(input: any) => stubFormStore.setUrlPath(input)"
    title="URL path"
    :multiple="true"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.FullPath"
    :value-getter="() => stubFormStore.getFullPath"
    :value-setter="(input: any) => stubFormStore.setForm(input)"
    title="Full path (including query string)"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.Query"
    :value-getter="() => stubFormStore.getQuery"
    :value-setter="(input: any) => stubFormStore.setQuery(input)"
    title="Query string"
    :has-multiple-keys="true"
    key-placeholder="Query string key"
    :multiple="true"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.Header"
    :value-getter="() => stubFormStore.getRequestHeaders"
    :value-setter="(input: any) => stubFormStore.setRequestHeaders(input)"
    title="Request headers"
    :has-multiple-keys="true"
    key-placeholder="Request header name"
    :multiple="true"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.ClientIp"
    :value-getter="() => stubFormStore.getClientIp"
    :value-setter="(v: string) => stubFormStore.setClientIp(v)"
    title="Client IP (e.g. '127.0.0.1' or '127.0.0.0/30' to provide an IP range)"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.Host"
    :value-getter="() => stubFormStore.getHostname"
    :value-setter="(v: string) => stubFormStore.setHostname(v)"
    title="Hostname (e.g. 'httplaceholder.com')"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.Body"
    :value-getter="() => stubFormStore.getRequestBody"
    :value-setter="(input: any) => stubFormStore.setRequestBody(input)"
    title="Request body"
    :multiple="true"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.Form"
    :value-getter="() => stubFormStore.getForm"
    :value-setter="(input: any) => stubFormStore.setForm(input)"
    title="Form body"
    :has-multiple-keys="true"
    key-placeholder="Posted form value key"
    :multiple="true"
  />
</template>

<script lang="ts">
import { computed, defineComponent } from "vue";
import { useStubFormStore } from "@/store/stubForm";
import { FormHelperKey } from "@/domain/stubForm/form-helper-key";
import { ResponseBodyType } from "@/domain/stubForm/response-body-type";
import HttpMethodSelector from "@/components/stubForm/formHelper/HttpMethodSelector.vue";
import HttpStatusCodeSelector from "@/components/stubForm/formHelper/HttpStatusCodeSelector.vue";
import RedirectSelector from "@/components/stubForm/formHelper/RedirectSelector.vue";
import BasicInput from "@/components/stubForm/formHelper/BasicInput.vue";
import ScenarioSelector from "@/components/stubForm/formHelper/ScenarioSelector.vue";
import LineEndingSelector from "@/components/stubForm/formHelper/LineEndingSelector.vue";
import TenantSelector from "@/components/stubForm/formHelper/TenantSelector.vue";
import ResponseBodyHelper from "@/components/stubForm/formHelper/ResponseBodyHelper.vue";
import SetDynamicMode from "@/components/stubForm/formHelper/SetDynamicMode.vue";
import ExampleSelector from "@/components/stubForm/formHelper/ExampleSelector.vue";
import { stubFormHelpers } from "@/domain/stubForm/stub-form-helpers";
import StringCheckerInput from "@/components/stubForm/formHelper/StringCheckerInput.vue";

export default defineComponent({
  name: "RenderedFormHelper",
  methods: {
    stubFormHelpers() {
      return stubFormHelpers;
    },
  },
  components: {
    StringCheckerInput,
    ExampleSelector,
    SetDynamicMode,
    ResponseBodyHelper,
    TenantSelector,
    LineEndingSelector,
    ScenarioSelector,
    BasicInput,
    RedirectSelector,
    HttpStatusCodeSelector,
    HttpMethodSelector,
  },
  setup() {
    const stubFormStore = useStubFormStore();

    // Computed
    const currentSelectedFormHelper = computed(
      () => stubFormStore.getCurrentSelectedFormHelper,
    );
    return {
      currentSelectedFormHelper,
      FormHelperKey,
      stubFormStore,
      ResponseBodyType,
    };
  },
});
</script>

<style scoped type="scss"></style>
