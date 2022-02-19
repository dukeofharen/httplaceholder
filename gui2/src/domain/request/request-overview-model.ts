export interface RequestOverviewModel {
  correlationId: string;
  method: string;
  url: string;
  executingStubId: string;
  stubTenant: string;
  requestBeginTime: Date;
  requestEndTime: Date;
}
