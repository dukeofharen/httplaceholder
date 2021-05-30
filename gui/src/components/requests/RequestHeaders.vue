<template>
  <v-expansion-panel>
    <v-expansion-panel-header>
      <strong>Request headers</strong>
    </v-expansion-panel-header>
    <v-expansion-panel-content>
      <v-simple-table dense>
        <template v-slot:default>
          <tbody>
            <tr v-for="(value, key) in requestParameters.headers" :key="key">
              <td class="pl-0">{{ key }}</td>
              <td class="value">{{ value }}</td>
            </tr>
          </tbody>
        </template>
      </v-simple-table>
    </v-expansion-panel-content>
  </v-expansion-panel>
</template>

<script>
export default {
  name: "RequestHeaders",
  props: ["requestParameters"]
};
</script>

<style scoped>
tbody tr:hover {
  background-color: transparent !important;
}

.value {
  opacity: 0.8;
}
</style>
