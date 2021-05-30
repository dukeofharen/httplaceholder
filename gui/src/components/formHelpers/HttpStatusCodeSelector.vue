<template>
  <v-row>
    <v-col>
      <v-row>
        <v-col class="ml-4">
          Select an HTTP status code from the list below.
        </v-col>
      </v-row>

      <v-list-item v-for="(code, index) of httpStatusCodes" :key="index">
        <v-list-item-content
          v-if="code.code"
          class="status-code"
          @click="codeSelected(code.code)"
        >
          <v-list-item-title class="pl-4"
            >{{ code.name }} - {{ code.code }}
          </v-list-item-title>
        </v-list-item-content>
        <v-list-item-content v-else>
          <v-list-item-title class="pl-4">{{ code.name }} </v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </v-col>
  </v-row>
</template>

<script>
import { httpStatusCodes } from "@/shared/stubFormResources";

export default {
  data() {
    return {
      httpStatusCodes
    };
  },
  methods: {
    codeSelected(code) {
      this.$store.commit("stubForm/setStatusCode", code);
      this.$store.commit("stubForm/closeFormHelper");
    }
  }
};
</script>

<style scoped>
.status-code {
  font-weight: bold;
  cursor: pointer;
}

.status-code:hover {
  background-color: #f1f1f1;
}
</style>
