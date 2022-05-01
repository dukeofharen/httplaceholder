import { defineStore } from "pinia";
import { del, get, put } from "@/utils/api";
import type { ScenarioModel } from "@/domain/scenario/scenario-model";
import type { ScenarioStateRequestModel } from "@/domain/scenario/scenario-state-request-model";

export interface ScenarioInputModel {
  scenario: string;
  state: string;
  hitCount: number | undefined;
}

export const useScenariosStore = defineStore({
  id: "scenarios",
  state: () => ({}),
  getters: {},
  actions: {
    getAllScenarios(): Promise<ScenarioModel[]> {
      return get("/ph-api/scenarios")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    getScenario(scenario: string): Promise<ScenarioModel> {
      return get(`/ph-api/scenarios/${scenario}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    setScenario(scenario: ScenarioInputModel): Promise<any> {
      return put(`/ph-api/scenarios/${scenario.scenario}`, {
        hitCount: scenario.hitCount,
        state: scenario.state,
      } as ScenarioStateRequestModel)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    deleteAllScenarios(): Promise<any> {
      return del("/ph-api/scenarios")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    deleteScenario(scenario: string): Promise<any> {
      return del(`/ph-api/scenarios/${scenario}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
