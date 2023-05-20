<template>
  <codemirror v-model="code" :extensions="extensions" @ready="handleReady" />
</template>

<script lang="ts">
import { computed, ref, shallowRef, watch } from "vue";
import { useSettingsStore } from "@/store/settings";
import { defineComponent } from "vue";
import { Codemirror } from "vue-codemirror";
import { StreamLanguage } from "@codemirror/language";
import { yaml } from "@codemirror/legacy-modes/mode/yaml";
import { oneDark } from "@/plugins/codemirror/material-one-dark.js";
import { html } from "@codemirror/lang-html";
import { xml } from "@codemirror/lang-xml";
import { json } from "@codemirror/lang-json";

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
    const code = ref(props.modelValue);
    const view = shallowRef();

    // Computed
    const extensions = computed(() => {
      const result = [];
      switch (props.language) {
        case "yaml":
          result.push(StreamLanguage.define(yaml));
          break;
        case "html":
          result.push(html());
          break;
        case "xml":
          result.push(xml());
          break;
        case "json":
          result.push(json());
          break;
      }

      if (generalStore.getDarkTheme) {
        result.push(oneDark);
      }

      return result;
    });

    // Events
    const handleReady = (payload) => {
      view.value = payload.view;
    };

    // Watch
    watch(
      () => props.modelValue,
      (newModelValue) => (code.value = newModelValue)
    );
    watch(code, (newCode) => emit("update:modelValue", newCode));

    // Methods
    const replaceSelection = (replacement: string) => {
      const state = view.value.state;
      const ranges = state.selection.ranges;
      const range = ranges.length ? ranges[0] : null;
      const from = range ? range.from : 0;
      const to = range ? range.to : 0;
      const newCode = code.value;
      code.value = [
        newCode.slice(0, from),
        replacement,
        newCode.slice(to),
      ].join("");
    };

    return { code, extensions, replaceSelection, handleReady };
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
    //   }
    // };
  },
});
</script>

<style lang="scss" scoped>
.editor-wrapper {
  width: 100%;
  overflow-x: hidden;
}
</style>
