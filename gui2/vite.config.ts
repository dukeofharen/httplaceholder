import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue(), vueDevTools(), tailwindcss()],
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
