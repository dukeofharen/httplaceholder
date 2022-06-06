import type { HashMap } from "@/domain/hash-map";

export interface StubUrlConditionModel {
  path?: any;
  query?: HashMap;
  fullPath?: string;
  isHttps?: boolean;
}
