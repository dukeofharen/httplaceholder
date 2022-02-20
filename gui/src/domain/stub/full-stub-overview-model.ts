import type { StubOverviewModel } from "@/domain/stub/stub-overview-model";
import type { StubMetadataModel } from "@/domain/stub/stub-metadata-model";

export interface FullStubOverviewModel {
  stub: StubOverviewModel;
  metadata: StubMetadataModel;
}
