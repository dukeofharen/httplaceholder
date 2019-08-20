<template>
  <div class="update-stub" v-shortkey="['ctrl', 's']" @shortkey="updateStub">
    <h1>Update stub</h1>

    <p>
      For examples, visit
      <a
        href="https://github.com/dukeofharen/httplaceholder"
        target="_blank"
      >https://github.com/dukeofharen/httplaceholder</a>.
    </p>

    <div class="input-group">
      <codemirror v-model="input" :options="cmOptions"></codemirror>
    </div>

    <div class="input-group">
      <a class="btn btn-success" v-on:click="updateStub">Update stub</a>
    </div>
  </div>
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
        line: true,
        theme: ""
      }
    };
  },
  created() {
    this.$store.dispatch("getStub", { stubId: this.$route.params.stubId });
    this.setTheme();
  },
  components: {
    codemirror
  },
  computed: {
    lastSelectedStub() {
      return this.$store.getters.getLastSelectedStub;
    },
    theme() {
      return this.$store.getters.getTheme;
    }
  },
  methods: {
    updateStub() {
      this.$store.dispatch("updateStub", { input: this.input, stubId: this.$route.params.stubId });
    },
    setTheme() {
      this.cmOptions.theme = this.theme.codeMirrorTheme;
    }
  },
  watch: {
    lastSelectedStub(newInput) {
      this.input = yaml.dump(newInput.fullStub.stub);
    },
    theme() {
      this.setTheme();
    }
  }
};
</script>

<style scoped>
.vue-codemirror {
  width: 100%;
  margin: 10px;
}
.CodeMirror {
  width: 100%;
}

.update-stub {
  text-align: left;
}
</style>