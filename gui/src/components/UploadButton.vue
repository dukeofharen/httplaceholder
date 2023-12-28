<template>
  <input
    type="file"
    name="file"
    ref="uploadField"
    @change="loadTextFromFile"
    :multiple="multiple"
  />
  <button :class="buttonClasses" @click="uploadClick">
    {{ buttonText }}
  </button>
</template>

<script lang="ts">
import { defineComponent, type PropType, ref } from "vue";
import type { FileUploadedModel } from "@/domain/file-uploaded-model";
import { UploadButtonType } from "@/domain/upload-button-type";
import { getExtension } from "@/utils/file";
import { vsprintf } from "sprintf-js";
import { resources } from "@/constants/resources";
import { warning } from "@/utils/toast";

export default defineComponent({
  name: "UploadButton",
  props: {
    buttonText: {
      type: String,
      required: true,
    },
    multiple: {
      type: Boolean,
      default: null,
      required: false,
    },
    buttonClasses: {
      type: String,
      default: "btn btn-primary me-2",
      required: false,
    },
    resultType: {
      type: String as PropType<UploadButtonType>,
      default: UploadButtonType.Text,
    },
    allowedExtensions: {
      type: Array as PropType<string[]>,
    },
  },
  emits: ["uploaded", "allUploaded", "beforeUpload"],
  setup(props, { emit }) {
    // Refs
    const uploadField = ref<HTMLElement>();

    // Methods
    const uploadClick = function () {
      if (uploadField.value) {
        uploadField.value.click();
      }
    };

    const handleSingleUploadedFile = (
      file: File,
      onUploaded: (uploadedFile: FileUploadedModel) => void,
      onError: (error: string) => void,
    ) => {
      const allowedExtensions = props?.allowedExtensions ?? [];
      if (
        allowedExtensions.length > 0 &&
        !allowedExtensions.includes(getExtension(file.name))
      ) {
        onError(
          vsprintf(resources.uploadInvalidFiles, [
            file.name,
            allowedExtensions.map((e) => `.${e}`).join(","),
          ]),
        );
        return;
      }

      const reader = new FileReader();
      reader.onload = (e: any) => {
        const uploadedFile: FileUploadedModel = {
          filename: file.name,
          result: e.target.result,
          success: true,
        };
        onUploaded(uploadedFile);
      };
      switch (props.resultType) {
        case UploadButtonType.Text:
          reader.readAsText(file);
          break;
        case UploadButtonType.Base64:
          reader.readAsDataURL(file);
          break;
        default:
          onError(`Result type for upload not supported: ${props.resultType}`);
      }
    };
    const loadTextFromFile = async (ev: any) => {
      emit("beforeUpload");
      const files: File[] = Array.from(ev.target.files);
      if (props.multiple) {
        const promises = [];
        for (const file of files) {
          promises.push(
            new Promise((resolve) => {
              handleSingleUploadedFile(
                file,
                (uploadedFile) => {
                  resolve(uploadedFile);
                },
                (error) => {
                  warning(error);
                  const result: FileUploadedModel = {
                    success: false,
                    filename: file.name,
                    result: undefined,
                  };
                  resolve(result);
                },
              );
            }),
          );
        }

        emit("allUploaded", await Promise.all(promises));
      } else {
        for (const file of files) {
          handleSingleUploadedFile(
            file,
            (uploadedFile) => emit("uploaded", uploadedFile),
            warning,
          );
        }
      }
    };

    return {
      uploadField,
      uploadClick,
      loadTextFromFile,
    };
  },
});
</script>

<style scoped>
input[type="file"] {
  display: none;
}
</style>
