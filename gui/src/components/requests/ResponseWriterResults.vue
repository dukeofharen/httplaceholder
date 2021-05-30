<template>
  <v-expansion-panel>
    <v-expansion-panel-header>
      <strong>Response writer results</strong>
    </v-expansion-panel-header>
    <v-expansion-panel-content>
      <v-list-item
        v-for="(result, key) in orderedStubResponseWriterResults"
        :key="key"
      >
        <v-list-item-content>
          <v-list-item-title
            >{{ result.responseWriterName }}
          </v-list-item-title>
          <v-list-item-subtitle>
            <Bool
              v-bind:bool="result.executed"
              trueText="executed"
              falseText="not executed"
            />
            <br />
            <span v-if="result.log">{{ result.log }}</span>
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
    </v-expansion-panel-content>
  </v-expansion-panel>
</template>

<script>
import Bool from "@/components/requests/Bool";

export default {
  name: "ResponseWriterResults",
  props: ["request"],
  components: {
    Bool
  },
  computed: {
    orderedStubResponseWriterResults() {
      const compare = a => {
        if (a.executed) return -1;
        if (!a.executed) return 1;
        return 0;
      };
      const results = this.request.stubResponseWriterResults;
      results.sort(compare);
      return results;
    }
  }
};
</script>

<style scoped></style>
