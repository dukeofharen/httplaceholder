import type { App } from "vue";
import { translate, translateWithMarkdown } from "@/utils/translate";

export default {
  install: (app: App) => {
    app.config.globalProperties.$translate = translate;
    app.config.globalProperties.$translateWithMarkdown = translateWithMarkdown;
  },
};
