<template>
  <div>
    <n-page-header subtitle="仪表盘">
      <template #title>
        <n-h2 style="margin: 0">首页概览</n-h2>
      </template>
    </n-page-header>

    <n-grid :cols="4" :x-gap="16" :y-gap="16" style="margin-top: 16px">
      <n-gi>
        <n-card title="患者总数" hoverable>
          <n-number-animation :from="0" :to="stats.totalPatients" :duration="800" style="font-size: 28px; font-weight: 700" />
        </n-card>
      </n-gi>
      <n-gi>
        <n-card title="员工总数" hoverable>
          <n-number-animation :from="0" :to="stats.totalStaff" :duration="800" style="font-size: 28px; font-weight: 700" />
        </n-card>
      </n-gi>
      <n-gi>
        <n-card title="院区数" hoverable>
          <n-number-animation :from="0" :to="stats.totalCampuses" :duration="800" style="font-size: 28px; font-weight: 700" />
        </n-card>
      </n-gi>
      <n-gi>
        <n-card title="科室数" hoverable>
          <n-number-animation :from="0" :to="stats.totalDepartments" :duration="800" style="font-size: 28px; font-weight: 700" />
        </n-card>
      </n-gi>
      <n-gi>
        <n-card title="排班数" hoverable>
          <n-number-animation :from="0" :to="stats.totalSchedules" :duration="800" style="font-size: 28px; font-weight: 700" />
        </n-card>
      </n-gi>
      <n-gi>
        <n-card title="系统用户" hoverable>
          <n-number-animation :from="0" :to="stats.totalUsers" :duration="800" style="font-size: 28px; font-weight: 700" />
        </n-card>
      </n-gi>
    </n-grid>

    <!-- 今日排班 -->
    <n-card title="今日排班" style="margin-top: 16px">
      <n-space v-if="!loadingSchedules" align="center">
        <span style="font-size: 24px; font-weight: 700">{{ todaySchedules.length }}</span>
        <span style="color: #888">条排班</span>
        <n-button quaternary size="small" type="primary" @click="router.push('/schedule')">查看全部 →</n-button>
      </n-space>
      <n-spin v-else size="small" />
      <n-empty v-if="!loadingSchedules && todaySchedules.length === 0" description="今日暂无排班" style="padding: 24px" />
    </n-card>

    <!-- 快捷入口 -->
    <n-card title="快捷入口" style="margin-top: 16px">
      <n-space>
        <n-button quaternary type="primary" @click="$router.push('/patient/create')">患者建档</n-button>
        <n-button quaternary type="primary" @click="$router.push('/patient/search')">患者检索</n-button>
        <n-button quaternary type="primary" @click="$router.push('/schedule')">排班管理</n-button>
        <n-button quaternary type="primary" @click="$router.push('/staff')">人员管理</n-button>
      </n-space>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { NButton, NPageHeader, NCard, NGrid, NGi, NNumberAnimation, NSpace, NEmpty, NSpin } from 'naive-ui'
import type { ScheduleDto } from '../../types'
import { searchPatients } from '../../api/patient'
import { getStaffList } from '../../api/staff'
import { getCampusList } from '../../api/campus'
import { getDepartmentList } from '../../api/department'
import { getUsers } from '../../api/userRole'
import { getScheduleByDept } from '../../api/schedule'

const router = useRouter()

const stats = reactive({
  totalPatients: 0,
  totalStaff: 0,
  totalCampuses: 0,
  totalDepartments: 0,
  totalSchedules: 0,
  totalUsers: 0,
})

const todaySchedules = ref<ScheduleDto[]>([])
const loadingSchedules = ref(false)
const todayStr = new Date().toISOString().split('T')[0]

async function loadStats() {
  try {
    const [patientRes, staffRes, campusRes, deptRes, userRes] = await Promise.allSettled([
      searchPatients(undefined, 1, 1),
      getStaffList(),
      getCampusList(),
      getDepartmentList(),
      getUsers(),
    ])

    if (patientRes.status === 'fulfilled') stats.totalPatients = patientRes.value.data.totalCount
    if (staffRes.status === 'fulfilled') stats.totalStaff = staffRes.value.data.length
    if (campusRes.status === 'fulfilled') stats.totalCampuses = campusRes.value.data.length
    if (deptRes.status === 'fulfilled') stats.totalDepartments = deptRes.value.data.length
    if (userRes.status === 'fulfilled') stats.totalUsers = userRes.value.data.length
  } catch { /* ignore */ }
}

async function loadTodaySchedules() {
  loadingSchedules.value = true
  try {
    // 加载所有活跃科室的排班
    const deptRes = await getDepartmentList()
    const activeDepts = deptRes.data.filter((d) => d.isActive)
    const results = await Promise.allSettled(
      activeDepts.map((d) => getScheduleByDept(d.id, todayStr)),
    )
    const all: ScheduleDto[] = []
    for (const r of results) {
      if (r.status === 'fulfilled') all.push(...r.value.data)
    }
    todaySchedules.value = all
    stats.totalSchedules += all.length
  } catch { /* ignore */ }
  finally { loadingSchedules.value = false }
}


onMounted(async () => {
  await Promise.all([loadStats(), loadTodaySchedules()])
})
</script>
