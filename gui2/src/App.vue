<script setup lang="ts">
import { RouterView } from 'vue-router'
import Navigation from '@/components/Navigation.vue'
</script>

<template>
  <Navigation />
  <div class="p-4 sm:ml-64">
    <RouterView />
  </div>
</template>

<style scoped></style>
