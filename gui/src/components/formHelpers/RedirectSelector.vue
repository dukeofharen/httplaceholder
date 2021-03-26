<template>
  <v-row>
    <v-col>
      <v-list>
        <v-list-item>
          <v-list-item-content class="redirect" @click="tempRedirect">
            <v-list-item-title>Temporary redirect</v-list-item-title>
          </v-list-item-content>
        </v-list-item>

        <v-list-item>
          <v-list-item-content class="redirect" @click="permanentRedirect">
            <v-list-item-title>Permanent redirect</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-col>
  </v-row>
</template>

<script>
export default {
  methods: {
    tempRedirect() {
      this.$store.commit("stubForm/setDefaultTempRedirect");
      this.$store.commit("stubForm/closeFormHelper");
    },
    permanentRedirect() {
      this.$store.commit("stubForm/setDefaultPermanentRedirect");
      this.$store.commit("stubForm/closeFormHelper");
    }
  }
};
</script>

<style scoped>
.redirect {
  font-weight: bold;
  cursor: pointer;
  padding-left: 20px;
}

.redirect:hover {
  background-color: #f1f1f1;
}
</style>
