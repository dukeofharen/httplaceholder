<script setup lang="ts">
import { translate } from '@/utils/translate.ts'
import { renderDocLink } from '@/utils/doc.ts'
import { useUsersStore } from '@/stores/users.ts'
import { useRouter } from 'vue-router'
import { computed, ref } from 'vue'
import { useMetadataStore } from '@/stores/metadata.ts'
import NavItem from '@/components/nav/NavItem.vue'
import type { MenuItemModel } from '@/domain/ui/menu-item-model.ts'
import { Bars3Icon } from '@heroicons/vue/24/solid'
import { useSettingsStore } from '@/stores/settings.ts'

const userStore = useUsersStore()
const metadataStore = useMetadataStore()
const settingsStore = useSettingsStore()
const router = useRouter()

// Data
const mobileMenuOpen = ref(false)
const plainMenuItems: MenuItemModel[] = [
  {
    title: translate('sidebar.requests'),
    icon: 'EyeIcon',
    routeName: 'Requests',
    hideWhenAuthEnabledAndNotLoggedIn: true,
  },
  {
    title: translate('sidebar.stubs'),
    icon: 'CodeBracketIcon',
    routeName: 'Stubs',
    hideWhenAuthEnabledAndNotLoggedIn: true,
  },
  {
    title: translate('sidebar.addStubs'),
    icon: 'PlusIcon',
    routeName: 'StubForm',
    hideWhenAuthEnabledAndNotLoggedIn: true,
  },
  {
    title: translate('sidebar.importStubs'),
    icon: 'ArrowUpIcon',
    routeName: 'ImportStubs',
    hideWhenAuthEnabledAndNotLoggedIn: true,
  },
  {
    title: translate('sidebar.scenarios'),
    icon: 'ListBulletIcon',
    routeName: 'Scenarios',
    hideWhenAuthEnabledAndNotLoggedIn: true,
  },
  {
    title: translate('sidebar.docs'),
    icon: 'DocumentIcon',
    url: renderDocLink(),
    targetBlank: true,
    onlyShowWhenLoggedInAndAuthEnabled: false,
  },
  {
    title: translate('sidebar.apiDocs'),
    icon: 'DocumentIcon',
    url: '/swagger/index.html',
    targetBlank: true,
    onlyShowWhenLoggedInAndAuthEnabled: false,
  },
  {
    title: translate('sidebar.settings'),
    icon: 'WrenchIcon',
    routeName: 'Settings',
    hideWhenAuthEnabledAndNotLoggedIn: true,
  },
  {
    title: translate('sidebar.logOut'),
    icon: 'ArrowLeftStartOnRectangleIcon',
    onlyShowWhenLoggedInAndAuthEnabled: true,
    onClick: async () => {
      userStore.logOut()
      await router.push({ name: 'Login' })
    },
  },
  {
    title: translate('settings.darkTheme'),
    icon: 'MoonIcon',
    targetBlank: true,
    onlyShowWhenLoggedInAndAuthEnabled: false,
    precondition: () => !settingsStore.getDarkTheme,
    onClick: () => {
      settingsStore.enableDarkTheme()
      return Promise.resolve(true)
    },
  },
  {
    title: translate('settings.lightTheme'),
    icon: 'SunIcon',
    targetBlank: true,
    onlyShowWhenLoggedInAndAuthEnabled: false,
    precondition: () => settingsStore.getDarkTheme,
    onClick: () => {
      settingsStore.disableDarkTheme()
      return Promise.resolve(true)
    },
  },
]

// Computed
const menuItems = computed<MenuItemModel[]>(() => {
  const isAuthenticated = userStore.getAuthenticated
  const authEnabled = metadataStore.getAuthenticationEnabled
  return plainMenuItems.filter(
    (i) =>
      (!i.precondition || i.precondition()) &&
      ((i.onlyShowWhenLoggedInAndAuthEnabled && isAuthenticated && authEnabled) ||
        (i.hideWhenAuthEnabledAndNotLoggedIn && authEnabled && isAuthenticated) ||
        (i.hideWhenAuthEnabledAndNotLoggedIn && !authEnabled) ||
        i.onlyShowWhenLoggedInAndAuthEnabled === false),
  )
})

// Functions
function onLinkClicked() {
  mobileMenuOpen.value = false
}
</script>

<template>
  <div class="bg-white dark:bg-gray-800">
    <button
      type="button"
      @click="mobileMenuOpen = !mobileMenuOpen"
      class="inline-flex items-center p-2 mt-2 ms-3 text-sm rounded-lg sm:hidden focus:outline-hidden focus:ring-2 text-gray-400"
    >
      <span class="sr-only">{{ $translate('general.openSidebar') }}</span>
      <Bars3Icon class="size-6" />
    </button>
  </div>

  <aside
    id="default-sidebar"
    class="fixed top-0 left-0 z-40 w-52 h-screen transition-transform sm:translate-x-0"
    :class="{ '-translate-x-full': !mobileMenuOpen, 'transform-none': mobileMenuOpen }"
    aria-label="Sidebar"
  >
    <div class="h-full px-3 py-4 overflow-y-auto bg-gray-800">
      <a href="https://httplaceholder.org" class="flex items-center mb-5" target="_blank">
        <img src="@/assets/logo-white_small.png" class="h-8 sm:h-10" alt="HttPlaceholder" />
      </a>
      <ul class="space-y-2 font-medium">
        <NavItem
          v-for="item of menuItems"
          :item="item"
          :key="item.title"
          @link-clicked="onLinkClicked"
        />
      </ul>
    </div>
  </aside>
  <div
    v-if="mobileMenuOpen"
    class="bg-gray-900/80 fixed inset-0 z-30 cursor-pointer"
    @click="mobileMenuOpen = false"
  ></div>
</template>

<style scoped></style>
