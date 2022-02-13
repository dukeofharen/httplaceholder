<template>
  <div class="row">
    <div class="col-md-12">
      <strong>Insert new tenant name</strong>
      <input
        type="text"
        class="form-control mt-2"
        v-model="tenant"
        @keyup.enter="tenantSelected(tenant)"
      />
      <button class="btn btn-success mt-2" @click="tenantSelected(tenant)">
        Add
      </button>
    </div>
    <div class="col-md-12 mt-3" v-if="tenantNames.length">
      <strong>Select existing tenant</strong>
      <div class="list-group mt-2">
        <button
          v-for="(tenant, index) of tenantNames"
          :key="index"
          class="list-group-item list-group-item-action fw-bold"
          @click="tenantSelected(tenant)"
        >
          {{ tenant }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { onMounted, ref } from "vue";
import { handleHttpError } from "@/utils/error";
import { useTenantsStore } from "@/store/tenants";

export default {
  name: "TenantSelector",
  setup() {
    const store = useStore();
    const tenantStore = useTenantsStore();

    // Data
    const tenantNames = ref([]);
    const tenant = ref("");

    // Methods
    const tenantSelected = (tenant) => {
      store.commit("stubForm/setTenant", tenant);
      store.commit("stubForm/closeFormHelper");
    };

    // Lifecycle
    onMounted(async () => {
      try {
        tenantNames.value = await tenantStore.getTenantNames();
      } catch (e) {
        handleHttpError(e);
      }
    });

    return { tenantNames, tenantSelected, tenant };
  },
};
</script>

<style scoped></style>
