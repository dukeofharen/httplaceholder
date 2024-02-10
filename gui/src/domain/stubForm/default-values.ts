import type { StubXpathModel } from "@/domain/stub/stub-xpath-model";
import type { StubResponseImageModel } from "@/domain/stub/stub-response-image-model";
import { ResponseImageType } from "@/domain/stub/enums/response-image-type";
import type { StubResponseReplaceModel } from "@/domain/stub/stub-response-replace-model";

// TODO dit moet straks grotendeels weg
export const defaultValues = {
  methods: ["GET", "POST"],
  priority: 1,
  query: {
    query1: "val1",
    query2: "val2",
  } as any,
  basicAuthentication: {
    username: "username",
    password: "password",
  },
  requestHeaders: {
    Header1: "val1",
    Header2: "val2",
  } as any,
  requestBody: ["val1", "val2"],
  formBody: {
    key1: "val1",
    key2: "val2",
  } as any,
  clientIp: "127.0.0.1",
  hostname: "httplaceholder.com",
  jsonPath: [
    {
      query: "$.people[0].name",
      expectedValue: "John",
    },
  ],
  jsonObject: {
    stringValue: "text",
    intValue: 3,
    array: ["value1", "value2"],
  },
  jsonArray: [
    "value1",
    3,
    {
      key1: "value1",
      key2: 1.45,
    },
  ],
  xpath: [
    {
      queryString: '/object/a[text() = "TEST"]',
    },
    {
      queryString: '/object/b[text() = "TEST"]',
      namespaces: {
        soap: "http://www.w3.org/2003/05/soap-envelope",
        m: "http://www.example.org/stock/Reddy",
      },
    },
  ] as StubXpathModel[],
  responseHeaders: {
    Header1: "val1",
    Header2: "val2",
  },
  extraDuration: 10000,
  redirect: "https://google.com",
  reverseProxy: {
    url: "https://jsonplaceholder.typicode.com/todos",
    appendPath: true,
    appendQueryString: true,
    replaceRootUrl: true,
  },
  responseContentType: "application/json",
  image: {
    type: ResponseImageType.Png,
    width: 1024,
    height: 256,
    backgroundColor: "#ffa0d3",
    text: "Placeholder text that will be drawn in the image",
    fontSize: 10,
    wordWrap: false,
  } as StubResponseImageModel,
  minHits: 1,
  maxHits: 2,
  exactHits: 3,
  scenarioState: "new-state",
  newScenarioState: "new-state",
  stringReplace: [
    {
      text: "old value",
      ignoreCase: true,
      replaceWith: "New value",
    } as StubResponseReplaceModel,
  ],
  regexReplace: [
    {
      regex: "(ipsum|consectetur)",
      replaceWith: "New value",
    } as StubResponseReplaceModel,
  ],
};
