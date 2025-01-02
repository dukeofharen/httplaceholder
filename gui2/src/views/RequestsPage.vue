<script setup lang="ts">
import H1Tag from '@/components/html-elements/H1Tag.vue'
import ButtonComponent from '@/components/html-elements/ButtonComponent.vue'
import { ArrowPathIcon } from '@heroicons/vue/24/outline'

// Functions
async function refresh() {
  alert('refresh')
}
</script>

<template>
  <H1Tag>{{ $translate('general.requests') }}</H1Tag>
  <div class="flex justify-start gap-2 flex-wrap">
    <ButtonComponent type="success" @click="refresh" :dense="true">
      <ArrowPathIcon class="size-6" />
      <span>{{ $translate('general.refresh') }}</span>
    </ButtonComponent>
  </div>
</template>

<style scoped></style>
