import type { ConfigKeyType } from '@/domain/stub/enums/config-key-type'

export interface ConfigurationModel {
  key: string
  path: string
  description: string
  configKeyType: ConfigKeyType
  value: string
}
