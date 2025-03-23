import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import { fileURLToPath } from 'url';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
      '@components': '/src/components',
      '@shared': '/src/shared',
      '@styles': '/src/styles',
      '@pages': '/src/pages',
    },
  },
  server: {
    port: 54783,
    open: true,
    host: '0.0.0.0'
  },
  define: {
    __APP_HOST__: JSON.stringify(process.env.VITE_APP_HOST || 'http://localhost'),
    __APP_PORT__: JSON.stringify(process.env.VITE_APP_PORT || '54783'),
    __BACKEND_HOST__: JSON.stringify(process.env.VITE_APP_HOST || 'https://localhost'),
    __BACKEND_PORT__: JSON.stringify(process.env.VITE_APP_PORT || '5001'),
  }
});
