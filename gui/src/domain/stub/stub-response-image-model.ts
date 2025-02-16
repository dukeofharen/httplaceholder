import type { ResponseImageType } from '@/domain/stub/enums/response-image-type'

export interface StubResponseImageModel {
  type?: ResponseImageType
  width?: number
  height?: number
  backgroundColor?: string
  text?: string
  fontSize?: number
  fontColor?: string
  jpegQuality?: string
  wordWrap: boolean
}
