<template>
  <div>
    <n-page-header subtitle="字典">
      <template #title>
        <n-h2 style="margin: 0">字典管理</n-h2>
      </template>
      <template #extra>
        <n-button type="primary" @click="openCreateTypeModal">新建字典类型</n-button>
      </template>
    </n-page-header>

    <!-- 字典类型列表 -->
    <n-card title="字典类型" size="small" style="margin-top: 16px">
      <n-data-table
        :columns="typeColumns"
        :data="typeList"
        :loading="loadingTypes"
        :bordered="true"
        :paginate="false"
        :max-height="250"
      />
    </n-card>

    <!-- 字典项列表（选中类型后显示） -->
    <n-card v-if="selectedType" :title="`字典项 — ${selectedType.name}`" size="small" style="margin-top: 16px">
      <template #header-extra>
        <n-button size="small" type="primary" @click="openCreateItemModal">新建字典项</n-button>
      </template>
      <n-data-table
        :columns="itemColumns"
        :data="itemList"
        :loading="loadingItems"
        :bordered="true"
        :paginate="false"
      />
    </n-card>

    <!-- 字典类型弹窗 -->
    <n-modal v-model:show="showTypeModal" :title="isEditingType ? '编辑字典类型' : '新建字典类型'" preset="card" style="width: 520px">
      <n-form ref="typeFormRef" :model="typeForm" :rules="typeRules" label-placement="left" label-width="80px">
        <n-form-item path="code" label="编码" v-if="!isEditingType">
          <n-input v-model:value="typeForm.code" placeholder="类型编码" :disabled="isEditingType" />
        </n-form-item>
        <n-form-item path="name" label="名称">
          <n-input v-model:value="typeForm.name" placeholder="类型名称" />
        </n-form-item>
        <n-form-item path="description" label="说明">
          <n-input v-model:value="typeForm.description" placeholder="类型说明" type="textarea" rows="2" />
        </n-form-item>
      </n-form>
      <template #footer>
        <n-space justify="end">
          <n-button @click="showTypeModal = false">取消</n-button>
          <n-button type="primary" :loading="submitting" @click="handleTypeSubmit">保存</n-button>
        </n-space>
      </template>
    </n-modal>

    <!-- 字典项弹窗 -->
    <n-modal v-model:show="showItemModal" :title="isEditingItem ? '编辑字典项' : '新建字典项'" preset="card" style="width: 520px">
      <n-form ref="itemFormRef" :model="itemForm" :rules="itemRules" label-placement="left" label-width="80px">
        <n-form-item path="code" label="编码" v-if="!isEditingItem">
          <n-input v-model:value="itemForm.code" placeholder="项编码" :disabled="isEditingItem" />
        </n-form-item>
        <n-form-item path="name" label="名称">
          <n-input v-model:value="itemForm.name" placeholder="项名称" />
        </n-form-item>
        <n-form-item path="sortOrder" label="排序">
          <n-input-number v-model:value="itemForm.sortOrder" :min="0" :max="9999" style="width: 100%" />
        </n-form-item>
        <n-form-item path="parentId" label="上级项">
          <n-tree-select
            v-model:value="itemForm.parentId"
            :options="itemParentOptions"
            placeholder="无（顶级项）"
            clearable
            key-field="id"
            label-field="name"
          />
        </n-form-item>
      </n-form>
      <template #footer>
        <n-space justify="end">
          <n-button @click="showItemModal = false">取消</n-button>
          <n-button type="primary" :loading="submitting" @click="handleItemSubmit">保存</n-button>
        </n-space>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, h, computed, onMounted } from 'vue'
import { useMessage, NButton, NTag, NSpace, NPageHeader, NCard } from 'naive-ui'
import type { DataTableColumns, FormRules, FormInst, TreeSelectOption } from 'naive-ui'
import type { DictionaryTypeDto, DictionaryItemDto } from '../../types'
import {
  getDictionaryTypes,
  createDictionaryType,
  updateDictionaryType,
  activateDictionaryType,
  deactivateDictionaryType,
  getDictionaryItems,
  createDictionaryItem,
  updateDictionaryItem,
  activateDictionaryItem,
  deactivateDictionaryItem,
} from '../../api/dictionary'

const message = useMessage()
const typeFormRef = ref<FormInst | null>(null)
const itemFormRef = ref<FormInst | null>(null)

const loadingTypes = ref(false)
const loadingItems = ref(false)
const submitting = ref(false)

const showTypeModal = ref(false)
const isEditingType = ref(false)
const editingTypeId = ref<number | null>(null)
const typeList = ref<DictionaryTypeDto[]>([])

const showItemModal = ref(false)
const isEditingItem = ref(false)
const editingItemId = ref<number | null>(null)
const selectedType = ref<DictionaryTypeDto | null>(null)
const itemList = ref<DictionaryItemDto[]>([])

const typeForm = reactive({
  code: '',
  name: '',
  description: '',
})

const typeRules: FormRules = {
  code: [{ required: true, message: '请输入类型编码', trigger: 'blur' }],
  name: [{ required: true, message: '请输入类型名称', trigger: 'blur' }],
}

const itemForm = reactive({
  code: '',
  name: '',
  sortOrder: 0,
  parentId: null as number | null,
})

const itemRules: FormRules = {
  code: [{ required: true, message: '请输入项编码', trigger: 'blur' }],
  name: [{ required: true, message: '请输入项名称', trigger: 'blur' }],
}

const itemParentOptions = computed<TreeSelectOption[]>(() => {
  function buildOptions(items: DictionaryItemDto[]): TreeSelectOption[] {
    return items
      .filter((d) => !isEditingItem.value || d.id !== editingItemId.value)
      .map((d) => ({
        id: d.id,
        name: d.name,
        label: d.name,
        value: d.id,
      }))
  }
  return buildOptions(itemList.value)
})

const typeColumns: DataTableColumns<DictionaryTypeDto> = [
  { title: '编码', key: 'code', width: 150 },
  { title: '名称', key: 'name', width: 200 },
  { title: '说明', key: 'description', ellipsis: { tooltip: true } },
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
    width: 200,
    render(row) {
      return h(NSpace, { size: 'small' }, {
        default: () => [
          h(NButton, { size: 'small', quaternary: true, onClick: () => { selectType(row); openEditTypeModal(row) } }, { default: () => '编辑' }),
          h(NButton, {
            size: 'small',
            quaternary: true,
            type: row.isActive ? 'warning' : 'success',
            onClick: () => toggleTypeActive(row),
          }, { default: () => row.isActive ? '停用' : '启用' }),
        ],
      })
    },
  },
]

const itemColumns: DataTableColumns<DictionaryItemDto> = [
  { title: '编码', key: 'code', width: 150 },
  { title: '名称', key: 'name', width: 200 },
  {
    title: '排序',
    key: 'sortOrder',
    width: 80,
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
    width: 200,
    render(row) {
      return h(NSpace, { size: 'small' }, {
        default: () => [
          h(NButton, { size: 'small', quaternary: true, onClick: () => openEditItemModal(row) }, { default: () => '编辑' }),
          h(NButton, {
            size: 'small',
            quaternary: true,
            type: row.isActive ? 'warning' : 'success',
            onClick: () => toggleItemActive(row),
          }, { default: () => row.isActive ? '停用' : '启用' }),
        ],
      })
    },
  },
]

async function loadTypes() {
  loadingTypes.value = true
  try {
    const res = await getDictionaryTypes()
    typeList.value = res.data
  } catch (err: any) {
    message.error(err.response?.data?.message || '加载字典类型失败')
  } finally {
    loadingTypes.value = false
  }
}

async function loadItems(typeId: number) {
  loadingItems.value = true
  try {
    const res = await getDictionaryItems(typeId)
    itemList.value = res.data
  } catch (err: any) {
    message.error(err.response?.data?.message || '加载字典项失败')
  } finally {
    loadingItems.value = false
  }
}

function selectType(row: DictionaryTypeDto) {
  selectedType.value = row
  loadItems(row.id)
}

// ===== 类型 CRUD =====

function openCreateTypeModal() {
  isEditingType.value = false
  editingTypeId.value = null
  typeForm.code = ''
  typeForm.name = ''
  typeForm.description = ''
  showTypeModal.value = true
}

function openEditTypeModal(row: DictionaryTypeDto) {
  // selectType is called before this in the click handler
  isEditingType.value = true
  editingTypeId.value = row.id
  typeForm.code = row.code
  typeForm.name = row.name
  typeForm.description = row.description || ''
  showTypeModal.value = true
}

async function handleTypeSubmit() {
  try {
    await typeFormRef.value?.validate()
  } catch {
    return
  }

  submitting.value = true
  try {
    if (isEditingType.value && editingTypeId.value) {
      await updateDictionaryType(editingTypeId.value, { name: typeForm.name, description: typeForm.description || undefined })
      message.success('字典类型已更新')
    } else {
      await createDictionaryType({ code: typeForm.code, name: typeForm.name, description: typeForm.description || undefined })
      message.success('字典类型已创建')
    }
    showTypeModal.value = false
    await loadTypes()
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  } finally {
    submitting.value = false
  }
}

async function toggleTypeActive(row: DictionaryTypeDto) {
  try {
    if (row.isActive) {
      await deactivateDictionaryType(row.id)
      message.success('字典类型已停用')
    } else {
      await activateDictionaryType(row.id)
      message.success('字典类型已启用')
    }
    await loadTypes()
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  }
}

// ===== 项 CRUD =====

function openCreateItemModal() {
  isEditingItem.value = false
  editingItemId.value = null
  itemForm.code = ''
  itemForm.name = ''
  itemForm.sortOrder = 0
  itemForm.parentId = null
  showItemModal.value = true
}

function openEditItemModal(row: DictionaryItemDto) {
  isEditingItem.value = true
  editingItemId.value = row.id
  itemForm.code = row.code
  itemForm.name = row.name
  itemForm.sortOrder = row.sortOrder
  itemForm.parentId = row.parentId ?? null
  showItemModal.value = true
}

async function handleItemSubmit() {
  try {
    await itemFormRef.value?.validate()
  } catch {
    return
  }

  if (!selectedType.value) return

  submitting.value = true
  try {
    if (isEditingItem.value && editingItemId.value) {
      await updateDictionaryItem(editingItemId.value, { name: itemForm.name, parentId: itemForm.parentId, sortOrder: itemForm.sortOrder })
      message.success('字典项已更新')
    } else {
      await createDictionaryItem({
        typeId: selectedType.value.id,
        code: itemForm.code,
        name: itemForm.name,
        parentId: itemForm.parentId,
        sortOrder: itemForm.sortOrder,
      })
      message.success('字典项已创建')
    }
    showItemModal.value = false
    await loadItems(selectedType.value.id)
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  } finally {
    submitting.value = false
  }
}

async function toggleItemActive(row: DictionaryItemDto) {
  try {
    if (row.isActive) {
      await deactivateDictionaryItem(row.id)
      message.success('字典项已停用')
    } else {
      await activateDictionaryItem(row.id)
      message.success('字典项已启用')
    }
    if (selectedType.value) {
      await loadItems(selectedType.value.id)
    }
  } catch (err: any) {
    message.error(err.response?.data?.message || '操作失败')
  }
}

onMounted(loadTypes)
</script>
