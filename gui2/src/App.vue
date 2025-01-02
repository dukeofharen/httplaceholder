<script setup lang="ts">
import { RouterView } from 'vue-router'
import NavMenu from '@/components/nav/NavMenu.vue'
import { useDarkTheme } from '@/composables/useDarkTheme'
import { useApplicationVersion } from '@/composables/useApplicationVersion.ts'
import { useLoginGuard } from '@/composables/useLoginGuard.ts'

useDarkTheme()
useApplicationVersion()
useLoginGuard()
</script>

<template>
  <NavMenu />
  <div
    class="p-4 sm:ml-52 bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-200 min-h-screen overflow-x-auto"
  >
    <RouterView />
  </div>
</template>

<style scoped></style>
