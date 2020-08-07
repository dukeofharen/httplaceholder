<template>
  <v-row>
    <v-col>
      <h1>Upload stub(s)</h1>
      <v-card>
        <v-card-title>You can upload stubs here</v-card-title>
        <v-card-text
        >Click the button and select a .yml file with stubs from your PC.
        </v-card-text>
        <v-card-actions>
          <input
            type="file"
            name="file"
            ref="stubUpload"
            @change="loadTextFromFile"
            multiple
          />
          <v-btn color="success" @click="uploadStubs">Upload stubs</v-btn>
        </v-card-actions>
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
  import {toastError, toastSuccess} from "@/utils/toastUtil";
  import {actionNames} from "@/store/storeConstants";
  import {resources} from "@/shared/resources";

  export default {
    name: "uploadStub",
    methods: {
      uploadStubs() {
        this.$refs.stubUpload.click();
      },
      loadTextFromFile(ev) {
        const expectedExtensions = ["yml", "yaml"];
        for (let file of ev.target.files) {
          let parts = file.name.split(".");
          if (!expectedExtensions.includes(parts[parts.length - 1])) {
            toastError(resources.onlyUploadYmlFiles);
            return;
          }
        }

        for (let file of ev.target.files) {
          let reader = new FileReader();
          reader.onload = e => {
            this.addStubsInternal(e.target.result);
          };
          reader.readAsText(file);
        }
      },
      async addStubsInternal(input) {
        try {
          const results = await this.$store.dispatch(actionNames.addStubs, {
            input
          });
          for (let result of results) {
            if (result.v) {
              toastSuccess(resources.stubAddedSuccessfully.format(result.v.id));
            } else if (result.e) {
              toastError(resources.stubNotAdded.format(result.e.stubId));
            }
          }
        } catch (e) {
          toastError(e);
        }
      }
    }
  };
</script>

<style scoped>
  .v-card {
    margin-top: 10px;
    margin-bottom: 10px;
  }

  input[type="file"] {
    display: none;
  }
</style>
