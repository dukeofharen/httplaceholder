<template>
  <v-row>
    <v-col>
      <v-row>
        <v-col class="ml-4">
          Select an HTTP method or fill in a custom HTTP method in the YAML
          below.
        </v-col>
      </v-row>
      <v-list>
        <v-list-item v-for="(method, index) of httpMethods" :key="index">
          <v-list-item-content class="method" @click="methodSelected(method)">
            <v-list-item-title class="pl-4">{{ method }}</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
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

.method:hover {
  background-color: #f1f1f1;
}
.theme--dark .method:hover {
  background-color: #666666;
}
</style>
