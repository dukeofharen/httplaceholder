declare module "@vue/runtime-core" {
  interface ComponentCustomProperties {
    $translate: (key: string) => string;
    $translateWithMarkdown: (key: string) => string;
    $vsprintf: (format: string, args: any[]) => string;
  }
}

export {};
