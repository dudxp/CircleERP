import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
      '@components' : "/src/components",
      '@shared' : "/src/shared",
      '@styles' : "/src/styles",
      '@pages' : "/src/pages",
    },
  },
  server: {
    port: 3000,
    open: true,
  },
})
