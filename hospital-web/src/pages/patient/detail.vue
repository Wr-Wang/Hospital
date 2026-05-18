<template>
  <div>
    <n-page-header subtitle="患者" @back="handleBack">
      <template #title>
        <n-h2 style="margin: 0">患者详情</n-h2>
      </template>
    </n-page-header>

    <n-spin :show="loading">
      <template v-if="patient">
        <n-card title="基本信息" style="margin-top: 16px">
          <n-descriptions bordered :column="2">
            <n-descriptions-item label="病历号">{{ patient.patientNo }}</n-descriptions-item>
            <n-descriptions-item label="姓名">{{ patient.name }}</n-descriptions-item>
            <n-descriptions-item label="性别">{{ patient.gender === 'Male' ? '男' : patient.gender === 'Female' ? '女' : '-' }}</n-descriptions-item>
            <n-descriptions-item label="出生日期">{{ patient.birthDate || '-' }}</n-descriptions-item>
            <n-descriptions-item label="电话">{{ patient.phone || '-' }}</n-descriptions-item>
            <n-descriptions-item label="身份证号">{{ patient.idCard || '-' }}</n-descriptions-item>
            <n-descriptions-item label="过敏史" :span="2">
              <n-tag v-if="patient.allergiesText" type="warning">{{ patient.allergiesText }}</n-tag>
              <span v-else>无</span>
            </n-descriptions-item>
          </n-descriptions>
        </n-card>

        <n-card title="就诊历史" style="margin-top: 16px">
          <template v-if="patient.recentVisits && patient.recentVisits.length > 0">
            <n-data-table
              :columns="visitColumns"
              :data="patient.recentVisits"
              :bordered="true"
              :paginate="false"
            />
          </template>
          <n-empty v-else description="暂无就诊记录" />
        </n-card>
      </template>
    </n-spin>
  </div>
</template>

<script setup lang="ts">
import { ref, h, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useMessage, NTag, NPageHeader, NDescriptions } from 'naive-ui'
import type { DataTableColumns } from 'naive-ui'
import type { PatientProfileDto, VisitSummaryDto } from '../../types'
import { getPatientProfile } from '../../api/patient'

const route = useRoute()
const router = useRouter()
const message = useMessage()

const loading = ref(true)
const patient = ref<PatientProfileDto | null>(null)

const visitColumns: DataTableColumns<VisitSummaryDto> = [
  { title: '日期', key: 'date', width: 110 },
  { title: '科室', key: 'deptName', width: 120 },
  { title: '医生', key: 'doctorName', width: 100 },
  { title: '诊断', key: 'diagnosis', ellipsis: { tooltip: true } },
]

function handleBack() {
  router.push('/patient/search')
}

async function loadPatient(id: number) {
  loading.value = true
  try {
    const res = await getPatientProfile(id)
    patient.value = res.data
  } catch (err: any) {
    message.error(err.response?.data?.message || '加载患者信息失败')
    router.push('/patient/search')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  const id = Number(route.params.id)
  if (isNaN(id)) {
    message.error('无效的患者 ID')
    router.push('/patient/search')
    return
  }
  loadPatient(id)
})
</script>
