<template>
  <div class="accordion" id="accordion">
    <slot></slot>
  </div>
</template>

<script>
import { defineComponent } from "vue";

export default defineComponent({
  name: "Accordion",
});
</script>

<style scoped></style>
