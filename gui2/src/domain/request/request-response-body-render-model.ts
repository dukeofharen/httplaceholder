import { type HashMap } from '@/domain/ui/hash-map.ts'

export interface RequestResponseBodyRenderModel {
  bodyIsBinary: boolean
  base64DecodeNotBinary: boolean
  body: string
  headers: HashMap
}
