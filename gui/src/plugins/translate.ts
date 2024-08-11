import type { App } from "vue";
import { translations as en } from "@/i18n/en";
import { translations as nl } from "@/i18n/nl";
import { useSettingsStore } from "@/store/settings";
import { defaultLanguage } from "@/constants/technical";

const langMapping: Record<string, any> = {
  en: en,
  nl: nl,
};

function getTranslation(key: string, translations: any) {
  return key.split(".").reduce((o, i) => {
    if (o) return o[i];
  }, translations as any);
}

export default {
  install: (app: App) => {
    app.config.globalProperties.$translate = (key: string): string => {
      const settingsStore = useSettingsStore();
      const currentLanguageCode = settingsStore.getLanguage;
      const isDefaultLanguage = currentLanguageCode === defaultLanguage;
      const currentLanguage = Object.keys(langMapping).includes(
        currentLanguageCode,
      )
        ? langMapping[currentLanguageCode]
        : langMapping[defaultLanguage];
      return (
        (!isDefaultLanguage ? getTranslation(key, currentLanguage) : null) ??
        getTranslation(key, langMapping[defaultLanguage]) ??
        key
      );
    };
  },
};
