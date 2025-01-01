import type { VariableHandlerModel } from '@/domain/metadata/variable-handler-model'

export interface MetadataModel {
  version: string
  runtimeVersion: string
  variableHandlers: VariableHandlerModel[]
}
