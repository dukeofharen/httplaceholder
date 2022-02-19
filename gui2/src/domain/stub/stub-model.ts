import type { StubConditionsModel } from "@/domain/stub/stub-conditions-model";
import type { StubResponseModel } from "@/domain/stub/stub-response-model";

export interface StubModel {
  id: string;
  conditions: StubConditionsModel;
  response: StubResponseModel;
  priority: number;
  tenant?: string;
  description?: string;
  enabled: boolean;
  scenario?: string;
}
