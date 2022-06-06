import type { StubUrlConditionModel } from "@/domain/stub/stub-url-condition-model";
import type { StubFormModel } from "@/domain/stub/stub-form-model";
import type { HashMap } from "@/domain/hash-map";
import type { StubXpathModel } from "@/domain/stub/stub-xpath-model";
import type { StubBasicAuthenticationModel } from "@/domain/stub/stub-basic-authentication-model";
import type { StubConditionsScenarioModel } from "@/domain/stub/stub-conditions-scenario-model";

export interface StubConditionsModel {
  method?: string;
  url?: StubUrlConditionModel;
  body?: any[];
  form?: StubFormModel[];
  headers?: any;
  xpath?: StubXpathModel[];
  jsonPath?: any[];
  basicAuthentication?: StubBasicAuthenticationModel;
  clientIp?: string;
  host?: string;
  json?: any;
  scenario?: StubConditionsScenarioModel;
}
