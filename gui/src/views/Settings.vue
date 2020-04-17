<template>
  <v-row>
    <v-col>
      <h1>Settings</h1>
      <v-switch v-model="darkTheme" class="ma-2" label="Dark theme"></v-switch>
    </v-col>
  </v-row>
</template>

<script>
import {mutationNames} from "@/store/storeConstants";

export default {
  data() {
    return {
      darkTheme: false
    };
  },
  created() {
    this.darkTheme = this.$store.getters.getDarkTheme;
  },
  watch: {
    darkTheme() {
      this.$store.commit(mutationNames.storeDarkTheme, this.darkTheme);
    }
  }
};
</script>

<style scoped></style>
