import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import Components from 'unplugin-vue-components/vite'
import { ComponentResolver } from 'unplugin-vue-components'

// https://vite.dev/config/

function CustomComponentResolver(): ComponentResolver {
  return {
    type: 'component',
    resolve: (name: string) => {
      if (name === 'Vue3SlideUpDown') {
        return {
          name: 'Vue3SlideUpDown',
          from: 'vue3-slide-up-down',
        }
      }
    },
  }
}

export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
    Components({
      resolvers: [CustomComponentResolver()],
      dts: './components.d.ts'
    }),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    proxy: {
      '/ph-api': {
        target: 'http://localhost:5000',
      },
      '/swagger': {
        target: 'http://localhost:5000',
      },
      '/requestHub': {
        target: 'http://localhost:5000',
        ws: true,
      },
      '/scenarioHub': {
        target: 'http://localhost:5000',
        ws: true,
      },
      '/stubHub': {
        target: 'http://localhost:5000',
        ws: true,
      },
    },
  },
  base: '',
})
