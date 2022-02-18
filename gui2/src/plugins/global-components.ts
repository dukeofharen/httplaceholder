import Modal from "@/components/bootstrap/Modal.vue";
import Accordion from "@/components/bootstrap/Accordion.vue";
import AccordionItem from "@/components/bootstrap/AccordionItem.vue";
import SlideUpDown from "vue3-slide-up-down";
import CodeMirror from "@/components/codemirror/CodeMirror.vue";
import SimpleEditor from "@/components/simpleEditor/SimpleEditor.vue";
import UploadButton from "@/components/UploadButton.vue";
import CodeHighlight from "@/components/hljs/CodeHighlight.vue";
import type { App } from "vue";

export function registerGlobalComponents(vueApp: App<Element>): void {
  vueApp.component("modal", Modal);
  vueApp.component("accordion", Accordion);
  vueApp.component("accordion-item", AccordionItem);
  vueApp.component("slide-up-down", SlideUpDown);
  vueApp.component("codemirror", CodeMirror);
  vueApp.component("simpleeditor", SimpleEditor);
  vueApp.component("upload-button", UploadButton);
  vueApp.component("code-highlight", CodeHighlight);
}
