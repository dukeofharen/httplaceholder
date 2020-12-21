<template>
  <v-row>
    <v-col>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Select an HTTP method or fill in a custom HTTP method in the YAML below.
          </v-list-item-title>
        </v-list-item-content>
      </v-list-item>

      <v-list-item v-for="(method, index) of httpMethods" :key="index">
        <v-list-item-content>
          <v-list-item-title class="method" @click="methodSelected(method)">{{ method }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </v-col>
  </v-row>
</template>

<script>
import {httpMethods} from "@/shared/stubFormResources";

export default {
  data() {
    return {
      httpMethods
    };
  },
  methods: {
    methodSelected(method) {
      this.$store.commit("stubForm/setMethod", method);
      this.$store.commit("stubForm/closeFormHelper");
    }
  }
};
</script>

<style scoped>
.method {
  font-weight: bold;
  cursor: pointer;
}
</style>
