import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { fileURLToPath } from 'url';

const resolveSrc = (segment: string) =>
  fileURLToPath(new URL(`./src/${segment}`, import.meta.url));

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
      '@app': resolveSrc('app'),
      '@features': resolveSrc('features'),
      '@shared': resolveSrc('shared'),
      '@styles': resolveSrc('styles'),
      '@assets': resolveSrc('assets'),
    },
  },
  server: {
    port: Number(process.env.VITE_CLIENT_PORT ?? 54783),
    open: true,
    host: '0.0.0.0',
  },
  define: {
    // A URL da API vem de uma unica variavel, para nao reaproveitar a porta do client.
    __API_BASE_URL__: JSON.stringify(
      process.env.VITE_API_BASE_URL ?? 'https://localhost:5001/api',
    ),
  },
});
