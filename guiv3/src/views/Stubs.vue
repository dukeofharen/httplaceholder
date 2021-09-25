<template>
  <div>
    <h1>Stubs</h1>

    <div class="accordion" :id="accordionId">
      <Stub
        v-for="stub of filteredStubs"
        :key="stub.stubId"
        :overview-stub="stub"
        :accordion-id="accordionId"
        @deleted="loadData"
      />
    </div>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { computed, onMounted, ref } from "vue";
import Stub from "@/components/stub/Stub";

export default {
  name: "Stubs",
  components: { Stub },
  setup() {
    const store = useStore();

    // Data
    const accordionId = "stubs-accordion";
    const stubs = ref([]);

    // Computed
    const filteredStubs = computed(() => {
      let stubsResult = stubs.value;
      const compare = (a, b) => {
        if (a.stub.id < b.stub.id) return -1;
        if (a.stub.id > b.stub.id) return 1;
        return 0;
      };

      stubsResult.sort(compare);
      return stubsResult;
    });

    // Methods
    const loadStubs = async () => {
      stubs.value = [];
      stubs.value = await store.dispatch("stubs/getStubsOverview");
      console.log(JSON.stringify(stubs.value));
    };
    const loadData = async () => {
      await Promise.all([loadStubs()]);
    };

    // Lifecycle
    onMounted(async () => await loadData());

    return { accordionId, stubs, filteredStubs, loadData };
  },
};
</script>

<style scoped></style>
