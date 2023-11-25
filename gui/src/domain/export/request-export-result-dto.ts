import type { RequestExportType } from "@/domain/request/enums/request-export-type";

export interface RequestExportResultDto {
  requestExportType: RequestExportType;
  result: string;
}
