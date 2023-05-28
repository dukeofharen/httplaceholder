import { type HashMap } from "@/domain/hash-map";

export interface RequestResponseBodyRenderModel {
  bodyIsBinary: boolean;
  body: string;
  headers: HashMap;
}
