<template>
  <v-row class="add-stub" v-shortkey="['ctrl', 's']" @shortkey="addStubs">
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
        line: true,
        theme: ""
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
    }
  }
};
</script>

<style scoped>
.editor {
  margin-top: 10px;
  margin-bottom: 10px;
}
</style>