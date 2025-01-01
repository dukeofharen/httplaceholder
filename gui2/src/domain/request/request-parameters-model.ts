import type { HashMap } from '@/domain/hash-map'

export interface RequestParametersModel {
  method: string
  url: string
  body: string
  bodyIsBinary: boolean
  headers: HashMap
  clientIp: string
}
