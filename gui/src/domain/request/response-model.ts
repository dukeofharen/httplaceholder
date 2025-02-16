import type { HashMap } from '@/domain/hash-map'

export interface ResponseModel {
  statusCode: number
  body: string
  bodyIsBinary: boolean
  headers: HashMap
}
