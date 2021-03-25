<template>
  <v-row>
    <v-col>
      <v-tabs v-if="renderedBodyTypeText">
        <v-tab @click="viewRenderedBody">{{ renderedBodyTypeText }}</v-tab>
        <v-tab @click="viewRawBody">Raw</v-tab>
      </v-tabs>
      <span v-if="showRenderedBody" class="body">
        <pre>{{ renderedBody }}</pre>
      </span>
      <span v-if="!showRenderedBody" class="body">{{ rawBody }}</span>
    </v-col>
  </v-row>
</template>

<script>
const xmlType = "XML";
const jsonType = "JSON";
const formType = "Form";

import xmlFormatter from "xml-formatter";
import { formFormat } from "@/utils/formFormatter";

export default {
  name: "requestBody",
  props: ["requestParameters"],
  data() {
    return {
      rawBody: "",
      renderedBody: "",
      showRenderedBody: false,
      renderedBodyTypeText: ""
    };
  },
  created() {
    this.rawBody = this.requestParameters.body;
    this.renderedBodyTypeText = this.getBodyType();
    this.renderedBody = this.renderBody();
    if (this.renderedBodyTypeText && this.renderedBody) {
      this.showRenderedBody = true;
    }
  },
  methods: {
    getBodyType() {
      let contentType = this.requestParameters.headers["Content-Type"];
      if (!contentType) {
        return "";
      }

      contentType = contentType.toLowerCase();
      if (
        contentType.includes("text/xml") ||
        contentType.includes("application/xml") ||
        contentType.includes("application/soap+xml")
      ) {
        return xmlType;
      } else if (contentType.includes("application/json")) {
        return jsonType;
      } else if (contentType.includes("application/x-www-form-urlencoded")) {
        return formType;
      }

      return "";
    },
    viewRawBody() {
      this.showRenderedBody = false;
    },
    viewRenderedBody() {
      this.showRenderedBody = true;
    },
    renderBody() {
      if (this.renderedBodyTypeText === xmlType) {
        return xmlFormatter(this.requestParameters.body);
      } else if (this.renderedBodyTypeText === jsonType) {
        try {
          let json = JSON.parse(this.requestParameters.body);
          return JSON.stringify(json, null, 2);
        } catch (err) {
          return "";
        }
      } else if (this.renderedBodyTypeText === formType) {
        return formFormat(this.requestParameters.body);
      }

      return "";
    },
    copy() {

    }
  }
};
</script>

<style scoped>
a {
  cursor: pointer;
}
a.selected {
  font-weight: bold;
}
.body {
  color: rgba(0, 0, 0, 0.6);
}
</style>
