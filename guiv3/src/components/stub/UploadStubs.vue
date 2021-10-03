<template>
  <span>
    <button type="button" class="btn btn-success" @click="upload">
      Upload stubs
    </button>
    <input
      type="file"
      name="file"
      ref="stubUpload"
      @change="loadTextFromFile"
      multiple
    />
  </span>
</template>

<script>
import { ref } from "vue";
import { getExtension } from "@/utils/file";
import { resources } from "@/constants/resources";
import toastr from "toastr";
import { useStore } from "vuex";
import { handleHttpError } from "@/utils/error";

export default {
  name: "UploadStubs",
  setup(_, { emit }) {
    const store = useStore();

    // Refs
    const stubUpload = ref(null);

    // Data
    let reloadHandle = null;

    // Methods
    const upload = () => {
      stubUpload.value.click();
    };
    const addStubs = async (input, filename) => {
      try {
        await store.dispatch("stubs/addStubs", input);
        toastr.success(resources.stubsInFileAddedSuccessfully.format(filename));
        if (reloadHandle) {
          clearTimeout(reloadHandle);
        }

        reloadHandle = setTimeout(() => {
          emit("uploaded");
        }, 200);
      } catch (e) {
        handleHttpError(e);
      }
    };
    const loadTextFromFile = (ev) => {
      const expectedExtensions = ["yml", "yaml"];
      const files = Array.from(ev.target.files);
      const invalidFileNames = files
        .filter((f) => !expectedExtensions.includes(getExtension(f.name)))
        .map((f) => f.name);
      if (invalidFileNames.length) {
        toastr.warning(
          resources.uploadInvalidFiles.format(invalidFileNames.join(", ")) +
            " " +
            resources.onlyUploadYmlFiles
        );
      }

      const validFiles = files.filter(
        (f) => !invalidFileNames.includes(f.name)
      );
      for (let file of validFiles) {
        let reader = new FileReader();
        reader.onload = async (e) => {
          try {
            await addStubs(e.target.result, file.name);
          } catch (e) {
            handleHttpError(e);
          }
        };
        reader.readAsText(file);
      }
    };

    return { upload, stubUpload, loadTextFromFile };
  },
};
</script>

<style scoped>
input[type="file"] {
  display: none;
}
</style>
