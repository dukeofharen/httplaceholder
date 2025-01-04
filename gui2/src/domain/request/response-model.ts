import type { HashMap } from '@/domain/ui/hash-map.ts'

export interface ResponseModel {
  statusCode: number
  body: string
  bodyIsBinary: boolean
  headers: HashMap
}
