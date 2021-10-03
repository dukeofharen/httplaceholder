import Modal from "@/components/bootstrap/Modal";
import Accordion from "@/components/bootstrap/Accordion";
import AccordionItem from "@/components/bootstrap/AccordionItem";
import SlideUpDown from "vue3-slide-up-down";
import CodeMirror from "@/components/codemirror/CodeMirror";

export default function registerGlobalComponents(vueApp) {
  vueApp.component("modal", Modal);
  vueApp.component("accordion", Accordion);
  vueApp.component("accordion-item", AccordionItem);
  vueApp.component("slide-up-down", SlideUpDown);
  vueApp.component("codemirror", CodeMirror);
}
