<template>
  <v-row>
    <v-col>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Select a tenant below. After selecting a tenant, you can change it in the YAML form below.
          </v-list-item-title>
        </v-list-item-content>
      </v-list-item>

      <v-list-item v-for="(tenant, index) of tenantNames" :key="index">
        <v-list-item-content>
          <v-list-item-title class="method" @click="tenantSelected(tenant)">{{ tenant }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </v-col>
  </v-row>
</template>

<script>
export default {
  async mounted() {
    let tenantNames = await this.$store.dispatch("tenants/getTenantNames");
    tenantNames.unshift("Default tenant")
    this.tenantNames = tenantNames;
  },
  data() {
    return {
      tenantNames: []
    };
  },
  methods: {
    tenantSelected(tenant) {
      this.$store.commit("stubForm/setTenant", tenant);
      this.$store.commit("stubForm/closeFormHelper");
    }
  }
};
</script>

<style scoped>
.method {
  font-weight: bold;
  cursor: pointer;
}
</style>
