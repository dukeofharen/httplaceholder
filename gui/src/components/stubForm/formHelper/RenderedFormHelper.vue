<template>
  <ExampleSelector v-if="currentSelectedFormHelper === FormHelperKey.Example" />
  <HttpMethodSelector v-if="currentSelectedFormHelper === FormHelperKey.HttpMethod" />
  <TenantSelector v-if="currentSelectedFormHelper === FormHelperKey.Tenant" />
  <HttpStatusCodeSelector v-if="currentSelectedFormHelper === FormHelperKey.StatusCode" />
  <ResponseBodyHelper v-if="currentSelectedFormHelper === FormHelperKey.ResponseBody" />
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
  <RedirectSelector v-if="currentSelectedFormHelper === FormHelperKey.Redirect" />
  <LineEndingSelector v-if="currentSelectedFormHelper === FormHelperKey.LineEndings" />
  <ScenarioSelector v-if="currentSelectedFormHelper === FormHelperKey.Scenario" />
  <SetDynamicMode v-if="currentSelectedFormHelper === FormHelperKey.DynamicMode" />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.Description"
    :value-getter="() => stubFormStore.getDescription"
    :value-setter="(v: string) => stubFormStore.setDescription(v)"
    :title="$translate('stubForm.descriptionTitle')"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.Priority"
    :value-getter="() => stubFormStore.getPriority"
    :value-setter="(v: string) => stubFormStore.setPriority(v)"
    input-type="numeric"
    :title="$translate('stubForm.priorityTitle')"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.Path"
    :value-getter="() => stubFormStore.getUrlPath"
    :value-setter="(input: any) => stubFormStore.setUrlPath(input)"
    :title="$translate('stubForm.urlPathTitle')"
    :multiple="true"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.FullPath"
    :value-getter="() => stubFormStore.getFullPath"
    :value-setter="(input: any) => stubFormStore.setFullPath(input)"
    :title="$translate('stubForm.fullPathTitle')"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.Query"
    :value-getter="() => stubFormStore.getQuery"
    :value-setter="(input: any) => stubFormStore.setQuery(input)"
    :title="$translate('stubForm.queryStringTitle')"
    :has-multiple-keys="true"
    :key-placeholder="$translate('stubForm.queryStringKeyPlaceholder')"
    :multiple="true"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.Header"
    :value-getter="() => stubFormStore.getRequestHeaders"
    :value-setter="(input: any) => stubFormStore.setRequestHeaders(input)"
    :title="$translate('stubForm.requestHeadersTitle')"
    :has-multiple-keys="true"
    :key-placeholder="$translate('stubForm.requestHeadersKeyPlaceholder')"
    :multiple="true"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.ClientIp"
    :value-getter="() => stubFormStore.getClientIp"
    :value-setter="(v: string) => stubFormStore.setClientIp(v)"
    :title="$translate('stubForm.clientIpTitle')"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.Host"
    :value-getter="() => stubFormStore.getHostname"
    :value-setter="(v: string) => stubFormStore.setHostname(v)"
    :title="$translate('stubForm.hostnameTitle')"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.Body"
    :value-getter="() => stubFormStore.getRequestBody"
    :value-setter="(input: any) => stubFormStore.setRequestBody(input)"
    :title="$translate('stubForm.requestBodyTitle')"
    :multiple="true"
  />
  <StringCheckerInput
    v-if="currentSelectedFormHelper === FormHelperKey.Form"
    :value-getter="() => stubFormStore.getForm"
    :value-setter="(input: any) => stubFormStore.setForm(input)"
    :title="$translate('stubForm.formBodyTitle')"
    :has-multiple-keys="true"
    :key-placeholder="$translate('stubForm.formBodyKeyPlaceholder')"
    :multiple="true"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.ScenarioMinHits"
    :value-getter="() => stubFormStore.getScenarioMinHits"
    :value-setter="(v: string) => stubFormStore.setScenarioMinHits(v)"
    input-type="numeric"
    :title="$translate('stubForm.minHitsTitle')"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.ScenarioMaxHits"
    :value-getter="() => stubFormStore.getScenarioMaxHits"
    :value-setter="(v: string) => stubFormStore.setScenarioMaxHits(v)"
    input-type="numeric"
    :title="$translate('stubForm.maxHitsTitle')"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.ScenarioExactHits"
    :value-getter="() => stubFormStore.getScenarioExactHits"
    :value-setter="(v: string) => stubFormStore.setScenarioExactHits(v)"
    input-type="numeric"
    :title="$translate('stubForm.exactHitsTitle')"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.ScenarioState"
    :value-getter="() => stubFormStore.getScenarioStateCheck"
    :value-setter="(v: string) => stubFormStore.setScenarioStateCheck(v)"
    :title="$translate('stubForm.scenarioStateTitle')"
  />
  <BasicAuthHelper v-if="currentSelectedFormHelper === FormHelperKey.BasicAuthentication" />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.ResponseContentType"
    :value-getter="() => stubFormStore.getResponseContentType"
    :value-setter="(v: string) => stubFormStore.setResponseContentType(v)"
    :title="$translate('stubForm.contentTypeTitle')"
  />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.ExtraDuration"
    :value-getter="() => stubFormStore.getExtraDuration"
    :value-setter="(v: number) => stubFormStore.setExtraDuration(v)"
    input-type="numeric"
    :title="$translate('stubForm.extraDurationTitle')"
  />
</template>

<script lang="ts">
import { computed, defineComponent } from 'vue'
import { useStubFormStore } from '@/store/stubForm'
import { FormHelperKey } from '@/domain/stubForm/form-helper-key'
import { ResponseBodyType } from '@/domain/stubForm/response-body-type'
import HttpMethodSelector from '@/components/stubForm/formHelper/HttpMethodSelector.vue'
import HttpStatusCodeSelector from '@/components/stubForm/formHelper/HttpStatusCodeSelector.vue'
import RedirectSelector from '@/components/stubForm/formHelper/RedirectSelector.vue'
import BasicInput from '@/components/stubForm/formHelper/BasicInput.vue'
import ScenarioSelector from '@/components/stubForm/formHelper/ScenarioSelector.vue'
import LineEndingSelector from '@/components/stubForm/formHelper/LineEndingSelector.vue'
import TenantSelector from '@/components/stubForm/formHelper/TenantSelector.vue'
import ResponseBodyHelper from '@/components/stubForm/formHelper/ResponseBodyHelper.vue'
import SetDynamicMode from '@/components/stubForm/formHelper/SetDynamicMode.vue'
import ExampleSelector from '@/components/stubForm/formHelper/ExampleSelector.vue'
import StringCheckerInput from '@/components/stubForm/formHelper/StringCheckerInput.vue'
import BasicAuthHelper from '@/components/stubForm/formHelper/BasicAuthHelper.vue'

export default defineComponent({
  name: 'RenderedFormHelper',
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
    BasicAuthHelper,
  },
  setup() {
    const stubFormStore = useStubFormStore()

    // Computed
    const currentSelectedFormHelper = computed(() => stubFormStore.getCurrentSelectedFormHelper)
    return {
      currentSelectedFormHelper,
      FormHelperKey,
      stubFormStore,
      ResponseBodyType,
    }
  },
})
</script>

<style scoped type="scss"></style>
