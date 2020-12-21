<template>
  <v-row>
    <v-col>
      <v-list-item v-if="!showList">
        <v-list-item-content>
          <v-list-item-title class="clickable bold" @click="showList = !showList">Click here to add request or response
            value
          </v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <template v-if="showList">
        <v-list-item v-for="(item, index) in formHelperItems" :key="index">
          <v-list-item-content>
            <v-list-item-title v-if="item.onClick" @click="item.onClick" class="clickable">{{
                item.title
              }}
            </v-list-item-title>
            <v-list-item-title :class="{bold: item.divider}" v-else>{{ item.title }}</v-list-item-title>
            <v-list-item-content v-if="item.subTitle" class="subtitle">{{ item.subTitle }}</v-list-item-content>
          </v-list-item-content>
        </v-list-item>
      </template>
      <HttpMethodSelector v-if="currentSelectedFormHelper === formHelperKeys.httpMethod" />
      <TenantSelector v-if="currentSelectedFormHelper === formHelperKeys.tenant" />
    </v-col>
  </v-row>
</template>

<script>
import {tooltipResources, formHelperKeys} from "@/shared/stubFormResources";
import HttpMethodSelector from "@/components/formHelpers/HttpMethodSelector";
import TenantSelector from "@/components/formHelpers/TenantSelector";


export default {
  components: {HttpMethodSelector, TenantSelector},
  mounted() {

  },
  data() {
    return {
      showList: false,
      formHelperKeys,
      formHelperItems: [
        {
          title: "General",
          divider: true
        },
        {
          title: "Description",
          subTitle: tooltipResources.description,
          onClick: () => this.setDefaultValue("stubForm/setDefaultDescription")
        },
        {
          title: "Priority",
          subTitle: tooltipResources.priority,
          onClick: () => this.setDefaultValue("stubForm/setDefaultPriority")
        },
        {
          title: "Tenant",
          subTitle: tooltipResources.tenant,
          onClick: () => this.openFormHelper(this.formHelperKeys.tenant)
        },
        {
          title: "Request",
          divider: true
        },
        {
          title: "HTTP method",
          subTitle: tooltipResources.httpMethod,
          onClick: () => this.openFormHelper(this.formHelperKeys.httpMethod)
        },
        {
          title: "URL path",
          subTitle: tooltipResources.urlPath,
          onClick: () => this.setDefaultValue("stubForm/setDefaultPath")
        },
        {
          title: "Full path",
          subTitle: tooltipResources.fullPath,
          onClick: () => this.setDefaultValue("stubForm/setDefaultFullPath")
        },
        {
          title: "Query string",
          subTitle: tooltipResources.queryString,
          onClick: () => this.setDefaultValue("stubForm/setDefaultQuery")
        },
        {
          title: "HTTPS",
          subTitle: tooltipResources.isHttps,
          onClick: () => this.setDefaultValue("stubForm/setDefaultIsHttps")
        },
        {
          title: "Basic authentication",
          subTitle: tooltipResources.basicAuthentication,
          onClick: () => this.setDefaultValue("stubForm/setDefaultBasicAuth")
        },
        {
          title: "Headers",
          subTitle: tooltipResources.headers,
          onClick: () => this.setDefaultValue("stubForm/setDefaultRequestHeaders")
        },
        {
          title: "Body",
          subTitle: tooltipResources.body,
          onClick: () => this.setDefaultValue("stubForm/setDefaultRequestBody")
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
.bold {
  font-weight: bold;
}

.clickable {
  cursor: pointer;
}

.subtitle {
  color: #909090;
}
</style>
