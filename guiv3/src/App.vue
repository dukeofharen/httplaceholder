<template>
  <NavbarTop />
  <div class="container-fluid">
    <router-view />
  </div>
</template>

<script>
import NavbarTop from "@/components/NavbarTop";

export default {
  components: { NavbarTop },
};
</script>

<style lang="scss">
body {
  margin: 0;
  padding: 0;
}
</style>
