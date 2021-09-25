<template>
  <div>{{ stubId }}</div>
</template>

<script>
import { useRoute } from "vue-router";
import { ref } from "vue";

export default {
  name: "StubForm",
  setup() {
    const route = useRoute();

    // Data
    const stubId = ref(route.params.stubId);

    return { stubId };
  },
};
</script>

<style scoped></style>
