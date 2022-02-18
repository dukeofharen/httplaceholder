import { defineStore } from "pinia";
import { del, get, put } from "@/utils/api";

export const useScenariosStore = defineStore({
  id: "scenarios",
  state: () => ({}),
  getters: {},
  actions: {
    getAllScenarios() {
      return get("/ph-api/scenarios")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    getScenario(scenario) {
      return get(`/ph-api/scenarios/${scenario}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    setScenario(scenario) {
      return put(`/ph-api/scenarios/${scenario.scenario}`, scenario)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    deleteAllScenarios() {
      return del("/ph-api/scenarios")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    deleteScenario(scenario) {
      return del(`/ph-api/scenarios/${scenario}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
