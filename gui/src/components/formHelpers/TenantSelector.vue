<template>
  <v-row>
    <v-col>
      <v-list-item>
        <v-list-item-content class="description">
          <v-list-item-title
            >Select a tenant below. After selecting a tenant, you can change it
            in the YAML form below.
          </v-list-item-title>
        </v-list-item-content>
      </v-list-item>

      <v-list-item v-for="(tenant, index) of tenantNames" :key="index">
        <v-list-item-content class="tenant" @click="tenantSelected(tenant)">
          <v-list-item-title>{{ tenant }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </v-col>
  </v-row>
</template>

<script>
export default {
  async mounted() {
    let tenantNames = await this.$store.dispatch("tenants/getTenantNames");
    tenantNames.unshift("Default tenant");
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
.description {
  padding-left: 20px;
}

.tenant {
  font-weight: bold;
  cursor: pointer;
  padding-left: 20px;
}

.tenant:hover {
  background-color: #f1f1f1;
}
</style>
