<template>
  <v-app-bar app clipped-left color="blue-grey darken-4">
    <v-app-bar-nav-icon
      @click="flipDrawerIsOpen"
      color="white"
    ></v-app-bar-nav-icon>
    <span class="title ml-3 mr-5"
      ><img src="@/img/logo-white.png" alt=""
    /></span>
    <div class="flex-grow-1"></div>
  </v-app-bar>
</template>

<script>
export default {
  name: "MenuBar",
  methods: {
    flipDrawerIsOpen() {
      this.$store.commit("general/flipDrawerIsOpen");
    }
  }
};
</script>

<style scoped>
.title img {
  position: absolute;
  top: 8px;
  height: 70%;
}
</style>
