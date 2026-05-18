<template>
  <div>
    <n-page-header subtitle="院区">
      <template #title>
        <n-h2 style="margin: 0">院区管理</n-h2>
      </template>
      <template #extra>
        <n-button type="primary" @click="openCreateModal">新建院区</n-button>
      </template>
    </n-page-header>

    <n-data-table
      :columns="columns"
      :data="campusList"
      :loading="loading"
      :bordered="true"
      :paginate="false"
      style="margin-top: 16px"
    />

    <!-- 新建/编辑弹窗 -->
    <n-modal v-model:show="showModal" :title="isEditing ? '编辑院区' : '新建院区'" preset="card" style="width: 520px">
      <n-form ref="formRef" :model="form" :rules="rules" label-placement="left" label-width="80px">
        <n-form-item path="code" label="编码" v-if="!isEditing">
          <n-input v-model:value="form.code" placeholder="院区编码，2-20个字符" :disabled="isEditing" />
        </n-form-item>
        <n-form-item path="name" label="名称">
          <n-input v-model:value="form.name" placeholder="院区名称" />
        </n-form-item>
        <n-form-item path="address" label="地址">
          <n-input v-model:value="form.address" placeholder="院区地址" />
        </n-form-item>
        <n-form-item path="phone" label="电话">
          <n-input v-model:value="form.phone" placeholder="联系电话" />
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
import { ref, reactive, h, onMounted } from 'vue'
import { useMessage, NButton, NTag, NSpace, NPageHeader } from 'naive-ui'
import type { DataTableColumns, FormRules, FormInst } from 'naive-ui'
import type { CampusDto } from '../../types'
import { getCampusList, createCampus, updateCampus, activateCampus, deactivateCampus } from '../../api/campus'

const message = useMessage()
const formRef = ref<FormInst | null>(null)

const loading = ref(false)
const submitting = ref(false)
const showModal = ref(false)
const isEditing = ref(false)
const editingId = ref<number | null>(null)
const campusList = ref<CampusDto[]>([])

const form = reactive({
  code: '',
  name: '',
  address: '',
  phone: '',
})

const rules: FormRules = {
  code: [
    { required: true, message: '请输入院区编码', trigger: 'blur' },
    { min: 2, max: 20, message: '编码长度 2-20 个字符', trigger: 'blur' },
  ],
  name: [{ required: true, message: '请输入院区名称', trigger: 'blur' }],
}

const columns: DataTableColumns<CampusDto> = [
  { title: '编码', key: 'code', width: 120 },
  { title: '名称', key: 'name', width: 200 },
  { title: '地址', key: 'address', ellipsis: { tooltip: true } },
  { title: '电话', key: 'phone', width: 150 },
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

async function loadData() {
  loading.value = true
  try {
    const res = await getCampusList()
    campusList.value = res.data
  } catch (err: any) {
    message.error(err.response?.data?.message || '加载院区列表失败')
  } finally {
    loading.value = false
  }
}

function openCreateModal() {
  isEditing.value = false
  editingId.value = null
  form.code = ''
  form.name = ''
  form.address = ''
  form.phone = ''
  showModal.value = true
}

function openEditModal(row: CampusDto) {
  isEditing.value = true
  editingId.value = row.id
  form.code = row.code
  form.name = row.name
  form.address = row.address || ''
  form.phone = row.phone || ''
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
      await updateCampus(editingId.value, { name: form.name, address: form.address || undefined, phone: form.phone || undefined })
      message.success('院区已更新')
    } else {
      await createCampus({ code: form.code, name: form.name, address: form.address || undefined, phone: form.phone || undefined })
      message.success('院区已创建')
    }
    showModal.value = false
    await loadData()
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  } finally {
    submitting.value = false
  }
}

async function toggleActive(row: CampusDto) {
  try {
    if (row.isActive) {
      await deactivateCampus(row.id)
      message.success('院区已停用')
    } else {
      await activateCampus(row.id)
      message.success('院区已启用')
    }
    await loadData()
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  }
}

onMounted(loadData)
</script>
