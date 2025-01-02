<script setup lang="ts">
import H1Tag from '@/components/html-elements/H1Tag.vue'
import ButtonComponent from '@/components/html-elements/ButtonComponent.vue'
</script>

<template>
  <H1Tag>{{ $translate('general.requests') }}</H1Tag>
  <div class="flex justify-start gap-2">
    <ButtonComponent type="default" :outline="false">Moi eem</ButtonComponent>
  </div>
</template>

<style scoped></style>
