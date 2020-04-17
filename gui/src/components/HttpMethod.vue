<template>
  <span v-bind:class="className">{{ method }}</span>
</template>

<script>
export default {
  name: "httpmethod",
  props: ["method"],
  data() {
    return {
      className: ""
    };
  },
  created() {
    if (this.method === "GET") {
      this.className = "green";
    } else if (this.method === "POST") {
      this.className = "blue";
    } else if (this.method === "PUT") {
      this.className = "orange";
    } else if (this.method === "DELETE") {
      this.className = "yellow";
    } else if (this.method === "OPTIONS") {
      this.className = "purple";
    } else if (this.method === "PATCH") {
      this.className = "pink";
    }
  }
};
</script>

<style scoped></style>
