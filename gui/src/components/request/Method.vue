<template>
  <span class="method" :class="className">{{ method }}</span>
</template>

<script>
import { computed } from "vue";

export default {
  name: "Method",
  props: {
    method: {
      type: String,
      required: true,
    },
  },
  setup(props) {
    const className = computed(() => {
      switch (props.method) {
        case "GET":
          return "method-get";
        case "POST":
          return "method-post";
        case "PUT":
          return "method-put";
        case "DELETE":
          return "method-delete";
        case "OPTIONS":
          return "method-options";
        case "PATCH":
          return "method-patch";
      }

      return "";
    });

    return { className };
  },
};
</script>

<style lang="scss" scoped>
.method {
  font-weight: bold;
}

.method-get {
  color: #006909;
}

.method-post {
  color: #5c61ff;
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
