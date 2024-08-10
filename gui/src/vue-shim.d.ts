declare module "@vue/runtime-core" {
  interface ComponentCustomProperties {
    $translate: (key: string) => string;
  }
}

export {};
