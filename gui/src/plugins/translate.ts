import type { App } from "vue";
import { translate } from "@/utils/translate";

export default {
  install: (app: App) => {
    app.config.globalProperties.$translate = translate;
  },
};
