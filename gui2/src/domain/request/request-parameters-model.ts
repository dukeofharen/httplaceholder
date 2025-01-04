import type { HashMap } from '@/domain/ui/hash-map.ts'

export interface RequestParametersModel {
  method: string
  url: string
  body: string
  bodyIsBinary: boolean
  headers: HashMap
  clientIp: string
}
