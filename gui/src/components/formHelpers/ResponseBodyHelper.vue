<template>
  <div class="form">
    <v-row>
      <v-col cols="12">
        <p>
          Select a type of response and fill in the actual response that should be returned and press "Insert".
        </p>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-select
          :items="responseBodyTypeSelectList"
          v-model="responseBodyType"
          label="Select a response type..."/>
      </v-col>
    </v-row>
    <v-row v-if="responseBodyType === responseBodyTypes.base64">
      <v-col cols="12">
        <input
          type="file"
          name="file"
          ref="base64Upload"
          @change="upload"
        />
        <p>
          You can upload a file for use in the Base64 response.
        </p>
        <v-btn color="primary" @click="uploadClick">Upload a file</v-btn>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-textarea label="Fill in the response..." v-model="responseBody"></v-textarea>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-btn color="success" @click="insert">Insert</v-btn>
        <v-btn color="error" @click="close">Close</v-btn>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import {responseBodyTypes} from "@/shared/stubFormResources";

export default {
  mounted() {
    this.responseBodyType = this.$store.getters["stubForm/getResponseBodyType"];
    this.responseBody = this.$store.getters["stubForm/getResponseBody"];
  },
  data() {
    return {
      responseBodyTypes,
      responseBodyType: "",
      responseBody: ""
    };
  },
  computed: {
    responseBodyTypeSelectList() {
      return Object.keys(responseBodyTypes).map(k => responseBodyTypes[k]);
    }
  },
  methods: {
    insert() {
      this.$store.commit("stubForm/setResponseBody", {type: this.responseBodyType, body: this.responseBody});
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
        this.$store.commit("stubForm/setResponseBody", {type: responseBodyTypes.base64, body});
        this.responseBody = body;
        this.$store.commit("stubForm/closeFormHelper");
      };
      reader.readAsDataURL(file);
    }
  }
};
</script>

<style scoped>
.form {
  margin-left: 20px;
}

input[type="file"] {
  display: none;
}
</style>
