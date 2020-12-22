<template>
  <v-row>
    <v-col>
      <h1>Settings</h1>
      <v-switch v-model="darkTheme" class="ma-2" label="Dark theme"></v-switch>
    </v-col>
  </v-row>
</template>

<script>
export default {
  data() {
    return {
      darkTheme: false
    };
  },
  created() {
    this.darkTheme = this.$store.getters["general/getDarkTheme"];
  },
  watch: {
    darkTheme() {
      this.$store.commit("general/storeDarkTheme", this.darkTheme);
    }
  }
};
</script>

<style scoped></style>
