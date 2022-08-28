import type { HashMap } from "@/domain/hash-map";
import type { StubResponseReverseProxyModel } from "@/domain/stub/stub-response-reverse-proxy-model";
import type { LineEndingType } from "@/domain/stub/enums/line-ending-type";
import type { StubResponseImageModel } from "@/domain/stub/stub-response-image-model";
import type { StubResponseScenarioModel } from "@/domain/stub/stub-response-scenario-model";

export interface StubResponseModel {
  enableDynamicMode?: boolean;
  statusCode?: number;
  contentType?: string;
  text?: string;
  base64?: string;
  file?: string;
  headers?: HashMap;
  extraDuration?: number;
  json?: string;
  xml?: string;
  html?: string;
  temporaryRedirect?: string;
  permanentRedirect?: string;
  reverseProxy?: StubResponseReverseProxyModel;
  lineEndings?: LineEndingType;
  image?: StubResponseImageModel;
  scenario?: StubResponseScenarioModel;
  abortConnection?: boolean;
}
