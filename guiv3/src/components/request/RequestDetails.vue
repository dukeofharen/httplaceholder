<template>
  <div>{{ request }}</div>
</template>

<script>
export default {
  name: "RequestDetails",
  props: {
    request: {
      type: Object,
      required: true,
    },
  },
};
</script>

<style scoped></style>
