<template>
  <div>
    <n-page-header subtitle="患者">
      <template #title>
        <n-h2 style="margin: 0">患者检索</n-h2>
      </template>
      <template #extra>
        <n-button type="primary" @click="$router.push('/patient/create')">新建患者</n-button>
      </template>
    </n-page-header>

    <n-card style="margin-top: 16px">
      <n-space style="margin-bottom: 16px">
        <n-input
          v-model:value="keyword"
          placeholder="搜索姓名 / 病历号 / 身份证 / 电话"
          clearable
          style="width: 360px"
          @keyup.enter="handleSearch"
        />
        <n-button type="primary" @click="handleSearch">搜索</n-button>
      </n-space>

      <n-data-table
        :columns="columns"
        :data="data"
        :loading="loading"
        :bordered="true"
        :paginate="false"
      />

      <n-space v-if="totalCount > 0" justify="center" style="margin-top: 16px">
        <n-pagination
          :page="page"
          :page-size="size"
          :item-count="totalCount"
          @update:page="onPageChange"
        />
      </n-space>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref, h, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useMessage, NButton, NTag, NSpace, NPageHeader } from 'naive-ui'
import type { DataTableColumns } from 'naive-ui'
import type { PatientDto } from '../../types'
import { searchPatients } from '../../api/patient'

const router = useRouter()
const message = useMessage()

const keyword = ref('')
const loading = ref(false)
const data = ref<PatientDto[]>([])
const totalCount = ref(0)
const page = ref(1)
const size = ref(20)

const columns: DataTableColumns<PatientDto> = [
  { title: '病历号', key: 'patientNo', width: 120 },
  { title: '姓名', key: 'name', width: 100 },
  {
    title: '性别',
    key: 'gender',
    width: 60,
    render(row) {
      return row.gender === 'Male' ? '男' : row.gender === 'Female' ? '女' : '-'
    },
  },
  { title: '出生日期', key: 'birthDate', width: 110 },
  { title: '电话', key: 'phone', width: 130 },
  { title: '身份证号', key: 'idCard', width: 180, ellipsis: { tooltip: true } },
  {
    title: '过敏史',
    key: 'allergiesText',
    ellipsis: { tooltip: true },
    render(row) {
      if (!row.allergiesText) return '-'
      return h(NTag, { size: 'small', type: 'warning' }, { default: () => row.allergiesText })
    },
  },
  {
    title: '操作',
    key: 'actions',
    width: 120,
    render(row) {
      return h(NButton, {
        size: 'small',
        quaternary: true,
        type: 'primary',
        onClick: () => router.push(`/patient/detail/${row.id}`),
      }, { default: () => '查看详情' })
    },
  },
]

async function handleSearch() {
  page.value = 1
  await loadData()
}

async function onPageChange(p: number) {
  page.value = p
  await loadData()
}

async function loadData() {
  loading.value = true
  try {
    const res = await searchPatients(keyword.value || undefined, page.value, size.value)
    data.value = res.data.items
    totalCount.value = res.data.totalCount
  } catch (err: any) {
    message.error(err.response?.data?.message || '搜索失败')
  } finally {
    loading.value = false
  }
}
</script>
