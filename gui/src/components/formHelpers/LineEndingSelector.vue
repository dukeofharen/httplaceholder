<template>
  <v-row>
    <v-col>
      <v-list>
        <v-list-item>
          <v-list-item-content
            class="line-endings"
            @click="lineEndingSelected(null)"
          >
            <v-list-item-title>As provided in response body</v-list-item-title>
          </v-list-item-content>
        </v-list-item>

        <v-list-item>
          <v-list-item-content
            class="line-endings"
            @click="lineEndingSelected(lineEndingTypes.unix)"
          >
            <v-list-item-title>UNIX line endings</v-list-item-title>
          </v-list-item-content>
        </v-list-item>

        <v-list-item>
          <v-list-item-content
            class="line-endings"
            @click="lineEndingSelected(lineEndingTypes.windows)"
          >
            <v-list-item-title>Windows line endings</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-col>
  </v-row>
</template>

<script>
import { lineEndingTypes } from "@/shared/stubFormResources";

export default {
  data() {
    return {
      lineEndingTypes
    };
  },
  methods: {
    lineEndingSelected(value) {
      this.$store.commit("stubForm/setLineEndings", value);
      this.$store.commit("stubForm/closeFormHelper");
    }
  }
};
</script>

<style scoped>
.line-endings {
  font-weight: bold;
  cursor: pointer;
  padding-left: 20px;
}

.line-endings:hover {
  background-color: #f1f1f1;
}
.theme--dark .line-endings:hover {
  background-color: #666666;
}
</style>
