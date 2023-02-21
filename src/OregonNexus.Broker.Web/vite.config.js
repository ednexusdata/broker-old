import { fileURLToPath, URL } from "url";
import { defineConfig } from 'vite'
import * as path from 'path';
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  define: {
    'process.env': process.env
  },
  build: {
    lib: {
      entry: path.resolve(__dirname, 'App/main.js'),
      name: 'Bundle',
      fileName: (format) => `bundle.${format}.js`
    },
    outDir: "wwwroot/dist",
    emptyOutDir: true,
    rollupOptions: {
      input: {
        a: 'App/main.js'
      }
    },
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./App', import.meta.url)),
    }
  }
})
