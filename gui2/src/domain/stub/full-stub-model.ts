import type { StubModel } from "@/domain/stub/stub-model";
import type { StubMetadataModel } from "@/domain/stub/stub-metadata-model";
import type { FullStubOverviewModel } from "@/domain/stub/full-stub-overview-model";

export interface FullStubModel extends FullStubOverviewModel {
  stub: StubModel;
  metadata: StubMetadataModel;
}
