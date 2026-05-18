<template>
  <router-view />
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useMessage, useLoadingBar } from 'naive-ui'
import { setErrorHandler } from '../api/request'

const message = useMessage()
const loadingBar = useLoadingBar()
const router = useRouter()

// 注册全局错误提示
setErrorHandler((msg: string) => {
  message.error(msg, { duration: 4000 })
})

// 路由切换时显示加载进度条
router.beforeEach(() => {
  loadingBar.start()
})
router.afterEach(() => {
  loadingBar.finish()
})
router.onError(() => {
  loadingBar.error()
})
</script>
