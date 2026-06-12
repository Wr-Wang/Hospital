// 模拟数据（后续替换为真实 API 调用）

function generateDeptId(index) {
  return `dept_${String(index).padStart(3, '0')}`
}

function generateStaffId(index) {
  return `staff_${String(index).padStart(3, '0')}`
}

// 院区数据
const CAMPUSES = [
  { id: 'campus_001', name: '总院区', address: '市中心大道100号' },
  { id: 'campus_002', name: '东院区', address: '东城区健康路66号' }
]

// 科室数据（带树形结构）
const DEPARTMENTS = [
  // 内科系列
  { id: generateDeptId(1), name: '内科', parentId: null, campusId: 'campus_001', type: 2 },
  { id: generateDeptId(2), name: '呼吸内科', parentId: generateDeptId(1), campusId: 'campus_001', type: 2 },
  { id: generateDeptId(3), name: '消化内科', parentId: generateDeptId(1), campusId: 'campus_001', type: 2 },
  { id: generateDeptId(4), name: '心血管内科', parentId: generateDeptId(1), campusId: 'campus_001', type: 2 },
  // 外科系列
  { id: generateDeptId(5), name: '外科', parentId: null, campusId: 'campus_001', type: 2 },
  { id: generateDeptId(6), name: '普外科', parentId: generateDeptId(5), campusId: 'campus_001', type: 2 },
  { id: generateDeptId(7), name: '骨科', parentId: generateDeptId(5), campusId: 'campus_001', type: 2 },
  // 其他
  { id: generateDeptId(8), name: '儿科', parentId: null, campusId: 'campus_001', type: 2 },
  { id: generateDeptId(9), name: '妇产科', parentId: null, campusId: 'campus_001', type: 2 },
  { id: generateDeptId(10), name: '眼科', parentId: null, campusId: 'campus_001', type: 2 },
  { id: generateDeptId(11), name: '耳鼻喉科', parentId: null, campusId: 'campus_001', type: 2 },
  { id: generateDeptId(12), name: '皮肤科', parentId: null, campusId: 'campus_001', type: 2 },
  { id: generateDeptId(13), name: '口腔科', parentId: null, campusId: 'campus_001', type: 2 },
  { id: generateDeptId(14), name: '中医科', parentId: null, campusId: 'campus_001', type: 2 },
  // 东院区科室
  { id: generateDeptId(15), name: '内科(东院)', parentId: null, campusId: 'campus_002', type: 2 },
  { id: generateDeptId(16), name: '儿科(东院)', parentId: null, campusId: 'campus_002', type: 2 }
]

// 医生数据
const DOCTORS = [
  { id: generateStaffId(1), name: '张明', title: '主任医师', deptId: generateDeptId(2), deptName: '呼吸内科', campusId: 'campus_001', avatar: '', desc: '擅长呼吸系统常见病、多发病的诊治，对慢性阻塞性肺疾病、哮喘有丰富经验' },
  { id: generateStaffId(2), name: '李芳', title: '副主任医师', deptId: generateDeptId(2), deptName: '呼吸内科', campusId: 'campus_001', avatar: '', desc: '擅长肺部感染性疾病、支气管哮喘的诊治' },
  { id: generateStaffId(3), name: '王强', title: '主任医师', deptId: generateDeptId(3), deptName: '消化内科', campusId: 'campus_001', avatar: '', desc: '擅长胃肠疾病、肝胆胰疾病的诊治，熟练掌握消化内镜技术' },
  { id: generateStaffId(4), name: '赵敏', title: '副主任医师', deptId: generateDeptId(3), deptName: '消化内科', campusId: 'campus_001', avatar: '', desc: '擅长慢性胃炎、消化性溃疡、功能性胃肠病的诊治' },
  { id: generateStaffId(5), name: '刘伟', title: '主任医师', deptId: generateDeptId(4), deptName: '心血管内科', campusId: 'campus_001', avatar: '', desc: '擅长高血压、冠心病、心力衰竭等心血管疾病的诊治' },
  { id: generateStaffId(6), name: '陈静', title: '主治医师', deptId: generateDeptId(6), deptName: '普外科', campusId: 'campus_001', avatar: '', desc: '擅长甲状腺、乳腺、胃肠等普外科疾病的诊治' },
  { id: generateStaffId(7), name: '周涛', title: '主任医师', deptId: generateDeptId(7), deptName: '骨科', campusId: 'campus_001', avatar: '', desc: '擅长创伤骨折、关节置换、脊柱疾病的诊治' },
  { id: generateStaffId(8), name: '吴丽', title: '副主任医师', deptId: generateDeptId(8), deptName: '儿科', campusId: 'campus_001', avatar: '', desc: '擅长儿童呼吸系统、消化系统疾病的诊治' },
  { id: generateStaffId(9), name: '孙磊', title: '主任医师', deptId: generateDeptId(9), deptName: '妇产科', campusId: 'campus_001', avatar: '', desc: '擅长妇科肿瘤、妇科内分泌疾病的诊治' },
  { id: generateStaffId(10), name: '黄娟', title: '副主任医师', deptId: generateDeptId(10), deptName: '眼科', campusId: 'campus_001', avatar: '', desc: '擅长白内障、青光眼、眼底病的诊治' }
]

// 时段定义
const TIME_SLOTS = [
  { id: 'slot_1', name: '上午第一时段', startTime: '08:00', endTime: '09:00' },
  { id: 'slot_2', name: '上午第二时段', startTime: '09:00', endTime: '10:00' },
  { id: 'slot_3', name: '上午第三时段', startTime: '10:00', endTime: '11:00' },
  { id: 'slot_4', name: '上午第四时段', startTime: '11:00', endTime: '12:00' },
  { id: 'slot_5', name: '下午第一时段', startTime: '14:00', endTime: '15:00' },
  { id: 'slot_6', name: '下午第二时段', startTime: '15:00', endTime: '16:00' },
  { id: 'slot_7', name: '下午第三时段', startTime: '16:00', endTime: '17:00' }
]

// 生成排班数据
function generateMockSchedules(deptId, doctorId) {
  const now = new Date()
  const schedules = []
  for (let i = 0; i < 7; i++) {
    const date = new Date(now)
    date.setDate(now.getDate() + i)
    // 周末减半
    const dayOfWeek = date.getDay()
    if (dayOfWeek === 0) continue // 周日休息

    const slots = dayOfWeek === 6 ? TIME_SLOTS.slice(0, 2) : TIME_SLOTS.slice(0, 4)
    const dateStr = date.toISOString().split('T')[0]

    schedules.push({
      scheduleId: `schedule_${dateStr}_${doctorId}`,
      doctorId,
      deptId,
      date: dateStr,
      dayOfWeek: ['周日', '周一', '周二', '周三', '周四', '周五', '周六'][dayOfWeek],
      slots: slots.map(s => ({
        ...s,
        totalQuota: 20,
        bookedQuota: Math.floor(Math.random() * 15),
        availableQuota: Math.max(0, 20 - Math.floor(Math.random() * 15))
      }))
    })
  }
  return schedules
}

// 生成预约记录
function generateMockAppointments(patientId) {
  const now = new Date()
  return [
    {
      id: 'apt_001',
      patientId,
      patientName: '张三',
      doctorName: '张明',
      doctorTitle: '主任医师',
      deptName: '呼吸内科',
      campusName: '总院区',
      date: '2026-05-26',
      timeSlot: '08:00-09:00',
      queueNumber: 5,
      status: 1,  // 待就诊
      statusText: '待就诊',
      createTime: '2026-05-25 10:30:00'
    },
    {
      id: 'apt_002',
      patientId,
      patientName: '张三',
      doctorName: '王强',
      doctorTitle: '主任医师',
      deptName: '消化内科',
      campusName: '总院区',
      date: '2026-05-24',
      timeSlot: '09:00-10:00',
      queueNumber: 12,
      status: 2,  // 已就诊
      statusText: '已就诊',
      createTime: '2026-05-23 08:15:00'
    },
    {
      id: 'apt_003',
      patientId,
      patientName: '张三',
      doctorName: '李芳',
      doctorTitle: '副主任医师',
      deptName: '呼吸内科',
      campusName: '总院区',
      date: '2026-05-22',
      timeSlot: '14:00-15:00',
      queueNumber: 3,
      status: 2,
      statusText: '已就诊',
      createTime: '2026-05-21 09:00:00'
    },
    {
      id: 'apt_004',
      patientId,
      patientName: '张三',
      doctorName: '陈静',
      doctorTitle: '主治医师',
      deptName: '普外科',
      campusName: '总院区',
      date: '2026-05-20',
      timeSlot: '10:00-11:00',
      queueNumber: 8,
      status: 3,  // 已取消
      statusText: '已取消',
      createTime: '2026-05-19 14:20:00'
    }
  ]
}

// 获取排队的模拟数据
function generateMockQueue() {
  return {
    department: '呼吸内科',
    doctor: '张明',
    date: '2026-05-25',
    currentNumber: 5,
    myNumber: 8,
    waitingCount: 3,
    estimatedWait: '约 15 分钟',
    status: 'waiting' // waiting / consulting / completed
  }
}

// 获取已保存的或初始化就诊人
function getPatients() {
  const app = getApp()
  if (app.globalData.patients.length > 0) return app.globalData.patients

  const defaultPatients = [
    { id: 'pat_001', name: '张三', idCard: '110101199001011234', phone: '13812345678', gender: 1, isDefault: true },
    { id: 'pat_002', name: '李四', idCard: '110101198505152345', phone: '13987654321', gender: 2, isDefault: false }
  ]
  app.globalData.patients = defaultPatients
  return defaultPatients
}

module.exports = {
  CAMPUSES,
  DEPARTMENTS,
  DOCTORS,
  TIME_SLOTS,
  generateMockSchedules,
  generateMockAppointments,
  generateMockQueue,
  getPatients
}
