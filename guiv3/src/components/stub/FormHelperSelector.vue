<template>
  <div class="row">
    <div class="col-md-12">
      <accordion>
        <accordion-item
          v-for="(item, index) in formHelperItems"
          :key="index"
          :opened="item.opened"
          @buttonClicked="item.opened = !item.opened"
        >
          <template v-slot:button-text>{{ item.title }}</template>
          <template v-slot:accordion-body>
            <div class="list-group">
              <button
                v-for="(subItem, index) in item.subItems"
                :key="index"
                class="list-group-item list-group-item-action"
                @click="subItem.onClick"
              >
                <label>{{ subItem.title }}</label>
                <span class="subtitle text-secondary">{{
                  subItem.subTitle
                }}</span>
              </button>
            </div>
          </template>
        </accordion-item>
      </accordion>
    </div>
    <div v-if="currentSelectedFormHelper" class="col-md-12 mt-2">
      <HttpMethodSelector
        v-if="currentSelectedFormHelper === formHelperKeys.httpMethod"
      />
      <TenantSelector
        v-if="currentSelectedFormHelper === formHelperKeys.tenant"
      />
    </div>
  </div>
</template>

<script>
import { computed, ref } from "vue";
import {
  elementDescriptions,
  formHelperKeys,
} from "@/constants/stubFormResources";
import { useStore } from "vuex";
import HttpMethodSelector from "@/components/stub/HttpMethodSelector";
import TenantSelector from "@/components/stub/TenantSelector";

export default {
  name: "FormHelperSelector",
  components: { TenantSelector, HttpMethodSelector },
  setup() {
    const store = useStore();

    // Data
    const formHelperItems = ref([
      {
        title: "Add general information",
        opened: false,
        subItems: [
          {
            title: "Description",
            subTitle: elementDescriptions.description,
            onClick: () => setDefaultValue("stubForm/setDefaultDescription"),
          },
          {
            title: "Priority",
            subTitle: elementDescriptions.priority,
            onClick: () => setDefaultValue("stubForm/setDefaultPriority"),
          },
          {
            title: "Disable stub",
            subTitle: elementDescriptions.disable,
            onClick: () => setDefaultValue("stubForm/setStubDisabled"),
          },
          {
            title: "Tenant",
            subTitle: elementDescriptions.tenant,
            onClick: () => openFormHelper(formHelperKeys.tenant),
          },
        ],
      },
      {
        title: "Add request condition",
        opened: false,
        subItems: [
          {
            title: "HTTP method",
            subTitle: elementDescriptions.httpMethod,
            onClick: () => openFormHelper(formHelperKeys.httpMethod),
          },
          {
            title: "URL path",
            subTitle: elementDescriptions.urlPath,
            onClick: () => setDefaultValue("stubForm/setDefaultPath"),
          },
          {
            title: "Full path",
            subTitle: elementDescriptions.fullPath,
            onClick: () => setDefaultValue("stubForm/setDefaultFullPath"),
          },
          {
            title: "Query string",
            subTitle: elementDescriptions.queryString,
            onClick: () => setDefaultValue("stubForm/setDefaultQuery"),
          },
          {
            title: "HTTPS",
            subTitle: elementDescriptions.isHttps,
            onClick: () => setDefaultValue("stubForm/setDefaultIsHttps"),
          },
          {
            title: "Basic authentication",
            subTitle: elementDescriptions.basicAuthentication,
            onClick: () => setDefaultValue("stubForm/setDefaultBasicAuth"),
          },
          {
            title: "Headers",
            subTitle: elementDescriptions.headers,
            onClick: () => setDefaultValue("stubForm/setDefaultRequestHeaders"),
          },
          {
            title: "Body",
            subTitle: elementDescriptions.body,
            onClick: () => setDefaultValue("stubForm/setDefaultRequestBody"),
          },
          {
            title: "Form body",
            subTitle: elementDescriptions.formBody,
            onClick: () => setDefaultValue("stubForm/setDefaultFormBody"),
          },
          {
            title: "Client IP",
            subTitle: elementDescriptions.clientIp,
            onClick: () => setDefaultValue("stubForm/setDefaultClientIp"),
          },
          {
            title: "Hostname",
            subTitle: elementDescriptions.hostname,
            onClick: () => setDefaultValue("stubForm/setDefaultHostname"),
          },
          {
            title: "JSONPath",
            subTitle: elementDescriptions.jsonPath,
            onClick: () => setDefaultValue("stubForm/setDefaultJsonPath"),
          },
          {
            title: "JSON object",
            subTitle: elementDescriptions.jsonObject,
            onClick: () => setDefaultValue("stubForm/setDefaultJsonObject"),
          },
          {
            title: "JSON array",
            subTitle: elementDescriptions.jsonArray,
            onClick: () => setDefaultValue("stubForm/setDefaultJsonArray"),
          },
          {
            title: "XPath",
            subTitle: elementDescriptions.xpath,
            onClick: () => setDefaultValue("stubForm/setDefaultXPath"),
          },
        ],
      },
      {
        title: "Add response definition",
        opened: false,
        subItems: [
          {
            title: "HTTP status code",
            subTitle: elementDescriptions.statusCode,
            onClick: () => openFormHelper(formHelperKeys.statusCode),
          },
          {
            title: "Response body",
            subTitle: elementDescriptions.responseBody,
            onClick: () => openFormHelper(formHelperKeys.responseBody),
          },
          {
            title: "Response headers",
            subTitle: elementDescriptions.responseHeaders,
            onClick: () =>
              setDefaultValue("stubForm/setDefaultResponseHeaders"),
          },
          {
            title: "Content type",
            subTitle: elementDescriptions.responseContentType,
            onClick: () =>
              setDefaultValue("stubForm/setDefaultResponseContentType"),
          },
          {
            title: "Extra duration",
            subTitle: elementDescriptions.extraDuration,
            onClick: () => setDefaultValue("stubForm/setDefaultExtraDuration"),
          },
          {
            title: "Image",
            subTitle: elementDescriptions.image,
            onClick: () => setDefaultValue("stubForm/setDefaultImage"),
          },
          {
            title: "Redirect",
            subTitle: elementDescriptions.redirect,
            onClick: () => openFormHelper(formHelperKeys.redirect),
          },
          {
            title: "Line endings",
            subTitle: elementDescriptions.lineEndings,
            onClick: () => openFormHelper(formHelperKeys.lineEndings),
          },
          {
            title: "Reverse proxy",
            subTitle: elementDescriptions.reverseProxy,
            onClick: () => setDefaultValue("stubForm/setDefaultReverseProxy"),
          },
        ],
      },
    ]);

    // Functions
    const closeAccordions = () => {
      for (const item of formHelperItems.value) {
        item.opened = false;
      }
    };

    // Methods
    const setDefaultValue = (mutationName) => {
      closeAccordions();
      store.commit(mutationName);
    };
    const openFormHelper = (key) => {
      closeAccordions();
      store.commit("stubForm/openFormHelper", key);
    };

    // Computed
    const currentSelectedFormHelper = computed(
      () => store.getters["stubForm/getCurrentSelectedFormHelper"]
    );

    return {
      formHelperItems,
      setDefaultValue,
      openFormHelper,
      formHelperKeys,
      currentSelectedFormHelper,
    };
  },
};
</script>

<style scoped>
label {
  display: block;
  cursor: pointer;
}

.subtitle {
  font-size: 0.9em;
}
</style>
