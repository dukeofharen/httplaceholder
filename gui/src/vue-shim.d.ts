import type { TranslateWithMarkdownOptions } from "@/utils/translate";

declare module "@vue/runtime-core" {
  interface ComponentCustomProperties {
    $translate: (key: string) => string;
    $translateWithMarkdown: (
      key: string,
      options: TranslateWithMarkdownOptions | null = null,
    ) => string;
    $vsprintf: (format: string, args: any[]) => string;
  }
}

export {};
