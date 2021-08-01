<template>
  <v-row v-shortkey="['ctrl', 's']" @shortkey="save">
    <v-col>
      <h1>{{ title }}</h1>
      <v-card class="mt-3 mb-3">
        <v-card-text class="ml-4">
          Fill in the stub below in YAML format and click on "Save". For
          examples, visit
          <a
            href="https://github.com/dukeofharen/httplaceholder"
            target="_blank"
            >https://github.com/dukeofharen/httplaceholder</a
          >.
        </v-card-text>
      </v-card>
      <v-card v-if="showFormHelperSelector" class="mt-3 mb-3 overflow-hidden">
        <v-card-text>
          <FormHelperSelector />
        </v-card-text>
      </v-card>
      <v-card class="mt-3 mb-3">
        <v-card-text>
          <v-row>
            <v-col class="ml-4 mr-2">
              <v-btn
                @click="editor = editorType.codemirror"
                class="mr-2"
                :color="editor === editorType.codemirror ? 'primary' : ''"
                small
                outlined
                >Editor with highlighting
              </v-btn>
              <v-btn
                @click="editor = editorType.simple"
                :color="editor === editorType.simple ? 'primary' : ''"
                small
                outlined
                >Simple editor
              </v-btn>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <v-card class="editor mt-3 mb-3">
        <v-card-actions>
          <codemirror
            v-model="input"
            :options="cmOptions"
            v-if="
              editor === editorType.notSet || editor === editorType.codemirror
            "
          ></codemirror>
          <v-textarea
            v-model="input"
            rows="11"
            v-if="editor === editorType.simple"
            wrap="soft"
            class="simple-editor"
          ></v-textarea>
        </v-card-actions>
      </v-card>
      <v-row>
        <v-col cols="12">
          <v-btn color="success mr-2" @click="save">Save</v-btn>
          <v-btn color="error" @click="resetDialog = true">Reset</v-btn>
        </v-col>
      </v-row>
    </v-col>
    <v-dialog v-model="resetDialog" max-width="290">
      <v-card>
        <v-card-title class="headline">Reset to defaults?</v-card-title>
        <v-card-actions>
          <v-btn color="green darken-1" text @click="resetDialog = false"
            >No
          </v-btn>
          <v-btn color="green darken-1" text @click="resetForm">Yes</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import { codemirror } from "vue-codemirror";
import yaml from "js-yaml";
import { toastError, toastSuccess } from "@/utils/toastUtil";
import { resources } from "@/shared/resources";
import {
  getIntermediateStub,
  clearIntermediateStub
} from "@/utils/sessionUtil";
import FormHelperSelector from "@/components/formHelpers/FormHelperSelector";

const editorType = {
  notSet: "notSet",
  simple: "simple",
  codemirror: "codemirror"
};

export default {
  name: "stubForm",
  data() {
    return {
      stubId: null,
      resetDialog: false,
      editorType,
      editor: editorType.notSet,
      cmOptions: {
        tabSize: 4,
        mode: "text/x-yaml",
        lineNumbers: true,
        line: true
      }
    };
  },
  components: {
    codemirror,
    FormHelperSelector
  },
  created() {
    if (this.darkTheme) {
      this.cmOptions.theme = "material-darker";
    }
  },
  beforeMount() {
    this.$store.commit("stubForm/clearForm");
  },
  async mounted() {
    const stubId = this.$route.params.stubId;
    if (stubId) {
      this.stubId = stubId;
      const fullStub = await this.$store.dispatch("stubs/getStub", {
        stubId
      });
      const input = yaml.dump(fullStub.stub);

      // If the stub is too large, codemirror will struggle with editing. Switch to a simple editor in that case.
      this.editor =
        input.length >= 1500 ? editorType.simple : editorType.codemirror;
      await this.$store.commit("stubForm/setInput", input);
    } else {
      this.stubId = null;
      let input;
      const intermediateStub = getIntermediateStub();
      if (intermediateStub) {
        input = intermediateStub;
        clearIntermediateStub();
      } else {
        input = resources.defaultStub;
      }

      await this.$store.commit("stubForm/setInput", input);
    }
  },
  computed: {
    darkTheme() {
      return this.$store.getters["general/getDarkTheme"];
    },
    newStub() {
      return !this.stubId;
    },
    title() {
      return this.newStub ? "Add stub(s)" : "Update stub";
    },
    input: {
      get() {
        return this.$store.getters["stubForm/getInput"];
      },
      set(value) {
        this.$store.commit("stubForm/setInput", value);
      }
    },
    showFormHelperSelector() {
      return this.input.indexOf("- ") !== 0;
    }
  },
  methods: {
    async save() {
      try {
        await this.$store.dispatch("stubs/addStubs", {
          input: this.input
        });
        if (this.newStub) {
          toastSuccess(resources.stubsAddedSuccessfully);
        } else {
          toastSuccess(resources.stubUpdatedSuccessfully.format(this.stubId));
        }
      } catch (e) {
        toastError(e);
      }
    },
    resetForm() {
      this.resetDialog = false;
      this.$store.commit("stubForm/clearForm", resources.defaultStub);
    }
  }
};
</script>

<style>
.simple-editor textarea {
  white-space: pre;
  overflow-wrap: normal;
  overflow-x: scroll;
}
</style>
