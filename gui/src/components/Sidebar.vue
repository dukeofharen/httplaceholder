<template>
  <div class="col-2 col-sm-3 col-md-2 col-xl-2 col-lg-2 px-md-2 px-0 bg-dark">
    <div class="d-flex flex-column flex-shrink-0 min-vh-100">
      <a
        href="https://httplaceholder.org"
        target="_blank"
        class="d-flex align-items-center pb-3 mb-md-0 me-md-auto text-white text-decoration-none justify-content-center mt-1"
      >
        <span class="fs-5 d-none d-md-inline logo"
          ><img src="@/assets/logo-white_small.png" alt=""
        /></span>
        <span class="fs-5 d-inline d-md-none logo"
          ><img src="@/assets/logo-white_small_square.png" alt=""
        /></span>
      </a>
      <div class="list-group list-group-flush">
        <SidebarMenuItem v-for="item of menuItems" :key="item.title" :item="item" />
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import SidebarMenuItem from '@/components/SidebarMenuItem.vue'
import { computed, defineComponent } from 'vue'
import router from '@/router'
import { useUsersStore } from '@/store/users'
import { useMetadataStore } from '@/store/metadata'
import { renderDocLink } from '@/utils/doc'
import { translate } from '@/utils/translate'

export default defineComponent({
  name: 'Sidebar',
  components: { SidebarMenuItem },
  setup() {
    const userStore = useUsersStore()
    const metadataStore = useMetadataStore()

    // Data
    const plainMenuItems = [
      {
        title: translate('sidebar.requests'),
        icon: 'eye',
        routeName: 'Requests',
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: translate('sidebar.stubs'),
        icon: 'code-slash',
        routeName: 'Stubs',
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: translate('sidebar.addStubs'),
        icon: 'plus',
        routeName: 'StubForm',
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: translate('sidebar.importStubs'),
        icon: 'arrow-up-short',
        routeName: 'ImportStubs',
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: translate('sidebar.scenarios'),
        icon: 'card-list',
        routeName: 'Scenarios',
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: translate('sidebar.docs'),
        icon: 'file-earmark-text',
        url: renderDocLink(),
        targetBlank: true,
        onlyShowWhenLoggedInAndAuthEnabled: false,
      },
      {
        title: translate('sidebar.apiDocs'),
        icon: 'file-earmark-text',
        url: '/swagger/index.html',
        targetBlank: true,
        onlyShowWhenLoggedInAndAuthEnabled: false,
      },
      {
        title: translate('sidebar.settings'),
        icon: 'wrench',
        routeName: 'Settings',
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: translate('sidebar.logOut'),
        icon: 'box-arrow-left',
        onlyShowWhenLoggedInAndAuthEnabled: true,
        onClick: async () => {
          userStore.logOut()
          await router.push({ name: 'Login' })
        },
      },
    ]

    // Computed
    const menuItems = computed(() => {
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

    return {
      menuItems: menuItems,
    }
  },
})
</script>

<style lang="scss" scoped>
@import '@/style/bootstrap';

.logo img {
  max-width: 100%;
}
</style>
