<template>
  <span v-bind:class="className"
    ><strong>{{ method }}</strong></span
  >
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
    let className = "";
    switch (this.method) {
      case "GET":
        className = "method-get";
        break;
      case "POST":
        className = "method-post";
        break;
      case "PUT":
        className = "method-put";
        break;
      case "DELETE":
        className = "method-delete";
        break;
      case "OPTIONS":
        className = "method-options";
        break;
      case "PATCH":
        className = "method-patch";
        break;
    }

    this.className = className;
  }
};
</script>

<style scoped>
.method-get {
  color: #006909;
}

.method-post {
  color: #0006a1;
}

.method-put {
  color: #894e00;
}

.method-delete {
  color: #9e981a;
}

.method-options {
  color: #5e0097;
}

.method-patch {
  color: #8d0c95;
}
</style>
