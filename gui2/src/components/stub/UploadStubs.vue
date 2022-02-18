<template>
  <div class="mb-2 col-md-6">
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
import { handleHttpError } from "@/utils/error";
import { useRouter } from "vue-router";
import { success, warning } from "@/utils/toast";
import { useStubsStore } from "@/store/stubs";
import { defineComponent } from "vue";

const expectedExtensions = ["yml", "yaml"];

export default defineComponent({
  name: "UploadStubs",
  setup() {
    const stubStore = useStubsStore();
    const router = useRouter();

    // Data
    let reloadHandle = null;

    // Methods
    const onUploaded = async (file) => {
      if (!expectedExtensions.includes(getExtension(file.filename))) {
        warning(
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
        await stubStore.addStubs(input);
        success(resources.stubsInFileAddedSuccessfully.format(filename));
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
});
</script>
