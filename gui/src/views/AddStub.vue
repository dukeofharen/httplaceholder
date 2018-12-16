<template>
  <div class="add-stub" v-shortkey="['ctrl', 's']" @shortkey="addStubs">
    <h1>Add stub(s)</h1>

    <p>
      You can add new stubs here. Fill in the stub below in YAML format and click on "Add stub(s)". For examples, visit
      <a
        href="https://github.com/dukeofharen/httplaceholder"
        target="_blank"
      >https://github.com/dukeofharen/httplaceholder</a>.
    </p>

    <div class="input-group">
      <codemirror v-model="input" :options="cmOptions"></codemirror>
    </div>

    <div class="input-group">
      <a class="btn btn-success" v-on:click="addStubs">Add stub(s)</a>
    </div>
  </div>
</template>

<script>
import { codemirror } from "vue-codemirror";
import resources from "@/resources";

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
  created() {
    this.setTheme();
  },
  computed: {
    theme() {
      return this.$store.getters.getTheme;
    }
  },
  components: {
    codemirror
  },
  methods: {
    addStubs() {
      this.$store.dispatch("addStubs", { input: this.input });
    },
    setTheme() {
      this.cmOptions.theme = this.theme.codeMirrorTheme;
    }
  },
  watch: {
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

.add-stub {
  text-align: left;
}
</style>