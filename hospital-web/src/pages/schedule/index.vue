<template>
  <div>
    <n-page-header subtitle="排班">
      <template #title>
        <n-h2 style="margin: 0">排班管理</n-h2>
      </template>
      <template #extra>
        <n-button type="primary" @click="openCreateModal">新建排班</n-button>
      </template>
    </n-page-header>

    <!-- 筛选栏 -->
    <n-card style="margin-top: 16px">
      <n-space align="center">
        <n-select
          v-model:value="filterCampusId"
          :options="campusOptions"
          placeholder="选择院区"
          clearable
          style="width: 160px"
          @update:value="onCampusFilterChange"
        />
        <n-select
          v-model:value="filterDeptId"
          :options="deptOptions"
          placeholder="选择科室"
          clearable
          style="width: 160px"
          @update:value="onDeptFilterChange"
        />
        <n-select
          v-model:value="filterDoctorId"
          :options="doctorOptions"
          placeholder="选择医生"
          clearable
          style="width: 160px"
          @update:value="onDoctorFilterChange"
        />
        <n-date-picker
          v-model:value="filterDateTs"
          type="date"
          placeholder="选择日期"
          clearable
          style="width: 160px"
          @update:value="onDateFilterChange"
        />
        <n-button type="primary" @click="loadData">查询</n-button>
      </n-space>
    </n-card>

    <!-- 排班列表 -->
    <n-card style="margin-top: 16px">
      <n-data-table
        :columns="columns"
        :data="scheduleList"
        :loading="loading"
        :bordered="true"
        :paginate="false"
      />

      <!-- 展开详情：时段列表 -->
      <template v-for="s in scheduleList" :key="s.id">
        <div v-if="expandedRow === s.id" style="padding: 12px 0 0 48px">
          <n-tag v-for="sl in s.slots" :key="sl.id" style="margin-right: 8px; margin-bottom: 4px" :type="sl.availableQuota > 0 ? 'success' : 'error'">
            {{ sl.slotName }} {{ sl.startTime }}-{{ sl.endTime }} 剩余{{ sl.availableQuota }}/{{ sl.totalQuota }}
          </n-tag>
        </div>
      </template>
    </n-card>

    <!-- 新建排班弹窗 -->
    <n-modal v-model:show="showModal" title="新建排班" preset="card" style="width: 600px">
      <n-form ref="formRef" :model="form" :rules="rules" label-placement="left" label-width="80px">
        <n-form-item path="doctorId" label="医生">
          <n-select
            v-model:value="form.doctorId"
            :options="allDoctorOptions"
            placeholder="选择医生"
            filterable
            @update:value="onDoctorSelect"
          />
        </n-form-item>
        <n-form-item path="scheduleDate" label="日期">
          <n-date-picker v-model:value="form.scheduleDateTs" type="date" placeholder="选择排班日期" style="width: 100%" />
        </n-form-item>

        <n-divider>时段设置</n-divider>

        <div v-for="(slot, i) in form.slots" :key="i" style="display: flex; gap: 8px; margin-bottom: 8px; align-items: flex-start">
          <n-input v-model:value="slot.slotName" placeholder="时段名称" style="width: 100px" />
          <n-time-picker v-model:value="slot.startTimeTs" format="HH:mm" placeholder="开始" style="width: 130px" />
          <n-time-picker v-model:value="slot.endTimeTs" format="HH:mm" placeholder="结束" style="width: 130px" />
          <n-input-number v-model:value="slot.totalQuota" :min="1" placeholder="配额" style="width: 90px" />
          <n-button size="small" quaternary circle type="error" @click="removeSlot(i)" :disabled="form.slots.length <= 1">
            ✕
          </n-button>
        </div>

        <n-button size="small" @click="addSlot">+ 添加时段</n-button>
      </n-form>

      <template #footer>
        <n-space justify="end">
          <n-button @click="showModal = false">取消</n-button>
          <n-button type="primary" :loading="submitting" @click="handleSubmit">保存</n-button>
        </n-space>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, h, computed, onMounted } from 'vue'
import { useMessage, NButton, NTag, NSpace, NPageHeader, NCard, NDivider, NTimePicker } from 'naive-ui'
import type { DataTableColumns, FormRules, FormInst, SelectOption } from 'naive-ui'
import type { ScheduleDto, CampusDto, DepartmentDto, StaffDto } from '../../types'
import { getScheduleByDept, getScheduleByDoctor, createSchedule, deactivateSchedule } from '../../api/schedule'
import { getCampusList } from '../../api/campus'
import { getDepartmentList } from '../../api/department'
import { getStaffList } from '../../api/staff'

const message = useMessage()
const formRef = ref<FormInst | null>(null)

const loading = ref(false)
const submitting = ref(false)
const showModal = ref(false)
const expandedRow = ref<number | null>(null)

const scheduleList = ref<ScheduleDto[]>([])
const campusList = ref<CampusDto[]>([])
const allDepartments = ref<DepartmentDto[]>([])
const allStaff = ref<StaffDto[]>([])

const filterCampusId = ref<number | null>(null)
const filterDeptId = ref<number | null>(null)
const filterDoctorId = ref<number | null>(null)
const filterDateTs = ref<number | null>(null)

// ===== 计算选项 =====

const campusOptions = computed(() =>
  campusList.value.map((c) => ({ label: c.name, value: c.id })),
)

const deptOptions = computed<SelectOption[]>(() => {
  if (!filterCampusId.value) return []
  return allDepartments.value
    .filter((d) => d.campusId === filterCampusId.value)
    .map((d) => ({ label: d.name, value: d.id }))
})

const doctorOptions = computed<SelectOption[]>(() => {
  let staff = allStaff.value.filter((s) => s.isActive)
  if (filterDeptId.value) {
    staff = staff.filter((s) => s.deptId === filterDeptId.value)
  }
  return staff.map((s) => ({ label: `${s.name} (${s.code})`, value: s.id }))
})

const allDoctorOptions = computed<SelectOption[]>(() =>
  allStaff.value
    .filter((s) => s.isActive)
    .map((s) => ({ label: `${s.name} (${s.code})`, value: s.id })),
)

// ===== 表单 =====

const form = reactive({
  doctorId: null as number | null,
  scheduleDateTs: null as number | null,
  slots: [] as { slotName: string; startTimeTs: number | null; endTimeTs: number | null; totalQuota: number }[],
})

const rules: FormRules = {
  doctorId: [{ required: true, type: 'number', message: '请选择医生', trigger: 'change' }],
  scheduleDate: [{ required: true, message: '请选择日期', trigger: 'change' }],
}

function addSlot() {
  form.slots.push({ slotName: '', startTimeTs: null, endTimeTs: null, totalQuota: 20 })
}

function removeSlot(index: number) {
  form.slots.splice(index, 1)
}

function onDoctorSelect(doctorId: number) {
  const doctor = allStaff.value.find((s) => s.id === doctorId)
  // 医生所属科室由 backend 决定，但我们在表单中不需要额外设置 deptId
}

// ===== 表格列定义 =====

function getDeptName(deptId: number): string {
  return allDepartments.value.find((d) => d.id === deptId)?.name ?? `#${deptId}`
}

function getDoctorName(doctorId: number): string {
  return allStaff.value.find((s) => s.id === doctorId)?.name ?? `#${doctorId}`
}

function getCampusName(campusId: number): string {
  return campusList.value.find((c) => c.id === campusId)?.name ?? `#${campusId}`
}

const columns: DataTableColumns<ScheduleDto> = [
  { title: '日期', key: 'scheduleDate', width: 110 },
  {
    title: '医生',
    key: 'doctorId',
    width: 100,
    render(row) { return getDoctorName(row.doctorId) },
  },
  {
    title: '科室',
    key: 'deptId',
    width: 100,
    render(row) { return getDeptName(row.deptId) },
  },
  {
    title: '院区',
    key: 'campusId',
    width: 100,
    render(row) { return getCampusName(row.campusId) },
  },
  {
    title: '状态',
    key: 'status',
    width: 80,
    render(row) {
      const map: Record<string, 'success' | 'error' | 'warning'> = { 已发布: 'success', 已停用: 'error', 已满: 'warning' }
      return h(NTag, { type: map[row.status] || 'default', size: 'small' }, { default: () => row.status })
    },
  },
  {
    title: '时段',
    key: 'slots',
    ellipsis: { tooltip: true },
    render(row) {
      return row.slots.map((s) => `${s.slotName}(${s.availableQuota}/${s.totalQuota})`).join(' ')
    },
  },
  {
    title: '操作',
    key: 'actions',
    width: 200,
    render(row) {
      return h(NSpace, { size: 'small' }, {
        default: () => {
          if (row.status === '已发布') {
            return h(NButton, { size: 'small', quaternary: true, type: 'warning', onClick: () => handleDeactivate(row) }, { default: () => '停用' })
          }
          return null
        },
      })
    },
  },
]

// ===== 数据加载 =====

async function loadCampuses() {
  try {
    const res = await getCampusList()
    campusList.value = res.data
  } catch { /* ignore */ }
}

async function loadDepartments() {
  try {
    const res = await getDepartmentList()
    allDepartments.value = res.data
  } catch { /* ignore */ }
}

async function loadStaff() {
  try {
    const res = await getStaffList()
    allStaff.value = res.data
  } catch { /* ignore */ }
}

async function loadData() {
  loading.value = true
  try {
    if (filterDoctorId.value) {
      const res = await getScheduleByDoctor(filterDoctorId.value)
      scheduleList.value = filterByDate(res.data)
    } else if (filterDeptId.value) {
      const dateStr = filterDateTs.value ? new Date(filterDateTs.value).toISOString().split('T')[0] : undefined
      const res = await getScheduleByDept(filterDeptId.value, dateStr)
      scheduleList.value = res.data
    } else {
      scheduleList.value = []
      message.info('请选择至少一个筛选条件（科室或医生）')
    }
  } catch (err: any) {
    message.error(err.response?.data?.message || '加载排班列表失败')
  } finally {
    loading.value = false
  }
}

function filterByDate(list: ScheduleDto[]): ScheduleDto[] {
  if (!filterDateTs.value) return list
  const dateStr = new Date(filterDateTs.value).toISOString().split('T')[0]
  return list.filter((s) => s.scheduleDate === dateStr)
}

function onCampusFilterChange() {
  filterDeptId.value = null
  filterDoctorId.value = null
}

function onDeptFilterChange() {
  filterDoctorId.value = null
}

function onDoctorFilterChange() {
  // doc selected, dept filter not needed for API
}

function onDateFilterChange() {
  // trigger handled by query button
}

// ===== 操作 =====

function openCreateModal() {
  form.doctorId = null
  form.scheduleDateTs = null
  form.slots = [{ slotName: '上午', startTimeTs: null, endTimeTs: null, totalQuota: 30 }]
  showModal.value = true
}

async function handleSubmit() {
  try {
    await formRef.value?.validate()
  } catch {
    return
  }

  if (form.slots.some((s) => !s.slotName || s.startTimeTs === null || s.endTimeTs === null || !s.totalQuota)) {
    message.warning('请完善所有时段信息')
    return
  }

  submitting.value = true
  try {
    const doctor = allStaff.value.find((s) => s.id === form.doctorId)
    if (!doctor) {
      message.error('请选择医生')
      return
    }

    const scheduleDate = form.scheduleDateTs
      ? new Date(form.scheduleDateTs).toISOString().split('T')[0]
      : ''

    const slots = form.slots.map((s) => ({
      slotName: s.slotName,
      startTime: s.startTimeTs !== null ? formatTime(s.startTimeTs) : '',
      endTime: s.endTimeTs !== null ? formatTime(s.endTimeTs) : '',
      totalQuota: s.totalQuota,
    }))

    await createSchedule({
      doctorId: form.doctorId!,
      deptId: doctor.deptId,
      campusId: doctor.campusId,
      scheduleDate,
      slots,
    })
    message.success('排班已创建')
    showModal.value = false
    await loadData()
  } catch (err: any) {
    message.error(err.response?.data?.message || '创建失败')
  } finally {
    submitting.value = false
  }
}

function formatTime(ts: number): string {
  const d = new Date(ts)
  return `${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`
}

async function handleDeactivate(row: ScheduleDto) {
  try {
    await deactivateSchedule(row.id)
    message.success('排班已停用')
    await loadData()
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  }
}

onMounted(async () => {
  await Promise.all([loadCampuses(), loadDepartments(), loadStaff()])
})
</script>
