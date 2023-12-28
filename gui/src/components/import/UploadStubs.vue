<template>
  <div class="mb-2 col-md-6">
    Press the button below to upload a YAML file with stubs.
  </div>
  <span>
    <upload-button
      button-text="Upload stubs"
      :multiple="true"
      @all-uploaded="onAllUploaded"
      :allowed-extensions="['yml', 'yaml']"
    />
  </span>
</template>

<script lang="ts">
import { resources } from "@/constants/resources";
import { handleHttpError } from "@/utils/error";
import { useRouter } from "vue-router";
import { success } from "@/utils/toast";
import { useStubsStore } from "@/store/stubs";
import { defineComponent } from "vue";
import { vsprintf } from "sprintf-js";
import type { FileUploadedModel } from "@/domain/file-uploaded-model";

export default defineComponent({
  name: "UploadStubs",
  setup() {
    const stubStore = useStubsStore();
    const router = useRouter();

    // Methods
    // const onUploaded = async (file: FileUploadedModel) => {
    //   try {
    //     await addStubs(file.result, file.filename);
    //   } catch (e) {
    //     handleHttpError(e);
    //   }
    // };

    const onAllUploaded = async (files: FileUploadedModel[]) => {
      for (const file of files) {
        if (!file.success) {
          continue;
        }

        try {
          await stubStore.addStubs(file.result);
          success(
            vsprintf(resources.stubsInFileAddedSuccessfully, [file.filename]),
          );
        } catch (e) {
          handleHttpError(e);
        }
      }

      await router.push({ name: "Stubs" });
    };

    return { onAllUploaded };
  },
});
</script>
