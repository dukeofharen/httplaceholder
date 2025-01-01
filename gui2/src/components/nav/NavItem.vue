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
      class="flex cursor-pointer items-center p-2 text-gray-900 rounded-lg dark:text-white hover:bg-gray-100 dark:hover:bg-gray-700 group"
      :class="{ 'bg-gray-100': isActive, 'dark:bg-gray-700': isActive }"
    >
      <component :is="resolvedIcon" class="size-6" />
      <!--      <svg-->
      <!--        class="w-5 h-5 text-gray-500 transition duration-75 dark:text-gray-400 group-hover:text-gray-900 dark:group-hover:text-white"-->
      <!--        aria-hidden="true"-->
      <!--        xmlns="http://www.w3.org/2000/svg"-->
      <!--        fill="currentColor"-->
      <!--        viewBox="0 0 22 21"-->
      <!--      >-->
      <!--        <path-->
      <!--          d="M16.975 11H10V4.025a1 1 0 0 0-1.066-.998 8.5 8.5 0 1 0 9.039 9.039.999.999 0 0 0-1-1.066h.002Z"-->
      <!--        />-->
      <!--        <path-->
      <!--          d="M12.5 0c-.157 0-.311.01-.565.027A1 1 0 0 0 11 1.02V10h8.975a1 1 0 0 0 1-.935c.013-.188.028-.374.028-.565A8.51 8.51 0 0 0 12.5 0Z"-->
      <!--        />-->
      <!--      </svg>-->
      <span class="ms-3">{{ props.item.title }}</span>
    </a>
  </li>
</template>
