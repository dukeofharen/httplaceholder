import type { App } from "vue";
import { translations } from "@/i18n/en";

export default {
  install: (app: App) => {
    app.config.globalProperties.$translate = (key: string): string => {
      // TODO use "en" as default and additionally select another language.
      return key.split(".").reduce((o, i) => {
        if (o) return o[i];
      }, translations as any);
    };
  },
};
