<template>
  <div>
    <h1>Requests</h1>
    <div class="accordion" :id="accordionId">
      <Request
        v-for="request of requests"
        :key="request.correlationId"
        :overview-request="request"
        :accordion-id="accordionId"
      />
    </div>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { onMounted, ref } from "vue";
import Request from "@/components/request/Request";

export default {
  name: "Requests",
  components: { Request },
  setup() {
    const store = useStore();

    // Data
    const accordionId = "requests-accordion";
    const requests = ref([]);

    onMounted(async () => {
      requests.value = await store.dispatch("requests/getRequestsOverview");
    });

    return { requests, accordionId };
  },
};
</script>

<style scoped></style>
