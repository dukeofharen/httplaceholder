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

<script>
import { ref } from "vue";

export default {
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
      type: String,
      default: "text",
      validator(value) {
        return ["text", "base64"].includes(value);
      },
    },
  },
  emits: ["uploaded"],
  setup(props, { emit }) {
    // Refs
    const uploadField = ref(null);

    // Methods
    const uploadClick = function () {
      uploadField.value.click();
    };
    const loadTextFromFile = (ev) => {
      const files = Array.from(ev.target.files);
      for (let file of files) {
        let reader = new FileReader();
        reader.onload = (e) => {
          emit("uploaded", {
            filename: file.name,
            result: e.target.result,
          });
        };
        switch (props.resultType) {
          case "text":
            reader.readAsText(file);
            break;
          case "base64":
            reader.readAsDataURL(file);
            break;
          default:
            throw `Result type for upload not supported: ${props.resultType}`;
        }
      }
    };

    return { uploadField, uploadClick, loadTextFromFile };
  },
};
</script>

<style scoped>
input[type="file"] {
  display: none;
}
</style>
