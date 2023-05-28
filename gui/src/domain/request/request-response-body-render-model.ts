import { type HashMap } from "@/domain/hash-map";

export interface RequestResponseBodyRenderModel {
  bodyIsBinary: boolean;
  base64DecodeNotBinary: boolean;
  body: string;
  headers: HashMap;
}
