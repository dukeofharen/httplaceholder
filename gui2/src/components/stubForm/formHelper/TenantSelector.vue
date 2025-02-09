<template>
  <div class="row">
    <div class="col-md-12">
      <strong>{{ $translate("stubForm.insertNewTenantName") }}</strong>
      <input
        type="text"
        class="form-control mt-2"
        v-model="tenant"
        @keyup.enter="tenantSelected(tenant)"
      />
      <button class="btn btn-success mt-2" @click="tenantSelected(tenant)">
        {{ $translate("general.add") }}
      </button>
    </div>
    <div class="col-md-12 mt-3" v-if="tenantNames.length">
      <strong>{{ $translate("stubForm.selectExistingTenant") }}</strong>
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

<script lang="ts">
import { onMounted, ref } from "vue";
import { handleHttpError } from "@/utils/error";
import { useTenantsStore } from "@/store/tenants";
import { useStubFormStore } from "@/store/stubForm";
import { defineComponent } from "vue";

export default defineComponent({
  name: "TenantSelector",
  setup() {
    const tenantStore = useTenantsStore();
    const stubFormStore = useStubFormStore();

    // Data
    const tenantNames = ref<string[]>([]);
    const tenant = ref("");

    // Methods
    const tenantSelected = (tenant: string) => {
      stubFormStore.setTenant(tenant);
      stubFormStore.closeFormHelper();
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
});
</script>

<style scoped></style>
