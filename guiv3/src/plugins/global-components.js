import Modal from "@/components/bootstrap/Modal";
import Accordion from "@/components/bootstrap/Accordion";
import AccordionItem from "@/components/bootstrap/AccordionItem";

export default function registerGlobalComponents(vueApp) {
  vueApp.component("modal", Modal);
  vueApp.component("accordion", Accordion);
  vueApp.component("accordion-item", AccordionItem);
}
