import type { ExampleModel } from "@/domain/example-model";

export function getExamples(): ExampleModel[] {
  return [
    {
      stub: "id: example\ntenant: abc123",
      title: "stub title",
      description: "stub description",
      id: "example-id",
    },
  ];
}
