<template>
  <v-row>
    <v-col>
      <div class="code-block">
        <!--        <v-tabs v-if="renderedBodyTypeText">-->
        <!--          <v-tab @click="viewRenderedBody">{{ renderedBodyTypeText }}</v-tab>-->
        <!--          <v-tab @click="viewRawBody">Raw</v-tab>-->
        <!--        </v-tabs>-->
        <v-row>
          <v-col cols="12" v-if="renderedBodyTypeText">
            <v-btn
              @click="viewRenderedBody"
              :color="showRenderedBody ? 'primary' : 'white'"
              class="mr-2"
              small
              outlined
              >{{ renderedBodyTypeText }}
            </v-btn>
            <v-btn
              @click="viewRawBody"
              :color="!showRenderedBody ? 'primary' : 'white'"
              small
              outlined
              >Raw
            </v-btn>
          </v-col>

          <v-col v-if="showRenderedBody" cols="12">
            <span class="body">
              <pre>{{ renderedBody }}</pre>
            </span>
          </v-col>

          <v-col v-if="!showRenderedBody" cols="12">
            <span class="body">{{ rawBody }}</span>
          </v-col>

          <v-col cols="12">
            <v-icon @click="copy" class="copy mt-3" title="Copy request body"
              >mdi-content-copy
            </v-icon>
          </v-col>
        </v-row>
      </div>
    </v-col>
  </v-row>
</template>

<script>
import { toastSuccess } from "@/utils/toastUtil";

const xmlType = "XML";
const jsonType = "JSON";
const formType = "Form";

import xmlFormatter from "xml-formatter";
import { formFormat } from "@/utils/formFormatter";
import { copyTextToClipboard } from "@/utils/clipboardUtil";
import { resources } from "@/shared/resources";

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
        return xmlFormatter(this.requestParameters.body, {
          lineSeparator: "\n",
          indentation: "  "
        });
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
      copyTextToClipboard(this.rawBody).then(() =>
        toastSuccess(resources.requestBodyCopiedToClipboard)
      );
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

.code-block {
  background-color: #353535;
  color: #ffffff;
  padding: 15px;
  border-radius: 8px;
}

.copy {
  font-size: 2.5em;
  color: #ffffff;
}
</style>
