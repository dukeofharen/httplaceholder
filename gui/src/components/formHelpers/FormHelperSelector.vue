<template>
  <v-row>
    <v-col>
      <v-list-item v-if="!showList">
        <v-list-item-content class="helper-button" @click="doShowList">
          <v-list-item-title>Click here to add request or response
            value
          </v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <template v-if="showList">
        <v-list-group v-for="(item, index) in formHelperItems" :key="index">
          <template v-slot:activator>
            <v-list-item-title class="main-item">{{ item.title }}</v-list-item-title>
          </template>

          <v-list-item v-for="(subItem, index) in item.subItems" :key="index" class="sub-item" @click="subItem.onClick">
            <v-list-item-content>
              <v-list-item-title v-text="subItem.title"></v-list-item-title>
              <v-list-item-content class="subtitle">{{ subItem.subTitle }}</v-list-item-content>
            </v-list-item-content>
          </v-list-item>
        </v-list-group>
      </template>
      <HttpMethodSelector v-if="currentSelectedFormHelper === formHelperKeys.httpMethod"/>
      <TenantSelector v-if="currentSelectedFormHelper === formHelperKeys.tenant"/>
      <HttpStatusCodeSelector v-if="currentSelectedFormHelper === formHelperKeys.statusCode"/>
      <ResponseBodyHelper v-if="currentSelectedFormHelper === formHelperKeys.responseBody"/>
      <RedirectSelector v-if="currentSelectedFormHelper === formHelperKeys.redirect"/>
      <LineEndingSelector v-if="currentSelectedFormHelper === formHelperKeys.lineEndings"/>
    </v-col>
  </v-row>
</template>

<script>
import {elementDescriptions, formHelperKeys} from "@/shared/stubFormResources";
import HttpMethodSelector from "@/components/formHelpers/HttpMethodSelector";
import TenantSelector from "@/components/formHelpers/TenantSelector";
import HttpStatusCodeSelector from "@/components/formHelpers/HttpStatusCodeSelector";
import ResponseBodyHelper from "@/components/formHelpers/ResponseBodyHelper";
import RedirectSelector from "@/components/formHelpers/RedirectSelector";
import LineEndingSelector from "@/components/formHelpers/LineEndingSelector";


export default {
  components: {
    HttpMethodSelector,
    TenantSelector,
    HttpStatusCodeSelector,
    ResponseBodyHelper,
    RedirectSelector,
    LineEndingSelector
  },
  mounted() {

  },
  data() {
    return {
      showList: false,
      formHelperKeys,
      formHelperItems: [
        {
          title: "General information",
          subItems: [
            {
              title: "Description",
              subTitle: elementDescriptions.description,
              onClick: () => this.setDefaultValue("stubForm/setDefaultDescription")
            },
            {
              title: "Priority",
              subTitle: elementDescriptions.priority,
              onClick: () => this.setDefaultValue("stubForm/setDefaultPriority")
            },
            {
              title: "Tenant",
              subTitle: elementDescriptions.tenant,
              onClick: () => this.openFormHelper(this.formHelperKeys.tenant)
            }]
        },
        {
          title: "Request conditions",
          subItems: [
            {
              title: "HTTP method",
              subTitle: elementDescriptions.httpMethod,
              onClick: () => this.openFormHelper(this.formHelperKeys.httpMethod)
            },
            {
              title: "URL path",
              subTitle: elementDescriptions.urlPath,
              onClick: () => this.setDefaultValue("stubForm/setDefaultPath")
            },
            {
              title: "Full path",
              subTitle: elementDescriptions.fullPath,
              onClick: () => this.setDefaultValue("stubForm/setDefaultFullPath")
            },
            {
              title: "Query string",
              subTitle: elementDescriptions.queryString,
              onClick: () => this.setDefaultValue("stubForm/setDefaultQuery")
            },
            {
              title: "HTTPS",
              subTitle: elementDescriptions.isHttps,
              onClick: () => this.setDefaultValue("stubForm/setDefaultIsHttps")
            },
            {
              title: "Basic authentication",
              subTitle: elementDescriptions.basicAuthentication,
              onClick: () => this.setDefaultValue("stubForm/setDefaultBasicAuth")
            },
            {
              title: "Headers",
              subTitle: elementDescriptions.headers,
              onClick: () => this.setDefaultValue("stubForm/setDefaultRequestHeaders")
            },
            {
              title: "Body",
              subTitle: elementDescriptions.body,
              onClick: () => this.setDefaultValue("stubForm/setDefaultRequestBody")
            },
            {
              title: "Form body",
              subTitle: elementDescriptions.formBody,
              onClick: () => this.setDefaultValue("stubForm/setDefaultFormBody")
            },
            {
              title: "Client IP",
              subTitle: elementDescriptions.clientIp,
              onClick: () => this.setDefaultValue("stubForm/setDefaultClientIp")
            },
            {
              title: "Hostname",
              subTitle: elementDescriptions.hostname,
              onClick: () => this.setDefaultValue("stubForm/setDefaultHostname")
            },
            {
              title: "JSONPath",
              subTitle: elementDescriptions.jsonPath,
              onClick: () => this.setDefaultValue("stubForm/setDefaultJsonPath")
            },
            {
              title: "XPath",
              subTitle: elementDescriptions.xpath,
              onClick: () => this.setDefaultValue("stubForm/setDefaultXPath")
            }
          ]
        },
        {
          title: "Response definitions",
          subItems: [
            {
              title: "HTTP status code",
              subTitle: elementDescriptions.statusCode,
              onClick: () => this.openFormHelper(this.formHelperKeys.statusCode)
            },
            {
              title: "Response body",
              subTitle: elementDescriptions.responseBody,
              onClick: () => this.openFormHelper(this.formHelperKeys.responseBody)
            },
            {
              title: "Response headers",
              subTitle: elementDescriptions.responseHeaders,
              onClick: () => this.setDefaultValue("stubForm/setDefaultResponseHeaders")
            },
            {
              title: "Extra duration",
              subTitle: elementDescriptions.extraDuration,
              onClick: () => this.setDefaultValue("stubForm/setDefaultExtraDuration")
            },
            {
              title: "Redirect",
              subTitle: elementDescriptions.redirect,
              onClick: () => this.openFormHelper(this.formHelperKeys.redirect)
            },
            {
              title: "Line endings",
              subTitle: elementDescriptions.lineEndings,
              onClick: () => this.openFormHelper(this.formHelperKeys.lineEndings)
            }
          ]
        }
      ]
    };
  },
  computed: {
    currentSelectedFormHelper() {
      return this.$store.getters["stubForm/getCurrentSelectedFormHelper"];
    }
  },
  methods: {
    doShowList() {
      this.$store.commit("stubForm/closeFormHelper");
      this.showList = true;
    },
    setDefaultValue(mutationName) {
      this.$store.commit(mutationName);
      this.showList = false;
    },
    openFormHelper(key) {
      this.$store.commit("stubForm/openFormHelper", key);
      this.showList = false;
    }
  }
};
</script>

<style scoped>
.main-item {
  font-weight: bold;
  cursor: pointer;
}

.sub-item {
  cursor: pointer;
}

.sub-item:hover {
  background-color: #f1f1f1;
}

.helper-button {
  font-weight: bold;
  cursor: pointer;
  padding-left: 20px;
}

.helper-button:hover {
  background-color: #f1f1f1;
}

.subtitle {
  color: #909090;
}
</style>
