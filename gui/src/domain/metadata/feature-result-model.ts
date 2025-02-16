import type { FeatureFlagType } from '@/domain/metadata/feature-flag-type'

export interface FeatureResultModel {
  featureFlag: FeatureFlagType
  enabled: boolean
}
