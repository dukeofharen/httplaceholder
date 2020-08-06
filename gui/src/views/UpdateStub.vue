<template>
  <v-row class="add-stub" v-shortkey="['ctrl', 's']" @shortkey="updateStub">
    <v-col>
      <h1>Update stub</h1>
      <v-card>
        <v-card-text>
          Fill in the stub below in YAML format and click on "Update stub". For
          examples, visit
          <a
            href="https://github.com/dukeofharen/httplaceholder"
            target="_blank"
            >https://github.com/dukeofharen/httplaceholder</a
          >.
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
import { toastError, toastSuccess } from "@/utils/toastUtil";
import { resources } from "@/shared/resources";
import { actionNames } from "@/store/storeConstants";

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
  async created() {
    if (this.darkTheme) {
      this.cmOptions.theme = "material-darker";
    }

    const fullStub = await this.$store.dispatch(actionNames.getStub, {
      stubId: this.$route.params.stubId
    });
    this.input = yaml.dump(fullStub.stub);
  },
  components: {
    codemirror
  },
  computed: {
    darkTheme() {
      return this.$store.getters.getDarkTheme;
    }
  },
  methods: {
    async updateStub() {
      const stubId = this.$route.params.stubId;
      try {
        await this.$store.dispatch(actionNames.updateStub, {
          input: this.input,
          stubId
        });
        toastSuccess(resources.stubUpdatedSuccessfully.format(stubId));
      } catch (e) {
        if (e.response) {
          if (e.response.status === 409) {
            toastError(resources.stubAlreadyAdded.format(stubId));
          } else {
            toastError(resources.stubNotAdded.format(stubId));
          }
        } else {
          toastError(e);
        }
      }
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
