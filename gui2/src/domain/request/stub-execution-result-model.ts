import type { ConditionCheckResultModel } from "@/domain/request/condition-check-result-model";

export interface StubExecutionResultModel {
  stubId: string;
  passed: boolean;
  conditions: ConditionCheckResultModel[];
}
