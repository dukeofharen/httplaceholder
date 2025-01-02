<script setup lang="ts">
import { computed, type PropType } from 'vue'
import type { MenuItemModel } from '@/domain/menu-item-model.ts'
import { useRouter } from 'vue-router'
import * as HeroIcons from '@heroicons/vue/24/solid'

const router = useRouter()
const props = defineProps({
  item: {
    type: Object as PropType<MenuItemModel>,
    required: true,
  },
})
const emit = defineEmits(['linkClicked'])

// Computed
const url = computed<string | undefined>(() => {
  if (props.item.url) {
    return props.item.url
  }

  if (props.item.routeName) {
    const href = resolveRoute(props.item.routeName)
    if (href) {
      return href
    }
  }

  return undefined
})

const urlTarget = computed(() => {
  return props.item.targetBlank ? '_blank' : ''
})

const isActive = computed(() => {
  if (!props.item.routeName) {
    return false
  }

  const href = resolveRoute(props.item.routeName)
  return href && location.href.toLowerCase().endsWith(href.toLowerCase())
})

const resolvedIcon = computed(() => {
  if (!props.item.icon) {
    return
  }

  return (HeroIcons as any)[props.item.icon] || undefined
})

// Functions
async function linkClick(event: Event) {
  if (props.item.onClick) {
    event.preventDefault()
    await props.item.onClick()
  }

  emit('linkClicked')
}

function resolveRoute(routeName: string) {
  const resolvedRoute = router.resolve(routeName)
  if (resolvedRoute.name && resolvedRoute.href) {
    return resolvedRoute.href
  }
}
</script>

<template>
  <li>
    <a
      :href="url"
      :target="urlTarget"
      @click="linkClick"
      class="flex cursor-pointer items-center p-2 rounded-lg text-white hover:bg-gray-700 group"
      :class="{ 'bg-gray-700': isActive }"
    >
      <component :is="resolvedIcon" class="size-6" />
      <span class="ms-3">{{ props.item.title }}</span>
    </a>
  </li>
</template>
