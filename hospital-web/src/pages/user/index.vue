<template>
  <div>
    <n-page-header subtitle="用户">
      <template #title>
        <n-h2 style="margin: 0">用户管理</n-h2>
      </template>
      <template #extra>
        <n-button type="primary" @click="openCreateModal">新建用户</n-button>
      </template>
    </n-page-header>

    <n-card style="margin-top: 16px">
      <n-data-table
        :columns="columns"
        :data="userList"
        :loading="loading"
        :bordered="true"
        :paginate="false"
      />
    </n-card>

    <!-- 新建/编辑弹窗 -->
    <n-modal v-model:show="showModal" :title="isEditing ? '编辑用户' : '新建用户'" preset="card" style="width: 520px">
      <n-form ref="formRef" :model="form" :rules="rules" label-placement="left" label-width="90px">
        <n-form-item path="loginName" label="登录名" v-if="!isEditing">
          <n-input v-model:value="form.loginName" placeholder="登录账号" :disabled="isEditing" />
        </n-form-item>
        <n-form-item :path="isEditing ? 'newPassword' : 'password'" :label="isEditing ? '新密码' : '密码'">
          <n-input
            v-if="!isEditing"
            v-model:value="form.password"
            type="password"
            placeholder="登录密码"
          />
          <n-input
            v-else
            v-model:value="form.newPassword"
            type="password"
            placeholder="留空则不修改"
          />
        </n-form-item>
        <n-form-item path="displayName" label="显示名">
          <n-input v-model:value="form.displayName" placeholder="用户显示名称" />
        </n-form-item>
        <n-form-item path="campusName" label="院区">
          <n-select v-model:value="form.campusName" :options="campusNameOptions" placeholder="选择院区" filterable />
        </n-form-item>
        <n-form-item path="roles" label="角色">
          <n-select v-model:value="form.roles" :options="roleOptions" placeholder="选择角色" multiple />
        </n-form-item>
        <n-form-item label="启用" v-if="isEditing">
          <n-switch v-model:value="form.isActive" />
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
import { useMessage, NButton, NTag, NSpace, NPageHeader, NCard, NSwitch } from 'naive-ui'
import type { DataTableColumns, FormRules, FormInst, SelectOption } from 'naive-ui'
import type { UserDto, RoleDto } from '../../types'
import { getUsers, createUser, updateUser } from '../../api/userRole'
import { getRoles } from '../../api/userRole'
import { getCampusList } from '../../api/campus'

const message = useMessage()
const formRef = ref<FormInst | null>(null)

const loading = ref(false)
const submitting = ref(false)
const showModal = ref(false)
const isEditing = ref(false)
const editingId = ref<number | null>(null)

const userList = ref<UserDto[]>([])
const roleList = ref<RoleDto[]>([])
const campusNameList = ref<string[]>([])

const roleOptions = computed<SelectOption[]>(() =>
  roleList.value.map((r) => ({ label: `${r.name} — ${r.description}`, value: r.name })),
)

const campusNameOptions = computed<SelectOption[]>(() =>
  campusNameList.value.map((c) => ({ label: c, value: c })),
)

const form = reactive({
  loginName: '',
  password: '',
  newPassword: '',
  displayName: '',
  campusName: '',
  roles: [] as string[],
  isActive: true,
})

const rules: FormRules = {
  loginName: [{ required: true, message: '请输入登录名', trigger: 'blur' }],
  displayName: [{ required: true, message: '请输入显示名', trigger: 'blur' }],
  campusName: [{ required: true, message: '请选择院区', trigger: 'change' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
}

const columns: DataTableColumns<UserDto> = [
  { title: '登录名', key: 'loginName', width: 100 },
  { title: '显示名', key: 'displayName', width: 120 },
  { title: '院区', key: 'campusName', width: 100 },
  {
    title: '角色',
    key: 'roles',
    width: 200,
    render(row) {
      return h(NSpace, { size: 'small' }, {
        default: () => row.roles.map((r) => h(NTag, { size: 'small', key: r }, { default: () => r })),
      })
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
    width: 100,
    render(row) {
      return h(NButton, { size: 'small', quaternary: true, onClick: () => openEditModal(row) }, { default: () => '编辑' })
    },
  },
]

// ===== 数据加载 =====

async function loadUsers() {
  loading.value = true
  try {
    const res = await getUsers()
    userList.value = res.data
  } catch (err: any) {
    message.error(err.response?.data?.message || '加载用户列表失败')
  } finally {
    loading.value = false
  }
}

async function loadRoles() {
  try {
    const res = await getRoles()
    roleList.value = res.data
  } catch { /* ignore */ }
}

async function loadCampuses() {
  try {
    const res = await getCampusList()
    campusNameList.value = res.data.map((c: { name: string }) => c.name)
  } catch { /* ignore */ }
}

// ===== 弹窗 =====

function openCreateModal() {
  isEditing.value = false
  editingId.value = null
  form.loginName = ''
  form.password = ''
  form.newPassword = ''
  form.displayName = ''
  form.campusName = ''
  form.roles = []
  form.isActive = true
  showModal.value = true
}

function openEditModal(row: UserDto) {
  isEditing.value = true
  editingId.value = row.id
  form.loginName = row.loginName
  form.password = ''
  form.newPassword = ''
  form.displayName = row.displayName
  form.campusName = row.campusName
  form.roles = [...row.roles]
  form.isActive = row.isActive
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
      await updateUser(editingId.value, {
        displayName: form.displayName,
        roles: form.roles,
        isActive: form.isActive,
        ...(form.newPassword ? { password: form.newPassword } : {}),
      })
      message.success('用户已更新')
    } else {
      await createUser({
        loginName: form.loginName,
        password: form.password,
        displayName: form.displayName,
        campusName: form.campusName,
        roles: form.roles,
      })
      message.success('用户已创建')
    }
    showModal.value = false
    await loadUsers()
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  } finally {
    submitting.value = false
  }
}

onMounted(async () => {
  await Promise.all([loadUsers(), loadRoles(), loadCampuses()])
})
</script>
