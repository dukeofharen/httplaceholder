<template>
  <accordion-item>
    <template v-slot:button-text>Response writer results</template>
    <template v-slot:accordion-body>
      <ul class="list-group">
        <li
          v-for="result of stubResponseWriterResults"
          :key="result.responseWriterName"
          class="list-group-item"
        >
          <label class="fw-bold">{{ result.responseWriterName }}</label>
          <div v-if="result.log" class="text-secondary">{{ result.log }}</div>
        </li>
      </ul>
    </template>
  </accordion-item>
</template>

<script>
export default {
  name: "ResponseWriterResults",
  props: {
    correlationId: {
      type: String,
      required: true,
    },
    stubResponseWriterResults: {
      type: Array,
      required: true,
    },
  },
};
</script>

<style scoped>
label {
  display: block;
}
</style>
