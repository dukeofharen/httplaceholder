import { fileURLToPath, URL } from "url";

import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  server: {
    proxy: {
      "/ph-api": {
        target: "http://localhost:5000",
      },
      "/swagger": {
        target: "http://localhost:5000",
      },
      "/requestHub": {
        target: "http://localhost:5000",
        ws: true,
      },
      "/scenarioHub": {
        target: "http://localhost:5000",
        ws: true,
      },
    },
  },
  base: "",
});
