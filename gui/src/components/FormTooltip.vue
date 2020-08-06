<template>
  <v-tooltip bottom max-width="550px">
    <template v-slot:activator="{ on, attrs }">
      <v-icon
        color="primary"
        class="pa-2"
        dark
        v-bind="attrs"
        v-on="on"
      >mdi-help
      </v-icon>
    </template>
    <span v-html="tooltipResources[tooltipKey]" />
  </v-tooltip>
</template>

<script>
  import {tooltipResources} from "@/shared/resources";
  export default {
    props: ["tooltipKey"],
    data() {
      return {
        tooltipResources
      }
    }
  }
</script>
