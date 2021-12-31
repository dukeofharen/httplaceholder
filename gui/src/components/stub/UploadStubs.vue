<template>
  <div class="mb-2">
    Press the button below to upload a YAML file with stubs.
  </div>
  <span>
    <upload-button
      button-text="Upload stubs"
      multiple="true"
      @uploaded="onUploaded"
    />
  </span>
</template>

<script>
import { getExtension } from "@/utils/file";
import { resources } from "@/constants/resources";
import toastr from "toastr";
import { useStore } from "vuex";
import { handleHttpError } from "@/utils/error";
import { useRouter } from "vue-router";

const expectedExtensions = ["yml", "yaml"];

export default {
  name: "UploadStubs",
  setup() {
    const store = useStore();
    const router = useRouter();

    // Data
    let reloadHandle = null;

    // Methods
    const onUploaded = async (file) => {
      if (!expectedExtensions.includes(getExtension(file.filename))) {
        toastr.warning(
          resources.uploadInvalidFiles.format(file.filename) +
            " " +
            resources.onlyUploadYmlFiles
        );
        return;
      }

      try {
        await addStubs(file.result, file.filename);
      } catch (e) {
        handleHttpError(e);
      }
    };
    const addStubs = async (input, filename) => {
      try {
        await store.dispatch("stubs/addStubs", input);
        toastr.success(resources.stubsInFileAddedSuccessfully.format(filename));
        if (reloadHandle) {
          clearTimeout(reloadHandle);
        }

        reloadHandle = setTimeout(async () => {
          await router.push({ name: "Stubs" });
        }, 200);
      } catch (e) {
        handleHttpError(e);
      }
    };

    return { onUploaded };
  },
};
</script>
