import type { RequestParametersModel } from "@/domain/request/request-parameters-model";
import type { StubExecutionResultModel } from "@/domain/request/stub-execution-result-model";
import type { StubResponseWriterResultModel } from "@/domain/request/stub-response-writer-result-model";

export interface RequestResultModel {
  correlationId: string;
  requestParameters: RequestParametersModel;
  stubExecutionResults: StubExecutionResultModel[];
  stubResponseWriterResults: StubResponseWriterResultModel[];
  executingStubId: string;
  stubTenant: string;
  requestBeginTime: Date;
  requestEndTime: Date;
}
