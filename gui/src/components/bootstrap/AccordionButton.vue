<template>
  <div class="accordion-item">
    <h2 class="accordion-header">
      <button
        class="accordion-button"
        type="button"
      >
        <slot name="button-text"></slot>
      </button>
    </h2>
  </div>
</template>

<style scoped>
.accordion-button::after {
  background-image: none;
}

.accordion-item {
  overflow-x: hidden;
}
</style>
