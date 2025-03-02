<template>
  <div class="loading" v-if="showLoading">
    <div class="lds-ring">
      <div></div>
      <div></div>
      <div></div>
      <div></div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent } from 'vue'
import { useHttpStore } from '@/store/http'
import { useGeneralStore } from '@/store/general'

export default defineComponent({
  name: 'Loading',
  setup() {
    const httpStore = useHttpStore()
    const generalStore = useGeneralStore()

    // Computed
    const showLoading = computed(
      () => generalStore.shouldShowLoader || httpStore.isExecutingHttpCalls,
    )

    return { showLoading }
  },
})
</script>

<style scoped>
.light-theme .loading {
  background: rgba(0, 0, 0, 0.5);
}

.dark-theme .loading {
  background: rgba(255, 255, 255, 0.5);
}

.loading {
  position: fixed;
  padding: 0;
  margin: 0;

  top: 0;
  left: 0;

  width: 100%;
  height: 100%;
  z-index: 99999;
}

/*Loading icon with inspiration from https://loading.io*/
.lds-ring {
  display: inline-block;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 80px;
  height: 80px;
}

.lds-ring div {
  box-sizing: border-box;
  display: block;
  position: absolute;
  width: 64px;
  height: 64px;
  margin: 8px;
  border: 8px solid #fff;
  border-radius: 50%;
  animation: lds-ring 1.2s cubic-bezier(0.5, 0, 0.5, 1) infinite;
  border-color: #fff transparent transparent transparent;
}

.lds-ring div:nth-child(1) {
  animation-delay: -0.45s;
}

.lds-ring div:nth-child(2) {
  animation-delay: -0.3s;
}

.lds-ring div:nth-child(3) {
  animation-delay: -0.15s;
}

@keyframes lds-ring {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>
