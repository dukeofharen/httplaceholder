<template>
  <span v-bind:style="{ color: color }">{{method}}</span>
</template>

<script>
export default {
  name: "httpmethod",
  props: ["method"],
  data() {
    return {
      color: "#969696"
    };
  },
  created() {
    if (this.method === "GET") {
      this.color = "#61E061";
    } else if (this.method === "POST") {
      this.color = "#0055FF";
    } else if (this.method === "PUT") {
      this.color = "#F57520";
    } else if (this.method === "DELETE") {
      this.color = "#F5CA20";
    } else if (this.method === "OPTIONS") {
      this.color = "#9540EA";
    } else if (this.method === "PATCH") {
      this.color = "#F520F5";
    }
  }
};
</script>

<style scoped>
</style>