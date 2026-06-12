<template>
  <div>
    <n-page-header subtitle="人员">
      <template #title>
        <n-h2 style="margin: 0">人员管理</n-h2>
      </template>
      <template #extra>
        <n-space align="center">
          <n-select
            v-model:value="filterCampusId"
            :options="campusOptions"
            placeholder="选择院区"
            clearable
            style="width: 200px"
            @update:value="onCampusFilterChange"
          />
          <n-select
            v-model:value="filterDeptId"
            :options="deptOptions"
            placeholder="选择科室"
            clearable
            style="width: 200px"
            @update:value="loadData"
          />
          <n-button type="primary" @click="openCreateModal">新建人员</n-button>
        </n-space>
      </template>
    </n-page-header>

    <n-data-table
      :columns="columns"
      :data="staffList"
      :loading="loading"
      :bordered="true"
      :paginate="false"
      style="margin-top: 16px"
    />

    <!-- 新建/编辑弹窗 -->
    <n-modal v-model:show="showModal" :title="isEditing ? '编辑人员' : '新建人员'" preset="card" style="width: 560px">
      <n-form ref="formRef" :model="form" :rules="rules" label-placement="left" label-width="90px">
        <n-form-item path="code" label="编码" v-if="!isEditing">
          <n-input v-model:value="form.code" placeholder="人员编码" :disabled="isEditing" />
        </n-form-item>
        <n-form-item path="name" label="姓名">
          <n-input v-model:value="form.name" placeholder="人员姓名" />
        </n-form-item>
        <n-form-item path="gender" label="性别">
          <n-select v-model:value="form.gender" :options="genderOptions" placeholder="选择性别" />
        </n-form-item>
        <n-form-item path="phone" label="电话">
          <n-input v-model:value="form.phone" placeholder="联系电话" />
        </n-form-item>
        <n-form-item path="campusId" label="所属院区" v-if="!isEditing">
          <n-select v-model:value="form.campusId" :options="campusOptions" placeholder="选择院区" :disabled="isEditing" @update:value="onCampusChange" />
        </n-form-item>
        <n-form-item path="deptId" label="所属科室">
          <n-select v-model:value="form.deptId" :options="deptOptionsForForm" placeholder="选择科室" />
        </n-form-item>
        <n-form-item path="licenseType" label="执业类型">
          <n-select v-model:value="form.licenseType" :options="licenseTypeOptions" placeholder="选择执业类型" />
        </n-form-item>
        <n-form-item path="licenseNo" label="执业证号">
          <n-input v-model:value="form.licenseNo" placeholder="执业证书编号" />
        </n-form-item>
        <n-form-item path="licenseExpiry" label="有效期">
          <n-date-picker v-model:value="form.licenseExpiryTs" type="date" placeholder="选择日期" clearable style="width: 100%" />
        </n-form-item>
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
import { useMessage, NButton, NTag, NSpace, NPageHeader } from 'naive-ui'
import type { DataTableColumns, FormRules, FormInst, SelectOption } from 'naive-ui'
import type { StaffDto, CampusDto, DepartmentDto } from '../../types'
import { getStaffList, getStaffByCampus, getStaffByDept, createStaff, updateStaff, activateStaff, deactivateStaff } from '../../api/staff'
import { getCampusList } from '../../api/campus'
import { getDepartmentList } from '../../api/department'

const message = useMessage()
const formRef = ref<FormInst | null>(null)

const loading = ref(false)
const submitting = ref(false)
const showModal = ref(false)
const isEditing = ref(false)
const editingId = ref<number | null>(null)
const staffList = ref<StaffDto[]>([])
const campusList = ref<CampusDto[]>([])
const allDepartments = ref<DepartmentDto[]>([])
const filterCampusId = ref<number | null>(null)
const filterDeptId = ref<number | null>(null)

const genderOptions: SelectOption[] = [
  { label: '男', value: 'Male' },
  { label: '女', value: 'Female' },
]

const licenseTypeOptions: SelectOption[] = [
  { label: '执业医师', value: '执业医师' },
  { label: '执业护士', value: '执业护士' },
  { label: '药师', value: '药师' },
  { label: '医技', value: '医技' },
]

const form = reactive({
  code: '',
  name: '',
  gender: '',
  phone: '',
  campusId: null as number | null,
  deptId: null as number | null,
  licenseType: '',
  licenseNo: '',
  licenseExpiryTs: null as number | null,
})

const rules: FormRules = {
  code: [{ required: true, message: '请输入人员编码', trigger: 'blur' }],
  name: [{ required: true, message: '请输入人员姓名', trigger: 'blur' }],
  gender: [{ required: true, message: '请选择性别', trigger: 'change' }],
  campusId: [{ required: true, type: 'number', message: '请选择院区', trigger: 'change' }],
  deptId: [{ required: true, type: 'number', message: '请选择科室', trigger: 'change' }],
  licenseType: [{ required: true, message: '请输入执业类型', trigger: 'blur' }],
  licenseNo: [{ required: true, message: '请输入执业证号', trigger: 'blur' }],
}

const campusOptions = computed(() =>
  campusList.value.map((c) => ({ label: c.name, value: c.id })),
)

const deptOptions = computed<SelectOption[]>(() => {
  if (!filterCampusId.value) return []
  return allDepartments.value
    .filter((d) => d.campusId === filterCampusId.value)
    .map((d) => ({ label: d.name, value: d.id }))
})

const deptOptionsForForm = computed<SelectOption[]>(() => {
  const campusId = form.campusId
  if (!campusId) return []
  return allDepartments.value
    .filter((d) => d.campusId === campusId)
    .map((d) => ({ label: d.name, value: d.id }))
})

function getDeptName(deptId: number): string {
  return allDepartments.value.find((d) => d.id === deptId)?.name ?? `#${deptId}`
}

const columns: DataTableColumns<StaffDto> = [
  { title: '编码', key: 'code', width: 100 },
  { title: '姓名', key: 'name', width: 100 },
  {
    title: '性别',
    key: 'gender',
    width: 60,
    render(row) {
      return row.gender === 'Male' ? '男' : row.gender === 'Female' ? '女' : row.gender
    },
  },
  { title: '电话', key: 'phone', width: 130 },
  {
    title: '所属科室',
    key: 'deptId',
    width: 120,
    render(row) {
      return getDeptName(row.deptId)
    },
  },
  {
    title: '执业类型',
    key: 'licenseType',
    width: 100,
  },
  {
    title: '执业证号',
    key: 'licenseNo',
    width: 150,
  },
  {
    title: '执业状态',
    key: 'isLicenseExpired',
    width: 90,
    render(row) {
      return h(NTag, { type: row.isLicenseExpired ? 'error' : 'success', size: 'small' }, { default: () => row.isLicenseExpired ? '过期' : '有效' })
    },
  },
  {
    title: '状态',
    key: 'isActive',
    width: 80,
    render(row) {
      return h(NTag, { type: row.isActive ? 'success' : 'error', size: 'small' }, { default: () => row.isActive ? '启用' : '停用' })
    },
  },
  {
    title: '操作',
    key: 'actions',
    width: 220,
    render(row) {
      return h(NSpace, { size: 'small' }, {
        default: () => [
          h(NButton, { size: 'small', quaternary: true, onClick: () => openEditModal(row) }, { default: () => '编辑' }),
          h(NButton, {
            size: 'small',
            quaternary: true,
            type: row.isActive ? 'warning' : 'success',
            onClick: () => toggleActive(row),
          }, { default: () => row.isActive ? '停用' : '启用' }),
        ],
      })
    },
  },
]

async function loadCampuses() {
  try {
    const res = await getCampusList()
    campusList.value = res.data
  } catch { /* ignore */ }
}

async function loadAllDepartments() {
  try {
    const res = await getDepartmentList()
    allDepartments.value = res.data
  } catch { /* ignore */ }
}

async function loadData() {
  loading.value = true
  try {
    if (filterDeptId.value) {
      const res = await getStaffByDept(filterDeptId.value)
      staffList.value = res.data
    } else if (filterCampusId.value) {
      const res = await getStaffByCampus(filterCampusId.value)
      staffList.value = res.data
    } else {
      const res = await getStaffList()
      staffList.value = res.data
    }
  } catch (err: any) {
    message.error(err.response?.data?.message || '加载人员列表失败')
  } finally {
    loading.value = false
  }
}

function onCampusFilterChange() {
  filterDeptId.value = null
  loadData()
}

function onCampusChange(value: number | null) {
  form.deptId = null
  form.campusId = value
}

function openCreateModal() {
  isEditing.value = false
  editingId.value = null
  form.code = ''
  form.name = ''
  form.gender = ''
  form.phone = ''
  form.campusId = null
  form.deptId = null
  form.licenseType = ''
  form.licenseNo = ''
  form.licenseExpiryTs = null
  showModal.value = true
}

function openEditModal(row: StaffDto) {
  isEditing.value = true
  editingId.value = row.id
  form.code = row.code
  form.name = row.name
  form.gender = row.gender
  form.phone = row.phone || ''
  form.campusId = row.campusId
  form.deptId = row.deptId
  form.licenseType = row.licenseType
  form.licenseNo = row.licenseNo
  form.licenseExpiryTs = row.licenseExpiry ? new Date(row.licenseExpiry).getTime() : null
  showModal.value = true
}

async function handleSubmit() {
  try {
    await formRef.value?.validate()
  } catch {
    return
  }

  submitting.value = true
  try {
    const licenseExpiry = form.licenseExpiryTs ? new Date(form.licenseExpiryTs).toISOString().split('T')[0] : undefined

    if (isEditing.value && editingId.value) {
      await updateStaff(editingId.value, {
        name: form.name,
        gender: form.gender,
        phone: form.phone || undefined,
        deptId: form.deptId!,
      })
      message.success('人员已更新')
    } else {
      await createStaff({
        code: form.code,
        name: form.name,
        gender: form.gender,
        phone: form.phone || undefined,
        campusId: form.campusId!,
        deptId: form.deptId!,
        licenseType: form.licenseType,
        licenseNo: form.licenseNo,
        licenseExpiry,
      })
      message.success('人员已创建')
    }
    showModal.value = false
    await loadData()
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  } finally {
    submitting.value = false
  }
}

async function toggleActive(row: StaffDto) {
  try {
    if (row.isActive) {
      await deactivateStaff(row.id)
      message.success('人员已停用')
    } else {
      await activateStaff(row.id)
      message.success('人员已启用')
    }
    await loadData()
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  }
}

onMounted(async () => {
  await Promise.all([loadCampuses(), loadAllDepartments()])
  await loadData()
})
</script>
