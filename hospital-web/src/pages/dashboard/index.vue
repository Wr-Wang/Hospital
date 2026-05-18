<template>
  <div>
    <n-h2>首页</n-h2>
    <n-grid :cols="4" :x-gap="16">
      <n-gi>
        <n-card title="今日挂号" hoverable>
          <n-number-animation :from="0" :to="0" :duration="1000" />
          <template #footer>人次</template>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card title="候诊人数" hoverable>
          <n-number-animation :from="0" :to="0" :duration="1000" />
          <template #footer>人</template>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card title="待收费" hoverable>
          <n-number-animation :from="0" :to="0" :duration="1000" />
          <template #footer>笔</template>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card title="待发药" hoverable>
          <n-number-animation :from="0" :to="0" :duration="1000" />
          <template #footer>笔</template>
        </n-card>
      </n-gi>
    </n-grid>
  </div>
</template>

<script setup lang="ts">
</script>
