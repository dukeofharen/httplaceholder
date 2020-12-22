<template>
  <v-row>
    <v-col>
      <v-list-item>
        <v-list-item-content class="description">
          <v-list-item-title
            >Select an HTTP status code from the list below.</v-list-item-title
          >
        </v-list-item-content>
      </v-list-item>

      <v-list-item v-for="code of httpStatusCodes" :key="code.code">
        <v-list-item-content
          class="status-code"
          @click="codeSelected(code.code)"
        >
          <v-list-item-title
            >{{ code.name }} - {{ code.code }}</v-list-item-title
          >
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
.description {
  padding-left: 20px;
}

.status-code {
  font-weight: bold;
  cursor: pointer;
  padding-left: 20px;
}

.status-code:hover {
  background-color: #f1f1f1;
}
</style>
