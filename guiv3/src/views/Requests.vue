<template>
  <div>{{ requests }}</div>
</template>

<script>
import { useStore } from "vuex";
import { onMounted, ref } from "vue";

export default {
  name: "Requests",
  setup() {
    const store = useStore();

    // Data
    const requests = ref([]);

    onMounted(async () => {
      requests.value = await store.dispatch("requests/getRequestsOverview");
    });

    return { requests };
  },
};
</script>

<style scoped></style>
