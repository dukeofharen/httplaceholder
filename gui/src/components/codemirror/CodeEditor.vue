<template>
  <div ref="container"></div>
</template>

<script lang="ts">
import { basicSetup } from "codemirror";
import { computed, onMounted, ref, shallowRef, watch } from "vue";
import { useSettingsStore } from "@/store/settings";
import { defineComponent } from "vue";
import {
  Language,
  LanguageSupport,
  StreamLanguage,
} from "@codemirror/language";
import { yaml } from "@codemirror/legacy-modes/mode/yaml";
import { oneDark } from "@/plugins/codemirror/material-one-dark";
import { html } from "@codemirror/lang-html";
import { xml } from "@codemirror/lang-xml";
import { json } from "@codemirror/lang-json";
import { EditorState, Compartment } from "@codemirror/state";
import { EditorView, keymap } from "@codemirror/view";
import {
  defaultKeymap,
  history,
  historyKeymap,
  indentWithTab,
} from "@codemirror/commands";
import { markdownLanguage } from "@codemirror/lang-markdown";
import { copyTextToClipboard } from "@/utils/clipboard";

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
    const language = new Compartment();

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
    watch(
      () => props.language,
      () => {
        if (cmLanguage.value) {
          view.value?.dispatch({
            effects: language.reconfigure(cmLanguage.value),
          });
        }
      },
    );

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
    const lineDeletion = (target: EditorView) => {
      const state = target.state;
      const ranges = state.selection.ranges;
      const hasSelection = ranges.length
        ? ranges[0].to - ranges[0].from > 0
        : false;
      if (hasSelection) {
        // Don't delete if there is a selection.
        return false;
      }

      // Remove the line.
      const position = state.selection.main.from;
      const line = state.doc.lineAt(position);
      const numOfLines = state.doc.lines;
      const from = line.from;
      const to = line.number !== numOfLines ? line.to + 1 : line.to;
      target.dispatch({
        changes: {
          from,
          to,
          insert: "",
        },
      });

      // Move line to clipboard.
      const lineText = state.doc.toString().slice(from, to);
      copyTextToClipboard(lineText).then();

      return true;
    };

    // Computed
    const cmLanguage = computed<Language | LanguageSupport>(() => {
      switch (props.language) {
        case "html":
          return html();
        case "xml":
          return xml();
        case "json":
          return json();
        case "yaml":
          return StreamLanguage.define(yaml);
        default:
          return markdownLanguage;
      }
    });
    const cmExtensions = computed(() => {
      const extensions = [];
      if (cmLanguage.value) {
        extensions.push(language.of(cmLanguage.value));
      }

      if (generalStore.getDarkTheme) {
        extensions.push(oneDark);
      }

      extensions.push(basicSetup);
      extensions.push(
        EditorView.updateListener.of((viewUpdate) => {
          if (viewUpdate.docChanged) {
            code.value = viewUpdate.state.doc.toString();
          }
        }),
      );
      extensions.push(history());
      extensions.push(
        keymap.of([
          ...defaultKeymap,
          ...historyKeymap,
          indentWithTab,
          { key: "Ctrl-x", mac: "Mod-x", run: lineDeletion },
        ]),
      );

      return extensions;
    });

    // Lifecycle
    onMounted(() => {
      state.value = EditorState.create({
        doc: props.modelValue,
        extensions: cmExtensions.value,
      });

      view.value = new EditorView({
        state: state.value,
        parent: container.value ?? undefined,
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
