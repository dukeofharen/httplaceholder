import type { ExampleModel } from "@/domain/example-model";
import examples from "@/constants/stub-examples.json";

export function getExamples(): ExampleModel[] {
  return examples as ExampleModel[];
}
