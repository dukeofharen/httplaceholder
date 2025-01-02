<script setup lang="ts">
import { RouterView } from 'vue-router'
import NavMenu from '@/components/nav/NavMenu.vue'
import { useDarkTheme } from '@/composables/useDarkTheme.ts'

useDarkTheme()
</script>

<template>
  <NavMenu />
  <div class="p-4 sm:ml-52">
    <RouterView />
  </div>
</template>

<style scoped></style>
