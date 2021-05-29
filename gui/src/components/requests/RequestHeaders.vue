<template>
  <v-simple-table dense>
    <template v-slot:default>
      <tbody>
        <tr v-for="(value, key) in requestParameters.headers">
          <td>{{ key }}</td>
          <td>{{ value }}</td>
        </tr>
      </tbody>
    </template>
  </v-simple-table>
</template>

<script>
export default {
  name: "RequestHeaders",
  props: ["requestParameters"]
};
</script>

<style scoped></style>
