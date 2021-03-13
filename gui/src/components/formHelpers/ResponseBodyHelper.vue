<template>
  <div class="form">
    <v-row>
      <v-col cols="12">
        <p>
          Select a type of response and fill in the actual response that should
          be returned and press "Insert".
        </p>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-select
          :items="responseBodyTypeSelectList"
          v-model="responseBodyType"
          label="Select a response type..."
        />
      </v-col>
    </v-row>
    <v-row v-if="responseBodyType === responseBodyTypes.base64">
      <v-col cols="12">
        <input type="file" name="file" ref="base64Upload" @change="upload" />
        <p>
          You can upload a file for use in the Base64 response.
        </p>
        <v-btn color="primary" @click="uploadClick">Upload a file</v-btn>
      </v-col>
    </v-row>
    <v-row v-if="responseBodyType === responseBodyTypes.base64">
      <v-col cols="12">
        <v-btn color="primary" @click="showBase64TextInput = true"
          >Show text input
        </v-btn>
      </v-col>
    </v-row>
    <v-row v-if="showDynamicModeRow">
      <v-col cols="12">
        <p>
          {{ elementDescriptions.dynamicMode }}
        </p>
        <v-checkbox
          v-model="enableDynamicMode"
          label="Enable dynamic mode"
          @change="changeDynamicMode"
        />
      </v-col>
      <v-col cols="12" v-if="showVariableParsers">
        <v-select
          :items="variableParserSelectList"
          v-model="selectedVariableHandler"
          item-text="name"
          item-value="key"
          label="Select a variable handler to insert in the response..."
          @input="insertVariableHandler"
        />
      </v-col>
    </v-row>
    <v-row
      v-if="
        responseBodyType !== responseBodyTypes.base64 || showBase64TextInput
      "
    >
      <v-col cols="12">
        <v-textarea
          label="Fill in the response..."
          v-model="responseBody"
          id="responseBody"
        ></v-textarea>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-btn color="success mr-2" @click="insert">Insert</v-btn>
        <v-btn color="error" @click="close">Close</v-btn>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import {
  responseBodyTypes,
  elementDescriptions
} from "@/shared/stubFormResources";

export default {
  async mounted() {
    this.responseBodyType = this.$store.getters["stubForm/getResponseBodyType"];
    let responseBody = this.$store.getters["stubForm/getResponseBody"];
    if (this.responseBodyType === responseBodyTypes.base64) {
      responseBody = atob(responseBody);
    }

    this.responseBody = responseBody;

    this.metadata = await this.$store.dispatch("metadata/getMetadata");
    this.enableDynamicMode = this.$store.getters["stubForm/getDynamicMode"];
  },
  data() {
    return {
      responseBodyTypes,
      elementDescriptions,
      responseBodyType: "",
      responseBody: "",
      showBase64TextInput: false,
      metadata: null,
      enableDynamicMode: false,
      selectedVariableHandler: "default"
    };
  },
  computed: {
    responseBodyTypeSelectList() {
      return Object.keys(responseBodyTypes).map(k => responseBodyTypes[k]);
    },
    variableParserSelectList() {
      if (!this.metadata || !this.metadata.variableHandlers) {
        return [];
      }

      const result = this.metadata.variableHandlers.map(h => ({
        key: h.name,
        name: h.fullName
      }));
      result.unshift({ key: "default", name: "" });
      return result;
    },
    showDynamicModeRow() {
      return this.responseBodyType !== responseBodyTypes.base64;
    },
    showVariableParsers() {
      return this.showDynamicModeRow && this.enableDynamicMode;
    }
  },
  methods: {
    insert() {
      let responseBody = this.responseBody;
      if (this.responseBodyType === responseBodyTypes.base64) {
        responseBody = btoa(responseBody);
      }
      this.$store.commit("stubForm/setResponseBody", {
        type: this.responseBodyType,
        body: responseBody
      });
      this.showBase64TextInput = false;
    },
    close() {
      this.$store.commit("stubForm/closeFormHelper");
    },
    uploadClick() {
      this.$refs.base64Upload.click();
    },
    upload(ev) {
      const files = Array.from(ev.target.files);
      const file = files[0];
      const reader = new FileReader();
      const regex = /^data:(.+);base64,(.*)$/;
      reader.onload = e => {
        const matches = e.target.result.match(regex);
        const contentType = matches[1];
        const body = matches[2];
        this.$store.commit("stubForm/setResponseContentType", contentType);
        this.$store.commit("stubForm/setResponseBody", {
          type: responseBodyTypes.base64,
          body
        });
        this.responseBody = body;
        this.$store.commit("stubForm/closeFormHelper");
        this.showBase64TextInput = false;
      };
      reader.readAsDataURL(file);
    },
    changeDynamicMode() {
      this.$store.commit("stubForm/setDynamicMode", this.enableDynamicMode);
    },
    insertVariableHandler(value) {
      setTimeout(() => (this.selectedVariableHandler = "default"), 10);
      const handler = this.metadata.variableHandlers.find(
        h => h.name === value
      );
      const textarea = document.getElementById("responseBody");
      const cursorPosition = textarea.selectionStart;
      const body = this.responseBody;
      this.responseBody = [
        body.slice(0, cursorPosition),
        handler.example,
        body.slice(cursorPosition)
      ].join("");
    }
  }
};
</script>

<style scoped>
.form {
  margin-left: 20px;
  margin-right: 20px;
}

.row {
  border-bottom: 1px solid #d5d5d5;
}

input[type="file"] {
  display: none;
}
</style>
