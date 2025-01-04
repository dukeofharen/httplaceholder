import type { ExampleModel } from '@/domain/ui/example-model.ts'
import examples from '@/assets/stub-examples.json'

export function getExamples(): ExampleModel[] {
  return JSON.parse(JSON.stringify(examples)) as ExampleModel[]
}
