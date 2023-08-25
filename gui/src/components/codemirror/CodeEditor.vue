<template>
  <div ref="container"></div>
</template>

<script lang="ts">
import { basicSetup } from "codemirror";
import { computed, onMounted, ref, shallowRef, watch } from "vue";
import { useSettingsStore } from "@/store/settings";
import { defineComponent } from "vue";
import { StreamLanguage } from "@codemirror/language";
import { yaml } from "@codemirror/legacy-modes/mode/yaml";
import { oneDark } from "@/plugins/codemirror/material-one-dark";
import { html } from "@codemirror/lang-html";
import { xml } from "@codemirror/lang-xml";
import { json } from "@codemirror/lang-json";
import { EditorState } from "@codemirror/state";
import { EditorView, keymap } from "@codemirror/view";
import {
  defaultKeymap,
  history,
  historyKeymap,
  indentWithTab,
} from "@codemirror/commands";

export default defineComponent({
  name: "CodeEditor",
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
    const container = shallowRef<HTMLDivElement>();
    const state = shallowRef<EditorState>();
    const view = shallowRef<EditorView>();
    const code = ref(props.modelValue);

    // Watch
    watch(
      () => props.modelValue,
      (newModelValue) => {
        code.value = newModelValue;
        if (view.value) {
          if (view.value.state.doc.toString() !== code.value) {
            view.value.dispatch({
              changes: {
                from: 0,
                to: view.value.state.doc.length,
                insert: code.value,
              },
            });
          }
        }
      },
    );
    watch(code, (newCode) => emit("update:modelValue", newCode));

    // Methods
    const replaceSelection = (replacement: string) => {
      const state = view.value?.state;
      if (!state) {
        return;
      }

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

    // Lifecycle
    onMounted(() => {
      const extensions = [];
      switch (props.language) {
        case "yaml":
          extensions.push(StreamLanguage.define(yaml));
          break;
        case "html":
          extensions.push(html());
          break;
        case "xml":
          extensions.push(xml());
          break;
        case "json":
          extensions.push(json());
          break;
      }

      if (generalStore.getDarkTheme) {
        extensions.push(oneDark);
      }

      state.value = EditorState.create({
        doc: props.modelValue,
        extensions: extensions.concat([
          basicSetup,
          EditorView.updateListener.of((viewUpdate) => {
            if (viewUpdate.docChanged) {
              code.value = viewUpdate.state.doc.toString();
            }
          }),
          EditorState.tabSize.of(14),
          history(),
          keymap.of([...defaultKeymap, ...historyKeymap, indentWithTab]),
        ]),
      });

      view.value = new EditorView({
        state: state.value,
        parent: container.value!,
      });
    });

    return { container, replaceSelection };
  },
});
</script>

<style lang="scss" scoped>
.editor-wrapper {
  width: 100%;
  overflow-x: hidden;
}
</style>
