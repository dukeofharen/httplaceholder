<template>
  <v-row v-shortkey="['ctrl', 's']" @shortkey="save">
    <v-col>
      <h1>{{ title }}</h1>
      <v-card>
        <v-card-text>
          Fill in the stub below in YAML format and click on "Save". For
          examples, visit
          <a
            href="https://github.com/dukeofharen/httplaceholder"
            target="_blank"
          >https://github.com/dukeofharen/httplaceholder</a
          >.
        </v-card-text>
      </v-card>
      <v-card class="editor">
        <v-card-text v-if="newStub">
          <v-row>
            <v-col>
              You can also select an example from the list below.
              <br/>
              <strong>WARNING</strong> The stub in the textbox below will be
              overwritten!
              <v-select
                :items="stubExamples"
                placeholder="Select a stub example..."
                v-model="selectedStubExample"
                item-text="name"
                item-value="key"
                @change="stubExampleSelected"
                clearable
              ></v-select>
            </v-col>
          </v-row>
        </v-card-text>
        <v-card-actions>
          <codemirror v-model="input" :options="cmOptions"></codemirror>
        </v-card-actions>
      </v-card>
      <v-btn color="success" @click="save">Save</v-btn>
    </v-col>
  </v-row>
</template>

<script>
import {codemirror} from "vue-codemirror";
import yaml from "js-yaml";
import stubExamples from "@/stub_examples.json";
import {toastError, toastSuccess} from "@/utils/toastUtil";
import {resources} from "@/shared/resources";
import {routeNames} from "@/router/routerConstants";

export default {
  name: "stubForm",
  data() {
    return {
      stubId: null,
      input: "",
      selectedStubExample: {},
      stubExamples: stubExamples,
      cmOptions: {
        tabSize: 4,
        mode: "text/x-yaml",
        lineNumbers: true,
        line: true
      }
    };
  },
  components: {
    codemirror
  },
  async mounted() {
    if (this.darkTheme) {
      this.cmOptions.theme = "material-darker";
    }

    const stubId = this.$route.params.stubId;
    if (stubId) {
      this.stubId = stubId;
      const fullStub = await this.$store.dispatch("stubs/getStub", {
        stubId
      });
      this.input = yaml.dump(fullStub.stub);
    } else {
      this.stubId = null;
      this.input = resources.defaultStub;
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
    }
  },
  methods: {
    async save() {
      if (this.newStub) {
        try {
          const results = await this.$store.dispatch("stubs/addStubs", {
            input: this.input
          });
          for (let result of results) {
            if (result.v) {
              toastSuccess(
                resources.stubAddedSuccessfully.format(result.v.stub.id)
              );
            } else if (result.e) {
              toastError(resources.stubNotAdded.format(result.e.stubId));
            }
          }
          if (results.length === 1 && results[0].v) {
            await this.$router.push({name: routeNames.stubForm, params: {stubId: results[0].v.stub.id}});
          }

        } catch (e) {
          toastError(e);
        }
      } else {
        try {
          await this.$store.dispatch("stubs/updateStub", {
            input: this.input,
            stubId: this.stubId
          });
          toastSuccess(resources.stubUpdatedSuccessfully.format(this.stubId));
        } catch (e) {
          if (e.response) {
            if (e.response.status === 409) {
              toastError(resources.stubAlreadyAdded.format(this.stubId));
            } else {
              toastError(resources.stubNotAdded.format(this.stubId));
            }
          } else {
            toastError(e);
          }
        }
      }
    },
    stubExampleSelected(key) {
      if (key !== "empty") {
        let stub = this.stubExamples.find(e => e.key === key);
        this.selectedStubExample = stub;
        this.input = stub.stub;
      }
    }
  }
};
</script>

<style scoped>
/*noinspection CssUnusedSymbol*/
.v-card {
  margin-top: 10px;
  margin-bottom: 10px;
}
</style>
