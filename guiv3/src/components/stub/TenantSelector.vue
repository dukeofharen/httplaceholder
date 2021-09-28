<template>
  <div class="list-group">
    <button
      v-for="(tenant, index) of tenantNames"
      :key="index"
      class="list-group-item list-group-item-action fw-bold"
      @click="tenantSelected(tenant)"
    >
      {{ tenant }}
    </button>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { onMounted, ref } from "vue";

export default {
  name: "TenantSelector",
  setup() {
    const store = useStore();

    // Data
    const tenantNames = ref([]);

    // Methods
    const tenantSelected = (tenant) => {
      store.commit("stubForm/setTenant", tenant);
      store.commit("stubForm/closeFormHelper");
    };

    // Lifecycle
    onMounted(async () => {
      const tenantNamesResult = await store.dispatch("tenants/getTenantNames");
      tenantNamesResult.unshift("Default tenant");
      tenantNames.value = tenantNamesResult;
    });

    return { tenantNames, tenantSelected };
  },
};
</script>

<style scoped></style>
