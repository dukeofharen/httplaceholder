import { useSettingsStore } from "@/store/settings";
import { defaultLanguage } from "@/constants";
import { translations as en } from "@/strings/i18n/en";
import { translations as nl } from "@/strings/i18n/nl";
import { marked } from "marked";

const langMapping: Record<string, any> = {
  en: en,
  nl: nl,
};

function getTranslation(key: string, translations: any) {
  return key.split(".").reduce((o, i) => {
    if (o) return o[i];
  }, translations as any);
}

export function translate(key: string): string {
  const settingsStore = useSettingsStore();
  const currentLanguageCode = settingsStore.getLanguage;
  const isDefaultLanguage = currentLanguageCode === defaultLanguage;
  const currentLanguage = Object.keys(langMapping).includes(currentLanguageCode)
    ? langMapping[currentLanguageCode]
    : langMapping[defaultLanguage];
  return (
    (!isDefaultLanguage ? getTranslation(key, currentLanguage) : null) ??
    getTranslation(key, langMapping[defaultLanguage]) ??
    key
  );
}

export interface TranslateWithMarkdownOptions {
  linkTarget?: string;
}

export function translateWithMarkdown(
  key: string,
  options: TranslateWithMarkdownOptions | null = null,
): string {
  const renderer = new marked.Renderer();
  const linkTarget = options?.linkTarget;
  if (linkTarget) {
    renderer.link = (href, title, text) => {
      return `<a href="${href}" target="${linkTarget}">${text}</a>`;
    };
  }
  return marked.parseInline(translate(key), { renderer }) as string;
}
