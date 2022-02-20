import type { HashMap } from "@/domain/hash-map";

export interface StubUrlConditionModel {
  path?: string;
  query?: HashMap;
  fullPath?: string;
  isHttps?: boolean;
}
