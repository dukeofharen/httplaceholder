<template>
  <div class="col-12 row">
    <div v-if="renderedBodyTypeText" class="col-12 row">
      <a v-on:click="viewRawBody" v-bind:class="{ selected: !showRenderedBody }">Raw</a>&nbsp;|&nbsp;<a v-on:click="viewRenderedBody" v-bind:class="{ selected: showRenderedBody }">{{renderedBodyTypeText}}</a>
    </div>
    <div class="col-12 row">
      <span v-if="showRenderedBody">
        <pre><code>{{renderedBody}}</code></pre>
      </span>
      <span v-if="!showRenderedBody">
        <pre><code>{{rawBody}}</code></pre>
      </span>
    </div>
  </div>
</template>

<script>
const xmlType = "XML";

import xmlFormatter from "xml-formatter";

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
        contentType.includes("application/xml")
      ) {
        return xmlType;
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
      }

      return "";
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
</style>