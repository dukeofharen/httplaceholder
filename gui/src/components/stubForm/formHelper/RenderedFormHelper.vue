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
  <SetPath v-if="currentSelectedFormHelper === FormHelperKey.Path" />
  <SetFullPath v-if="currentSelectedFormHelper === FormHelperKey.FullPath" />
  <SetQuery v-if="currentSelectedFormHelper === FormHelperKey.Query" />
  <SetBody v-if="currentSelectedFormHelper === FormHelperKey.Body" />
  <SetHeader v-if="currentSelectedFormHelper === FormHelperKey.Header" />
  <SetForm v-if="currentSelectedFormHelper === FormHelperKey.Form" />
  <SetHost v-if="currentSelectedFormHelper === FormHelperKey.Host" />
  <BasicInput
    v-if="currentSelectedFormHelper === FormHelperKey.Description"
    :value-getter="() => stubFormStore.getDescription"
    :value-setter="(v: string) => stubFormStore.setDescription(v)"
    title="Insert description"
  />
</template>

<script lang="ts">
import { computed, defineComponent } from "vue";
import { useStubFormStore } from "@/store/stubForm";
import { FormHelperKey } from "@/domain/stubForm/form-helper-key";
import { ResponseBodyType } from "@/domain/stubForm/response-body-type";
import HttpMethodSelector from "@/components/stubForm/formHelper/HttpMethodSelector.vue";
import SetPath from "@/components/stubForm/formHelper/SetPath.vue";
import HttpStatusCodeSelector from "@/components/stubForm/formHelper/HttpStatusCodeSelector.vue";
import RedirectSelector from "@/components/stubForm/formHelper/RedirectSelector.vue";
import SetHeader from "@/components/stubForm/formHelper/SetHeader.vue";
import BasicInput from "@/components/stubForm/formHelper/BasicInput.vue";
import SetBody from "@/components/stubForm/formHelper/SetBody.vue";
import SetQuery from "@/components/stubForm/formHelper/SetQuery.vue";
import SetForm from "@/components/stubForm/formHelper/SetForm.vue";
import ScenarioSelector from "@/components/stubForm/formHelper/ScenarioSelector.vue";
import LineEndingSelector from "@/components/stubForm/formHelper/LineEndingSelector.vue";
import TenantSelector from "@/components/stubForm/formHelper/TenantSelector.vue";
import ResponseBodyHelper from "@/components/stubForm/formHelper/ResponseBodyHelper.vue";
import SetFullPath from "@/components/stubForm/formHelper/SetFullPath.vue";
import SetDynamicMode from "@/components/stubForm/formHelper/SetDynamicMode.vue";
import ExampleSelector from "@/components/stubForm/formHelper/ExampleSelector.vue";
import SetHost from "@/components/stubForm/formHelper/SetHost.vue";

export default defineComponent({
  name: "RenderedFormHelper",
  components: {
    SetHost,
    ExampleSelector,
    SetDynamicMode,
    SetFullPath,
    ResponseBodyHelper,
    TenantSelector,
    LineEndingSelector,
    ScenarioSelector,
    SetForm,
    SetQuery,
    SetBody,
    BasicInput,
    SetHeader,
    RedirectSelector,
    HttpStatusCodeSelector,
    SetPath,
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
