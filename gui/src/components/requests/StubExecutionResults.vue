<template>
  <v-expansion-panel>
    <v-expansion-panel-header>
      <strong>Stub execution results</strong>
    </v-expansion-panel-header>
    <v-expansion-panel-content>
      <v-expansion-panels>
        <v-expansion-panel
          v-for="(result, key) in orderedStubExecutionResults"
          :key="key"
        >
          <v-expansion-panel-header>
            <strong>
              <span>{{ result.stubId }}</span>
              <span>&nbsp;</span>
              <span>(</span>
              <span>
                <Bool
                  v-bind:bool="result.passed"
                  trueText="passed"
                  falseText="not passed"
                />
              </span>
              <span>)</span>
            </strong>
          </v-expansion-panel-header>
          <v-expansion-panel-content v-if="result.conditions.length > 0">
            <div v-for="(condition, key) in result.conditions" :key="key">
              <v-list-item>
                <v-list-item-content>
                  <v-list-item-title
                    ><strong>{{
                      condition.checkerName
                    }}</strong></v-list-item-title
                  >
                  <v-list-item-subtitle>
                    <Bool
                      v-bind:bool="
                        condition.conditionValidation ===
                          conditionValidationType.Valid
                      "
                      trueText="passed"
                      falseText="not passed"
                    />
                  </v-list-item-subtitle>
                </v-list-item-content>
              </v-list-item>
              <v-list-item v-if="condition.log">
                <v-list-item-content>
                  <v-list-item-subtitle
                    >{{ condition.log }}
                  </v-list-item-subtitle>
                </v-list-item-content>
              </v-list-item>
              <v-divider></v-divider>
            </div>
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-expansion-panel-content>
  </v-expansion-panel>
</template>

<script>
import Bool from "@/components/requests/Bool";
import { conditionValidationType } from "@/shared/resources";

export default {
  name: "StubExecutionResults",
  props: ["request"],
  components: {
    Bool
  },
  data() {
    return {
      conditionValidationType
    };
  },
  computed: {
    orderedStubExecutionResults() {
      const compare = a => {
        if (a.passed) return -1;
        if (!a.passed) return 1;
        return 0;
      };
      const results = this.request.stubExecutionResults;
      results.sort(compare);
      return results;
    }
  }
};
</script>

<style scoped></style>
