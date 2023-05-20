<template>
  <codemirror v-model="code" :extensions="extensions" />
</template>

<script lang="ts">
import { onMounted, ref, watch } from "vue";
import { useSettingsStore } from "@/store/settings";
import { defineComponent } from "vue";
import { Codemirror } from "vue-codemirror";
import { StreamLanguage } from "@codemirror/language";
import { yaml } from "@codemirror/legacy-modes/mode/yaml";
import { oneDark } from "@/plugins/codemirror/material-one-dark.js";

export default defineComponent({
  name: "CodeEditor",
  components: { Codemirror },
  props: {
    modelValue: {
      type: String,
      default: "",
    },
    language: {
      type: String,
      default: "yaml",
    },
    theme: {
      type: String,
      default: "default",
    },
  },
  setup(props, { emit }) {
    const generalStore = useSettingsStore();

    // Data
    const code = ref("");

    // Watch
    watch(
      () => props.modelValue,
      (newModelValue) => {
        code.value = newModelValue;
      }
    );

    // Functions
    const buildExtensions = () => {
      const extensions = [];
      switch (props.language) {
        case "yaml":
          extensions.push(StreamLanguage.define(yaml));
          break;
      }

      if (generalStore.getDarkTheme) {
        extensions.push(oneDark);
      }

      return extensions;
    };

    return { code, extensions: buildExtensions() };
    // // Methods
    // const initializeCodemirror = () => {
    //   if (editor.value) {
    //     cmInstance = CodeMirror.fromTextArea(editor.value, props.options);
    //     cmInstance.on("change", () => {
    //       if (props.modelValue !== cmInstance.getValue()) {
    //         emit("update:modelValue", cmInstance.getValue());
    //       }
    //     });
    //     cmInstance.setOption("extraKeys", {
    //       Tab: (cm) => {
    //         if (cm.somethingSelected()) {
    //           cm.indentSelection("add");
    //         } else {
    //           // Make sure inserts spaces instead of tabs.
    //           const currentIndent = cm.getOption("indentUnit") || 0;
    //           const spaces = Array(currentIndent + 1).join(" ");
    //           cm.replaceSelection(spaces);
    //         }
    //       },
    //       "Shift-Tab": (cm) => {
    //         if (cm.somethingSelected()) {
    //           cm.indentSelection("subtract");
    //         }
    //       },
    //     });
    //     if (generalStore.getDarkTheme) {
    //       cmInstance.setOption("theme", "material-darker");
    //     }
    //   }
    // };
    // const replaceSelection = (replacement: string, selection: string) => {
    //   if (cmInstance) {
    //     cmInstance.replaceSelection(replacement, selection);
    //   }
    // };
    //
    // // Watch
    // watch(
    //   () => props.options,
    //   () => {
    //     if (cmInstance && props.options) {
    //       const cleanOptions = JSON.parse(JSON.stringify(props.options));
    //       for (const key of Object.keys(cleanOptions)) {
    //         (cmInstance as any).setOption(key, cleanOptions[key]);
    //       }
    //     }
    //   },
    //   { deep: true }
    // );
    //
    // // Lifecycle
    // onMounted(() => initializeCodemirror());
    //
    // return { contents, editor, replaceSelection };
  },
});
</script>

<style lang="scss" scoped>
.editor-wrapper {
  width: 100%;
  overflow-x: hidden;
}
</style>
