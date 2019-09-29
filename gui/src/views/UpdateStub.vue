<template>
  <v-row class="add-stub" v-shortkey="['ctrl', 's']" @shortkey="updateStub">
    <v-col>
      <h1>Update stub</h1>
      <v-card>
        <v-card-text>
          Fill in the stub below in YAML format and click on "Update stub". For examples, visit
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
      <v-btn color="success" @click="updateStub">Update stub</v-btn>
    </v-col>
  </v-row>
</template>

<script>
import { codemirror } from "vue-codemirror";
import yaml from "js-yaml";

export default {
  name: "updateStub",
  data() {
    return {
      input: "",
      cmOptions: {
        tabSize: 4,
        mode: "text/x-yaml",
        lineNumbers: true,
        line: true
      }
    };
  },
  created() {
    this.$store.dispatch("getStub", { stubId: this.$route.params.stubId });
    if (this.darkTheme) {
      this.cmOptions.theme = "material-darker";
    }
  },
  components: {
    codemirror
  },
  computed: {
    lastSelectedStub() {
      return this.$store.getters.getLastSelectedStub;
    },
    darkTheme() {
      return this.$store.getters.getDarkTheme;
    }
  },
  methods: {
    updateStub() {
      this.$store.dispatch("updateStub", {
        input: this.input,
        stubId: this.$route.params.stubId
      });
    }
  },
  watch: {
    lastSelectedStub(newInput) {
      this.input = yaml.dump(newInput.fullStub.stub);
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