import type { StubConditionsModel } from '@/domain/stub/stub-conditions-model'
import type { StubResponseModel } from '@/domain/stub/stub-response-model'
import type { StubOverviewModel } from '@/domain/stub/stub-overview-model'

export interface StubModel extends StubOverviewModel {
  id: string
  conditions: StubConditionsModel
  response: StubResponseModel
  priority: number
  tenant?: string
  description?: string
  enabled: boolean
  scenario?: string
}
