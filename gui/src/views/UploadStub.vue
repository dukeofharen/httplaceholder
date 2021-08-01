<template>
  <v-row>
    <v-col>
      <h1>Upload stub(s)</h1>
      <v-card class="pt-3 pb-3 pl-4">
        <v-card-title>You can upload stubs here</v-card-title>
        <v-card-text
          >Click the button and select a .yml file with stubs from your PC.
        </v-card-text>
        <v-card-actions class="ml-2">
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
import { toastError, toastSuccess, toastWarning } from "@/utils/toastUtil";
import { resources } from "@/shared/resources";
import { getExtension } from "@/utils/fileHelper";

export default {
  name: "uploadStub",
  methods: {
    uploadStubs() {
      this.$refs.stubUpload.click();
    },
    loadTextFromFile(ev) {
      const expectedExtensions = ["yml", "yaml"];
      const files = Array.from(ev.target.files);
      const invalidFileNames = files
        .filter(f => !expectedExtensions.includes(getExtension(f.name)))
        .map(f => f.name);
      if (invalidFileNames.length) {
        toastWarning(
          resources.uploadInvalidFiles.format(invalidFileNames.join(", ")) +
            " " +
            resources.onlyUploadYmlFiles
        );
      }

      const validFiles = files.filter(f => !invalidFileNames.includes(f.name));
      for (let file of validFiles) {
        let reader = new FileReader();
        reader.onload = e => {
          this.addStubsInternal(e.target.result, file.name);
        };
        reader.readAsText(file);
      }
    },
    async addStubsInternal(input, filename) {
      try {
        await this.$store.dispatch("stubs/addStubs", {
          input
        });
        toastSuccess(resources.stubsInFileAddedSuccessfully.format(filename));
      } catch (e) {
        toastError(e);
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

input[type="file"] {
  display: none;
}
</style>
