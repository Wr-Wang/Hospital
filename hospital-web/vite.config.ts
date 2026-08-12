import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://192.168.31.19:8080',
        changeOrigin: true,
      },
    },
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
    // naive-ui 为全量引入（app.use(naive)），其独立 chunk 约 1.36MB（gzip 约 370kB），
    // 阈值相应调高；后续若改为按需引入可再收紧
    chunkSizeWarningLimit: 1500,
    rolldownOptions: {
      output: {
        // 第三方库分包：主包只留业务代码，避免单个大 chunk
        manualChunks(id) {
          if (!id.includes('node_modules')) return
          // naive-ui 及其内部依赖（vueuc / vooks / css-render 等）
          if (/naive-ui|vueuc|vooks|seemly|treemate|css-render|evtd|async-validator|date-fns/.test(id)) {
            return 'naive-ui'
          }
          // Vue 全家桶
          if (/(^|[\\/])(vue|@vue|vue-router|pinia|vue-demi)([\\/]|$)/.test(id)) {
            return 'vue-vendor'
          }
          if (/axios/.test(id)) {
            return 'axios'
          }
          return 'vendor'
        },
      },
    },
  },
})
