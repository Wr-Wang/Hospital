<template>
  <div>
    <n-page-header subtitle="患者">
      <template #title>
        <n-h2 style="margin: 0">患者建档</n-h2>
      </template>
    </n-page-header>

    <n-card style="margin-top: 16px; max-width: 640px">
      <n-form ref="formRef" :model="form" :rules="rules" label-placement="left" label-width="100px">
        <n-form-item path="patientNo" label="病历号">
          <n-input v-model:value="form.patientNo" placeholder="病历号，如 P20250001" />
        </n-form-item>
        <n-form-item path="name" label="姓名">
          <n-input v-model:value="form.name" placeholder="患者姓名" @blur="onNameBlur" />
        </n-form-item>
        <n-form-item path="gender" label="性别">
          <n-select v-model:value="form.gender" :options="genderOptions" placeholder="选择性别" clearable />
        </n-form-item>
        <n-form-item path="birthDate" label="出生日期">
          <n-date-picker v-model:value="form.birthDateTs" type="date" placeholder="选择日期" clearable style="width: 100%" />
        </n-form-item>
        <n-form-item path="phone" label="电话">
          <n-input v-model:value="form.phone" placeholder="联系电话" @blur="onPhoneBlur" />
        </n-form-item>
        <n-form-item path="idCard" label="身份证号">
          <n-input v-model:value="form.idCard" placeholder="18 位身份证号码" />
        </n-form-item>
        <n-form-item path="allergiesText" label="过敏史">
          <n-input v-model:value="form.allergiesText" type="textarea" rows="2" placeholder="药物/物质过敏记录" />
        </n-form-item>
      </n-form>

      <!-- 疑似重复提醒 -->
      <n-alert v-if="duplicates.length > 0" type="warning" title="疑似重复患者" :closable="false" style="margin-bottom: 16px">
        <div v-for="d in duplicates" :key="d.id" style="margin-bottom: 4px">
          {{ d.patientNo }} · {{ d.name }} · {{ d.gender || '-' }} · {{ d.phone || '-' }}
          <n-button size="tiny" quaternary type="primary" @click="viewDuplicate(d.id)">查看</n-button>
        </div>
      </n-alert>

      <n-space justify="end">
        <n-button @click="handleReset">重置</n-button>
        <n-button type="primary" :loading="submitting" @click="handleSubmit">保存</n-button>
      </n-space>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useMessage } from 'naive-ui'
import type { FormRules, FormInst, SelectOption } from 'naive-ui'
import type { PatientDto } from '../../types'
import { createPatient, getSuspectDuplicates } from '../../api/patient'

const router = useRouter()
const message = useMessage()
const formRef = ref<FormInst | null>(null)
const submitting = ref(false)
const duplicates = ref<PatientDto[]>([])

const genderOptions: SelectOption[] = [
  { label: '男', value: 'Male' },
  { label: '女', value: 'Female' },
]

const form = reactive({
  patientNo: '',
  name: '',
  gender: null as string | null,
  birthDateTs: null as number | null,
  phone: '',
  idCard: '',
  allergiesText: '',
})

const rules: FormRules = {
  patientNo: [{ required: true, message: '请输入病历号', trigger: 'blur' }],
  name: [{ required: true, message: '请输入患者姓名', trigger: 'blur' }],
}

let duplicateCheckTimer: ReturnType<typeof setTimeout> | null = null

function debouncedDuplicateCheck() {
  if (duplicateCheckTimer) clearTimeout(duplicateCheckTimer)
  if (!form.name.trim()) {
    duplicates.value = []
    return
  }
  duplicateCheckTimer = setTimeout(async () => {
    try {
      const res = await getSuspectDuplicates(form.name.trim(), form.phone.trim() || undefined)
      duplicates.value = res.data
    } catch {
      // 静默失败，不阻塞操作
    }
  }, 500)
}

function onNameBlur() {
  debouncedDuplicateCheck()
}

function onPhoneBlur() {
  if (form.name.trim()) debouncedDuplicateCheck()
}

function viewDuplicate(id: number) {
  router.push(`/patient/detail/${id}`)
}

function handleReset() {
  form.patientNo = ''
  form.name = ''
  form.gender = null
  form.birthDateTs = null
  form.phone = ''
  form.idCard = ''
  form.allergiesText = ''
  duplicates.value = []
}

async function handleSubmit() {
  try {
    await formRef.value?.validate()
  } catch {
    return
  }

  submitting.value = true
  try {
    const birthDate = form.birthDateTs
      ? new Date(form.birthDateTs).toISOString().split('T')[0]
      : undefined

    const res = await createPatient({
      patientNo: form.patientNo,
      name: form.name,
      gender: form.gender || undefined,
      birthDate,
      phone: form.phone || undefined,
      idCard: form.idCard || undefined,
      allergiesText: form.allergiesText || undefined,
    })
    message.success('患者已创建', { duration: 3000 })
    // 跳转到详情页
    router.push(`/patient/detail/${res.data.id}`)
  } catch (err: any) {
    message.error(err.response?.data?.message || '创建失败')
  } finally {
    submitting.value = false
  }
}
</script>
