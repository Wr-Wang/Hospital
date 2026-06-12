<template>
  <div>
    <n-page-header subtitle="角色">
      <template #title>
        <n-h2 style="margin: 0">角色管理</n-h2>
      </template>
      <template #extra>
        <n-button type="primary" @click="openCreateModal">新建角色</n-button>
      </template>
    </n-page-header>

    <n-card style="margin-top: 16px">
      <n-data-table
        :columns="columns"
        :data="roleList"
        :loading="loading"
        :bordered="true"
        :paginate="false"
      />
    </n-card>

    <!-- 新建/编辑弹窗 -->
    <n-modal v-model:show="showModal" :title="isEditing ? '编辑角色' : '新建角色'" preset="card" style="width: 560px">
      <n-form ref="formRef" :model="form" :rules="rules" label-placement="left" label-width="80px">
        <n-form-item path="name" label="名称" v-if="!isEditing">
          <n-input v-model:value="form.name" placeholder="角色编码，如 ADMIN" :disabled="isEditing" />
        </n-form-item>
        <n-form-item path="description" label="说明">
          <n-input v-model:value="form.description" placeholder="角色说明" type="textarea" rows="2" />
        </n-form-item>

        <n-divider>权限设置</n-divider>

        <n-space vertical>
          <div v-for="group in permissionGroups" :key="group.label">
            <n-checkbox
              :checked="isGroupChecked(group.permissions)"
              :indeterminate="isGroupIndeterminate(group.permissions)"
              @update:checked="toggleGroup(group.permissions, $event)"
            >
              <strong>{{ group.label }}</strong>
            </n-checkbox>
            <div style="padding-left: 24px; margin-top: 4px">
              <n-checkbox-group v-model:value="form.permissions">
                <n-space>
                  <n-checkbox
                    v-for="p in group.permissions"
                    :key="p.value"
                    :value="p.value"
                    :label="p.label"
                  />
                </n-space>
              </n-checkbox-group>
            </div>
          </div>
        </n-space>
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
import { useMessage, NButton, NSpace, NPageHeader, NCard, NDivider, NCheckbox, NCheckboxGroup } from 'naive-ui'
import type { DataTableColumns, FormRules, FormInst } from 'naive-ui'
import type { RoleDto } from '../../types'
import { getRoles, createRole, updateRole, deleteRole } from '../../api/userRole'

const message = useMessage()
const formRef = ref<FormInst | null>(null)

const loading = ref(false)
const submitting = ref(false)
const showModal = ref(false)
const isEditing = ref(false)
const editingId = ref<number | null>(null)

const roleList = ref<RoleDto[]>([])

const form = reactive({
  name: '',
  description: '',
  permissions: [] as string[],
})

const rules: FormRules = {
  name: [{ required: true, message: '请输入角色名称', trigger: 'blur' }],
}

// ===== 权限定义（与后端 Permissions.cs 一致） =====
interface PermissionDef {
  label: string
  value: string
}

interface PermissionGroup {
  label: string
  permissions: PermissionDef[]
}

const permissionGroups: PermissionGroup[] = [
  {
    label: '系统',
    permissions: [
      { label: '系统使用', value: 'sys.shell.use' },
      { label: '用户/角色管理', value: 'sys.security.manage' },
    ],
  },
  {
    label: '主数据',
    permissions: [
      { label: '院区管理', value: 'mdm.campus.manage' },
      { label: '科室管理', value: 'mdm.dept.manage' },
      { label: '人员管理', value: 'mdm.staff.manage' },
      { label: '字典管理', value: 'mdm.dict.manage' },
    ],
  },
  {
    label: '患者',
    permissions: [
      { label: '患者建档', value: 'pat.register' },
      { label: '患者检索', value: 'pat.search' },
    ],
  },
  {
    label: '排班',
    permissions: [
      { label: '排班管理', value: 'opd.schedule' },
    ],
  },
  {
    label: '临床（WPF 桌面端）',
    permissions: [
      { label: '挂号工作台', value: 'opd.register' },
      { label: '门诊医生站', value: 'opd.encounter' },
    ],
  },
  {
    label: '药房（WPF 桌面端）',
    permissions: [
      { label: '发药工作台', value: 'pha.dispense' },
    ],
  },
  {
    label: '收费（WPF 桌面端）',
    permissions: [
      { label: '收费工作台', value: 'fin.cash' },
    ],
  },
]

function isGroupChecked(perms: PermissionDef[]): boolean {
  return perms.every((p) => form.permissions.includes(p.value))
}

function isGroupIndeterminate(perms: PermissionDef[]): boolean {
  const selected = perms.filter((p) => form.permissions.includes(p.value))
  return selected.length > 0 && selected.length < perms.length
}

function toggleGroup(perms: PermissionDef[], checked: boolean) {
  const values = perms.map((p) => p.value)
  if (checked) {
    // 只添加当前组尚未选中的
    for (const v of values) {
      if (!form.permissions.includes(v)) {
        form.permissions.push(v)
      }
    }
  } else {
    form.permissions = form.permissions.filter((p) => !values.includes(p))
  }
}

// ===== 表格 =====

const columns: DataTableColumns<RoleDto> = [
  { title: '名称', key: 'name', width: 120 },
  { title: '说明', key: 'description', width: 200, ellipsis: { tooltip: true } },
  {
    title: '权限数',
    key: 'permissions',
    width: 80,
    render(row) { return row.permissions.length },
  },
  {
    title: '操作',
    key: 'actions',
    width: 180,
    render(row) {
      return h(NSpace, { size: 'small' }, {
        default: () => [
          h(NButton, { size: 'small', quaternary: true, onClick: () => openEditModal(row) }, { default: () => '编辑' }),
          h(NButton, { size: 'small', quaternary: true, type: 'error', onClick: () => handleDelete(row) }, { default: () => '删除' }),
        ],
      })
    },
  },
]

// ===== 数据加载 =====

async function loadData() {
  loading.value = true
  try {
    const res = await getRoles()
    roleList.value = res.data
  } catch (err: any) {
    message.error(err.response?.data?.message || '加载角色列表失败')
  } finally {
    loading.value = false
  }
}

// ===== 弹窗 =====

function openCreateModal() {
  isEditing.value = false
  editingId.value = null
  form.name = ''
  form.description = ''
  form.permissions = []
  showModal.value = true
}

function openEditModal(row: RoleDto) {
  isEditing.value = true
  editingId.value = row.id
  form.name = row.name
  form.description = row.description
  form.permissions = [...row.permissions]
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
      await updateRole(editingId.value, {
        description: form.description,
        permissions: form.permissions,
      })
      message.success('角色已更新')
    } else {
      await createRole({
        name: form.name,
        description: form.description,
        permissions: form.permissions,
      })
      message.success('角色已创建')
    }
    showModal.value = false
    await loadData()
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  } finally {
    submitting.value = false
  }
}

async function handleDelete(row: RoleDto) {
  const confirmed = window.confirm(`确定要删除角色「${row.name}」吗？`)

  if (!confirmed) return

  try {
    await deleteRole(row.id)
    message.success('角色已删除')
    await loadData()
  } catch (err: any) {
    message.error(err.response?.data?.message || '删除失败')
  }
}

onMounted(loadData)
</script>
