<script setup lang="ts">
import { translate } from '@/utils/translate.ts'
import { renderDocLink } from '@/utils/doc.ts'
import { useUsersStore } from '@/stores/users.ts'
import { useRouter } from 'vue-router'
import { computed } from 'vue'
import { useMetadataStore } from '@/stores/metadata.ts'
import NavItem from '@/components/nav/NavItem.vue'
import type { MenuItemModel } from '@/domain/menu-item-model.ts'

const userStore = useUsersStore()
const metadataStore = useMetadataStore()
const router = useRouter()

// Data
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
]

// Computed
const menuItems = computed<MenuItemModel[]>(() => {
  const isAuthenticated = userStore.getAuthenticated
  const authEnabled = metadataStore.getAuthenticationEnabled
  return plainMenuItems.filter(
    (i) =>
      (i.onlyShowWhenLoggedInAndAuthEnabled && isAuthenticated && authEnabled) ||
      (i.hideWhenAuthEnabledAndNotLoggedIn && authEnabled && isAuthenticated) ||
      (i.hideWhenAuthEnabledAndNotLoggedIn && !authEnabled) ||
      i.onlyShowWhenLoggedInAndAuthEnabled === false,
  )
})
</script>

<template>
  <button
    data-drawer-target="default-sidebar"
    data-drawer-toggle="default-sidebar"
    aria-controls="default-sidebar"
    type="button"
    class="inline-flex items-center p-2 mt-2 ms-3 text-sm text-gray-500 rounded-lg sm:hidden hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-gray-200 dark:text-gray-400 dark:hover:bg-gray-700 dark:focus:ring-gray-600"
  >
    <span class="sr-only">Open sidebar</span>
    <svg
      class="w-6 h-6"
      aria-hidden="true"
      fill="currentColor"
      viewBox="0 0 20 20"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        clip-rule="evenodd"
        fill-rule="evenodd"
        d="M2 4.75A.75.75 0 012.75 4h14.5a.75.75 0 010 1.5H2.75A.75.75 0 012 4.75zm0 10.5a.75.75 0 01.75-.75h7.5a.75.75 0 010 1.5h-7.5a.75.75 0 01-.75-.75zM2 10a.75.75 0 01.75-.75h14.5a.75.75 0 010 1.5H2.75A.75.75 0 012 10z"
      ></path>
    </svg>
  </button>

  <aside
    id="default-sidebar"
    class="fixed top-0 left-0 z-40 w-64 h-screen transition-transform -translate-x-full sm:translate-x-0"
    aria-label="Sidebar"
  >
    <div class="h-full px-3 py-4 overflow-y-auto bg-gray-50 dark:bg-gray-800">
      <ul class="space-y-2 font-medium">
        <NavItem v-for="item of menuItems" :item="item" :key="item.title" />
      </ul>
    </div>
  </aside>
</template>

<style scoped></style>
