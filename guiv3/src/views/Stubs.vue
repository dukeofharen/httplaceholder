<template>
  <div>
    <h1>Stubs</h1>

    <div class="accordion" :id="accordionId">
      <Stub
        v-for="stub of stubs"
        :key="stub.stubId"
        :overview-stub="stub"
        :accordion-id="accordionId"
      />
    </div>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { onMounted, ref } from "vue";
import Stub from "@/components/stub/Stub";

export default {
  name: "Stubs",
  components: { Stub },
  setup() {
    const store = useStore();

    // Data
    const accordionId = "stubs-accordion";
    const stubs = ref([]);

    // Methods
    const loadStubs = async () => {
      stubs.value = await store.dispatch("stubs/getStubsOverview");
    };

    // Lifecycle
    onMounted(async () => await loadStubs());

    return { accordionId, stubs };
  },
};
</script>

<style scoped></style>
