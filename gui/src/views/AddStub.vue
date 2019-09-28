<template>
  <v-row v-shortkey="['ctrl', 's']" @shortkey="addStubs">
    <v-col>
      <h1>Add stub(s)</h1>
      <v-card>
        <v-card-title>You can add new stubs here</v-card-title>
        <v-card-text>
          Fill in the stub below in YAML format and click on "Add stub(s)". For examples, visit
          <a
            href="https://github.com/dukeofharen/httplaceholder"
            target="_blank"
          >https://github.com/dukeofharen/httplaceholder</a>.
        </v-card-text>
      </v-card>
      <v-card class="editor">
        <v-card-actions>
          <codemirror v-model="input" :options="cmOptions"></codemirror>
        </v-card-actions>
      </v-card>
      <v-btn color="success" @click="addStubs">Add stub(s)</v-btn>
      <h1>Upload stub(s)</h1>
      <v-card>
        <v-card-title>You can upload stubs here</v-card-title>
        <v-card-text>Click the button and select a .yml file with stubs from your PC.</v-card-text>
        <v-card-actions>
          <input type="file" name="file" ref="stubUpload" @change="loadTextFromFile" />
          <v-btn color="success" @click="uploadStubs">Upload stubs</v-btn>
        </v-card-actions>
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
import { codemirror } from "vue-codemirror";
import { resources } from "@/resources";

export default {
  name: "addStub",
  data() {
    return {
      input: resources.defaultStub,
      cmOptions: {
        tabSize: 4,
        mode: "text/x-yaml",
        lineNumbers: true,
        line: true
      }
    };
  },
  created() {},
  components: {
    codemirror
  },
  methods: {
    addStubs() {
      this.$store.dispatch("addStubs", { input: this.input });
    },
    uploadStubs() {
      this.$refs.stubUpload.click();
    },
    loadTextFromFile(ev) {
      const file = ev.target.files[0];
      const reader = new FileReader();
      reader.onload = e => {
        this.$store.dispatch("addStubs", { input: e.target.result });
      };
      reader.readAsText(file)
    }
  }
};
</script>

<style scoped>
.editor {
  margin-top: 10px;
  margin-bottom: 10px;
}
input[type="file"] {
  display: none;
}
</style>