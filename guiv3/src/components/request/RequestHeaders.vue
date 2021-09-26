<template>
  <accordion-item>
    <template v-slot:button-text>Request headers</template>
    <template v-slot:accordion-body>
      <table class="table" v-if="requestParams.headers">
        <tbody>
          <tr v-for="(value, key) in requestParams.headers" :key="key">
            <td class="p-1">{{ key }}</td>
            <td class="p-1">{{ value }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </accordion-item>
</template>

<script>
import { computed } from "vue";
import AccordionItem from "@/components/bootstrap/AccordionItem";

export default {
  name: "RequestHeaders",
  components: { AccordionItem },
  props: {
    request: {
      type: Object,
      required: true,
    },
  },
  setup(props) {
    // Computed
    const requestParams = computed(
      () => props.request?.requestParameters || {}
    );

    return { requestParams };
  },
};
</script>

<style scoped></style>
