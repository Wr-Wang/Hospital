# V1 功能实现状态检查

> 检查日期：2026-05-16
> 对应需求文档：[PROJECT_PLAN.md](PROJECT_PLAN.md) §4「第一版必须实现的功能」

---

## 状态速览

| # | 模块 | 后端 API | WPF 页面 | 数据库 Schema | 领域实体 | 状态 |
|---|------|---------|----------|---------------|---------|------|
| ① | 认证与导航 | ✅ | ✅ | ✅ | — | **已完成** |
| ② | 组织主数据 | ❌ | ⚠️ 占位页 | ✅ | ❌ | **待实现** |
| ③ | 字典管理 | ❌ | ⚠️ 占位页 | ✅ | ❌ | **待实现** |
| ④ | 患者建档 | ✅ | ⚠️ 占位页 | ✅ | ✅ | **部分完成** |
| ⑤ | 挂号 | ❌ | ⚠️ 占位页 | ✅ | ❌ | **待实现** |
| ⑥ | 门诊医生站 | ❌ | ⚠️ 占位页 | ✅ | ❌ | **待实现** |
| ⑦ | 发药 | ❌ | ⚠️ 占位页 | ✅ | ❌ | **待实现** |
| ⑧ | 收费 | ❌ | ⚠️ 占位页 | ✅ | ❌ | **待实现** |
| ⑨ | 患者检索 | ⚠️ 基础 | ⚠️ 占位页 | ✅ | ✅ | **部分完成** |
| ⑩ | 用户与权限 | ❌ | ⚠️ 占位页 | ✅ | ❌ | **待实现** |

---

## 详细检查

### ① 认证与导航 — ✅ 已完成

**后端：**
- [x] `AuthenticationController` — login / logout API
- [x] JWT Token 生成 (`JwtTokenService`)
- [x] JWT 认证中间件配置
- [x] 种子用户数据（admin / doctor 等）

**WPF：**
- [x] `LoginWindow` — 用户名密码登录界面
- [x] `MainWindow` — 侧边栏导航 + 内容区
- [x] `NavigationService` — 路由注册与导航
- [x] `LoginViewModel` — 登录逻辑、Token 存储
- [x] `ApplicationContext` — 用户上下文（单例）
- [x] 登出重新登录流程

**数据库：**
- [x] `003_sec_security.sql` — 用户表、种子数据

---

### ② 组织主数据 — ❌ 待实现

**后端：**
- [ ] 院区管理 API（CURD）
- [ ] 科室树管理 API
- [ ] 人员档案 API

**WPF：**
- [ ] `mdm.campus` — 仅占位页
- [ ] `mdm.dept` — 仅占位页
- [ ] `mdm.staff` — 仅占位页

**领域：**
- [ ] `Campus` 实体
- [ ] `Department` 实体（树结构）
- [ ] `Staff` 实体

**数据库：**
- [x] `001_mdm_organization.sql` — 院区/科室/人员表已定义

**路由已注册：** `mdm.campus`、`mdm.dept`、`mdm.staff`

---

### ③ 字典管理 — ❌ 待实现

**后端：**
- [ ] 字典类型管理 API
- [ ] 字典项 CRUD API

**WPF：**
- [ ] `mdm.dict` — 仅占位页

**领域：**
- [ ] `Dictionary` / `DictionaryItem` 实体

**数据库：**
- [x] `002_mdm_dictionary.sql` — 字典表已定义

**路由已注册：** `mdm.dict`

---

### ④ 患者建档 — ⚠️ 部分完成

**后端：**
- [x] `PatientController` — CRUD 端点
- [x] `PatientApplicationService`
- [x] `PatientRepository`（内存模拟，待集成 EF Core）

**WPF：**
- [ ] `pat.register` — 仅占位页（未实现 UI）

**领域：**
- [x] `Patient` 聚合根
- [x] `PatientIdentifier` 子实体
- [x] `PatientConsent` 子实体
- [x] `Gender` / `IdCard` / `PhoneNumber` 值对象

**数据库：**
- [x] `004_pat_empi.sql` — 患者表已定义

> **备注：** 后端 CRUD 已实现但 WPF 无 UI 界面，Repository 为内存实现。

---

### ⑤ 挂号 — ❌ 待实现

**后端：**
- [ ] 号源排班管理 API
- [ ] 窗口挂号 API
- [ ] 退号 API

**WPF：**
- [ ] `opd.schedule` — 仅占位页
- [ ] `opd.register` — 仅占位页

**领域：**
- [ ] `Schedule` / `Registration` 实体

**数据库：**
- [x] `005_opd_schedule.sql` — 排班号表
- [x] `006_opd_registration.sql` — 挂号表

**路由已注册：** `opd.schedule`、`opd.register`

---

### ⑥ 门诊医生站 — ❌ 待实现

**后端：**
- [ ] 病历书写与保存 API
- [ ] 诊断开立 API
- [ ] 处方开立 API
- [ ] 检验/检查申请 API
- [ ] 转诊住院申请 API

**WPF：**
- [ ] `opd.encounter` — 仅占位页

**领域：**
- [ ] `Encounter` / `MedicalRecord` / `Prescription` / `Order` 等实体

**数据库：**
- [x] `007_clinical_encounter.sql` — 就诊/病历/诊断表
- [x] `008_clinical_orders_split.sql` — 医嘱/处方/申请单表

**路由已注册：** `opd.encounter`

---

### ⑦ 发药 — ❌ 待实现

**后端：**
- [ ] 处方审核 API
- [ ] 发药 API
- [ ] 退药 API
- [ ] 管控药双人核对 API

**WPF：**
- [ ] `pha.dispense` — 仅占位页

**领域：**
- [ ] `DrugDispense` / `Inventory` 等实体

**数据库：**
- [x] `009_pha_drug.sql` — 药品/库存/出入库表

**路由已注册：** `pha.dispense`

---

### ⑧ 收费 — ❌ 待实现

**后端：**
- [ ] 费用收取 API
- [ ] 退费 API

**WPF：**
- [ ] `fin.cash` — 仅占位页

**领域：**
- [ ] `Billing` / `Payment` 等实体

**数据库：**
- [x] `014_fin_billing.sql` — 账单/明细/支付记录表

**路由已注册：** `fin.cash`

---

### ⑨ 患者检索 — ⚠️ 部分完成

**后端：**
- [x] GET `/api/patient/{id}` — 按 ID 查询
- [x] GET `/api/patient/by-patient-no/{patientNo}` — 按病历号查询
- [ ] 模糊搜索 API（姓名/身份证/手机号）
- [ ] 分页查询 API
- [ ] 患者 360 视图 API

**WPF：**
- [ ] `pat.search` — 仅占位页
- [ ] `pat.360` — 未注册路由

> **备注：** 后端仅有基础按 ID/病历号查询，缺少 WPF UI。

---

### ⑩ 用户与权限 — ❌ 待实现

**后端：**
- [ ] 用户管理 API
- [ ] 角色管理 API
- [ ] 权限配置 API
- [ ] 审计日志 API

**WPF：**
- [ ] `sys.userrole` — 仅占位页

**领域：**
- [ ] `User` / `Role` / `Permission` 实体

**数据库：**
- [x] `003_sec_security.sql` — 用户/角色/权限表已定义，包含种子数据

**备注：**
- JWT Token 中已包含角色 claims ✅
- 缺少角色管理界面和 RBAC 配置界面

**路由已注册：** `sys.userrole`

---

## 总结

### 已完成的模块
- **认证与导航**（V1 ①）— 前后端均可工作

### 部分完成的模块
- **患者建档 / 检索**（V1 ④⑨）— 后端 API 和领域模型已实现，WPF 缺少 UI 界面

### 完全未实现的模块
- **组织主数据**（V1 ②）— 后端 API + 前端页面均缺失
- **字典管理**（V1 ③）— 同上
- **挂号**（V1 ⑤）— 同上
- **门诊医生站**（V1 ⑥）— 同上
- **发药**（V1 ⑦）— 同上
- **收费**（V1 ⑧）— 同上
- **用户与权限**（V1 ⑩）— 同上

### 总体进度
- 后端 API：约 **15%** 完成（仅认证 + 患者基础 CRUD）
- WPF 页面：约 **5%** 完成（仅登录 + 主框架 + 占位页）
- 领域模型：约 **10%** 完成（仅 Patient 边界上下文）
- 数据库 Schema：**100%** 完成（15 个模块的建表脚本均已定义）
