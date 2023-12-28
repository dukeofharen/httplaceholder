<template>
  <div class="mb-2 col-md-6">
    Press the button below to upload a YAML file with stubs.
  </div>
  <span>
    <upload-button
      button-text="Upload stubs"
      :multiple="true"
      @all-uploaded="onAllUploaded"
    />
  </span>
</template>

<script lang="ts">
import { getExtension } from "@/utils/file";
import { resources } from "@/constants/resources";
import { handleHttpError } from "@/utils/error";
import { useRouter } from "vue-router";
import { success, warning } from "@/utils/toast";
import { useStubsStore } from "@/store/stubs";
import { defineComponent } from "vue";
import { vsprintf } from "sprintf-js";
import type { FileUploadedModel } from "@/domain/file-uploaded-model";

const expectedExtensions = ["yml", "yaml"];

export default defineComponent({
  name: "UploadStubs",
  setup() {
    const stubStore = useStubsStore();
    const router = useRouter();

    // Data
    let reloadHandle: any = null;

    // Methods
    const onUploaded = async (file: FileUploadedModel) => {
      if (!expectedExtensions.includes(getExtension(file.filename))) {
        warning(
          vsprintf(resources.uploadInvalidFiles, [file.filename]) +
            " " +
            resources.onlyUploadYmlFiles,
        );
        return;
      }

      console.log(file);
      try {
        await addStubs(file.result, file.filename);
      } catch (e) {
        handleHttpError(e);
      }
    };
    const addStubs = async (input: any, filename: string) => {
      try {
        await stubStore.addStubs(input);
        success(vsprintf(resources.stubsInFileAddedSuccessfully, [filename]));
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

    const onAllUploaded = async (files: FileUploadedModel[]) => {
      console.log(files);
    };

    return { onAllUploaded };
  },
});
</script>
