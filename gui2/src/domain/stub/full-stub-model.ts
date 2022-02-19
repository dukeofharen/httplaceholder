import type { StubModel } from "@/domain/stub/stub-model";
import type { StubMetadataModel } from "@/domain/stub/stub-metadata-model";

export interface FullStubModel {
  stub: StubModel;
  metadata: StubMetadataModel;
}
