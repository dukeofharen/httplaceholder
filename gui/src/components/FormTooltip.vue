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
    <span v-html="tooltipText" />
  </v-tooltip>
</template>

<script>
  import {tooltipResources} from "@/shared/stubFormResources";
  export default {
    props: ["tooltipKey"],
    data() {
      return {
        tooltipResources
      }
    },
    computed: {
      tooltipText() {
        return tooltipResources[this.tooltipKey].replace(/(?:\r\n|\r|\n)/g, '</br>');
      }
    }
  }
</script>
