<template>
  <div>
    <n-page-header subtitle="科室">
      <template #title>
        <n-h2 style="margin: 0">科室管理</n-h2>
      </template>
      <template #extra>
        <n-space align="center">
          <n-select
            v-model:value="selectedCampusId"
            :options="campusOptions"
            placeholder="选择院区"
            clearable
            style="width: 200px"
            @update:value="loadTree"
          />
          <n-button type="primary" :disabled="!selectedCampusId" @click="openCreateModal">新建科室</n-button>
        </n-space>
      </template>
    </n-page-header>

    <n-data-table
      :columns="columns"
      :data="departmentTree"
      :loading="loading"
      :bordered="true"
      :paginate="false"
      :max-height="600"
      style="margin-top: 16px"
    />

    <!-- 新建/编辑弹窗 -->
    <n-modal v-model:show="showModal" :title="isEditing ? '编辑科室' : '新建科室'" preset="card" style="width: 520px">
      <n-form ref="formRef" :model="form" :rules="rules" label-placement="left" label-width="90px">
        <n-form-item path="code" label="编码" v-if="!isEditing">
          <n-input v-model:value="form.code" placeholder="科室编码，1-20个字符" :disabled="isEditing" />
        </n-form-item>
        <n-form-item path="name" label="名称">
          <n-input v-model:value="form.name" placeholder="科室名称" />
        </n-form-item>
        <n-form-item path="campusId" label="所属院区" v-if="!isEditing">
          <n-select v-model:value="form.campusId" :options="campusOptions" placeholder="选择院区" :disabled="isEditing" />
        </n-form-item>
        <n-form-item path="type" label="科室类型">
          <n-select v-model:value="form.type" :options="typeOptions" placeholder="选择类型" />
        </n-form-item>
        <n-form-item path="parentId" label="上级科室">
          <n-tree-select
            v-model:value="form.parentId"
            :options="parentOptions"
            placeholder="无（顶级科室）"
            clearable
            :disabled="!selectedCampusId && !isEditing"
          />
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
import { useMessage, NButton, NTag, NSpace, NPageHeader, NSelect, NTreeSelect } from 'naive-ui'
import type { DataTableColumns, FormRules, FormInst } from 'naive-ui'
import type { CampusDto, DepartmentDto } from '../../types'
import { getCampusList } from '../../api/campus'
import { getDepartmentTree, getDepartmentList, createDepartment, updateDepartment, activateDepartment, deactivateDepartment } from '../../api/department'

const message = useMessage()
const formRef = ref<FormInst | null>(null)

const loading = ref(false)
const submitting = ref(false)
const showModal = ref(false)
const isEditing = ref(false)
const editingId = ref<number | null>(null)
const departmentTree = ref<DepartmentDto[]>([])
const allDepartments = ref<DepartmentDto[]>([])
const campusList = ref<CampusDto[]>([])
const selectedCampusId = ref<number | null>(null)

const form = reactive({
  code: '',
  name: '',
  campusId: null as number | null,
  type: '',
  parentId: null as number | null,
})

const rules: FormRules = {
  code: [
    { required: true, message: '请输入科室编码', trigger: 'blur' },
    { min: 1, max: 20, message: '编码长度 1-20 个字符', trigger: 'blur' },
  ],
  name: [{ required: true, message: '请输入科室名称', trigger: 'blur' }],
  campusId: [{ required: true, type: 'number', message: '请选择院区', trigger: 'change' }],
  type: [{ required: true, message: '请选择科室类型', trigger: 'change' }],
}

const typeOptions = [
  { label: '门诊', value: '门诊' },
  { label: '住院', value: '住院' },
  { label: '医技', value: '医技' },
  { label: '行政', value: '行政' },
  { label: '药房', value: '药房' },
]

const campusOptions = computed(() =>
  campusList.value.map((c) => ({ label: c.name, value: c.id })),
)

/** 上级科室选项：当前院区下所有科室（排除自身及其子节点） */
const parentOptions = computed(() => {
  // 使用 allDepartments 构建树选项
  function buildOptions(list: DepartmentDto[]): any[] {
    return list
      .filter((d) => !isEditing.value || d.id !== editingId.value)
      .map((d) => ({
        label: d.name,
        value: d.id,
        children: d.children ? buildOptions(d.children) : undefined,
      }))
  }
  // 根据 selectedCampusId 或 form.campusId 过滤
  const campusId = selectedCampusId.value || form.campusId
  if (!campusId) return []
  // 从 allDepartments 中找到对应院区的根节点
  const roots = allDepartments.value.filter((d) => d.campusId === campusId && !d.parentId)
  return buildOptions(roots)
})

const columns: DataTableColumns<DepartmentDto> = [
  { title: '编码', key: 'code', width: 120 },
  { title: '名称', key: 'name', width: 200 },
  {
    title: '类型',
    key: 'type',
    width: 100,
    render(row) {
      return h(NTag, { size: 'small' }, { default: () => row.type })
    },
  },
  {
    title: '状态',
    key: 'isActive',
    width: 100,
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
    if (res.data.length > 0 && !selectedCampusId.value) {
      selectedCampusId.value = res.data[0].id
    }
  } catch {
    // 忽略
  }
}

async function loadTree() {
  if (!selectedCampusId.value) {
    departmentTree.value = []
    return
  }
  loading.value = true
  try {
    const res = await getDepartmentTree(selectedCampusId.value)
    departmentTree.value = res.data
  } catch (err: any) {
    message.error(err.response?.data?.message || '加载科室列表失败')
  } finally {
    loading.value = false
  }
}

async function loadAllDepartments() {
  try {
    const res = await getDepartmentList()
    allDepartments.value = res.data
  } catch {
    // 下拉选择用，忽略加载失败
  }
}

function openCreateModal() {
  isEditing.value = false
  editingId.value = null
  form.code = ''
  form.name = ''
  form.campusId = selectedCampusId.value
  form.type = ''
  form.parentId = null
  showModal.value = true
}

function openEditModal(row: DepartmentDto) {
  isEditing.value = true
  editingId.value = row.id
  form.code = row.code
  form.name = row.name
  form.campusId = row.campusId
  form.type = row.type
  form.parentId = row.parentId ?? null
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
    if (isEditing.value && editingId.value) {
      await updateDepartment(editingId.value, {
        name: form.name,
        type: form.type,
        parentId: form.parentId,
      })
      message.success('科室已更新')
    } else {
      await createDepartment({
        code: form.code,
        name: form.name,
        campusId: form.campusId!,
        type: form.type,
        parentId: form.parentId,
      })
      message.success('科室已创建')
    }
    showModal.value = false
    selectedCampusId.value = form.campusId
    await Promise.all([loadTree(), loadAllDepartments()])
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  } finally {
    submitting.value = false
  }
}

async function toggleActive(row: DepartmentDto) {
  try {
    if (row.isActive) {
      await deactivateDepartment(row.id)
      message.success('科室已停用')
    } else {
      await activateDepartment(row.id)
      message.success('科室已启用')
    }
    await loadTree()
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  }
}

onMounted(async () => {
  await loadCampuses()
  await loadAllDepartments()
  if (selectedCampusId.value) {
    await loadTree()
  }
})
</script>
