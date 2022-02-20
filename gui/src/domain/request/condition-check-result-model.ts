import type { ConditionValidationType } from "@/domain/request/enums/condition-validation-type";

export interface ConditionCheckResultModel {
  checkerName: string;
  conditionValidation: ConditionValidationType;
  log: string;
}
