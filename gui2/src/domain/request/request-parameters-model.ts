import type { HashMap } from "@/domain/hash-map";

export interface RequestParametersModel {
  method: string;
  url: string;
  body: string;
  headers: HashMap;
  clientIp: string;
}
