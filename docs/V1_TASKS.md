# V1 开发任务清单

> 对应 [PROJECT_PLAN.md](PROJECT_PLAN.md) §4「第一版必须实现的功能」
>
> **用途：** 每个任务可单独复制执行，完成一个勾一个。
> **约定：** `[x]` = 已完成，`[ ]` = 待完成

---

## 一、V1 全景状态表

| # | 模块 | 后端 API | WPF 页面 | 数据库 Schema | 领域实体 | 优先级 | 状态 |
|---|------|---------|----------|---------------|---------|--------|------|
| ① | 认证与导航 | ✅ | ✅ | ✅ | ✅ | P0 | **已完成** |
| ② | 组织主数据 | ❌ | ⚠️ 占位页 | ✅ | ❌ | P1 | **待实现** |
| ③ | 字典管理 | ❌ | ⚠️ 占位页 | ✅ | ❌ | P1 | **待实现** |
| ④ | 患者建档 | ✅ | ⚠️ 占位页 | ✅ | ✅ | P1 | **部分完成** |
| ⑤ | 挂号 | ❌ | ⚠️ 占位页 | ✅ | ❌ | P2 | **待实现** |
| ⑥ | 门诊医生站 | ❌ | ⚠️ 占位页 | ✅ | ❌ | P2 | **待实现** |
| ⑦ | 发药 | ❌ | ⚠️ 占位页 | ✅ | ❌ | P3 | **待实现** |
| ⑧ | 收费 | ❌ | ⚠️ 占位页 | ✅ | ❌ | P3 | **待实现** |
| ⑨ | 患者检索 | ⚠️ 基础 | ⚠️ 占位页 | ✅ | ✅ | P1 | **部分完成** |
| ⑩ | 用户与权限 | ❌ | ⚠️ 占位页 | ✅ | ❌ | P3 | **待实现** |

---

## 二、模块详情与任务清单

---

### 模块①：认证与导航

> **模块描述：** 系统的基础入口。负责用户身份验证（JWT 登录）、会话管理（Token 存储与续期）、菜单路由（侧边栏导航）、用户上下文（当前登录用户的信息与角色）。一切功能的前提。
>
> **对应的业务角色：** 全部系统用户
>
> **依赖：** 无

#### 1.1 解决方案与项目结构
- [x] 创建 DDD 分层项目（Domain / Application / Infrastructure / Api / App）
- [x] 配置项目间引用关系
- [x] 确保 `dotnet build` 0 错误 0 警告

#### 1.2 数据库与种子数据
- [x] 编写 `000_init_database.sql` 初始化脚本
- [x] 编写全部 15 个模块的建表脚本
- [x] 编写 `900_seed_minimal.sql` 种子数据（admin / doctor 等测试账号）
- [x] `003_sec_security.sql` 用户/角色/权限表定义

#### 1.3 JWT 认证
- [x] 实现 `JwtTokenService` — Token 生成 + 验签
- [x] 实现 `LocalUserStore` — 内存用户存储（种子数据）
- [x] 实现 `LocalAuthenticationService` — 本地认证校验
- [x] 实现 `AuthenticationController` — POST login / logout API
- [x] 配置 JWT 中间件（Program.cs）

#### 1.4 WPF 壳工程
- [x] 搭建 `MainWindow` — 侧边栏 + 内容区布局（260px 固定侧边栏 + 白底内容区）
- [x] 实现 `NavigationService` — 路由注册与导航
- [x] 实现 `LoginWindow` — 用户名密码登录界面
- [x] 实现 `LoginViewModel` — 登录逻辑
- [x] 实现 `ApplicationContext`（`IAppContext`）— 用户上下文单例
- [x] 配置 DI 容器（`ServiceCollectionExtensions`）
- [x] 注册全部 WPF 占位页面路由（用于演示导航切换）

#### 1.5 基础 UI 主题
- [x] 设计令牌（Design Tokens）定义在 `App.xaml`
- [x] 侧边栏深色主题 + 导航项选中态
- [x] 登录卡片浮升效果（阴影）
- [x] 登录按钮样式 + 表单输入框样式

---

### 模块②：组织主数据

> **模块描述：** 管理医疗机构的组织架构。包括院区（Campus）、科室树（Department）、人员档案（Staff）。多院区模式的基础——其他所有业务模块（挂号、就诊、收费等）都依赖组织数据来确定数据归属。科室树支持父子层级，用于权限数据域隔离和业务统计汇总。
>
> **对应的业务角色：** 信息科管理员
>
> **依赖：** 无（但其他所有模块依赖本模块）

#### 2.1 院区管理（Campus）

**业务说明：** 集团下属各院区的基础信息管理。支持多院区统一管理，每个院区有独立编码、名称、地址、联系方式。院区是数据隔离的第一级维度。

- [ ] **Domain —** 创建 `Campus` 实体（Id、Code、Name、Address、Phone、IsActive）
- [ ] **Domain —** 创建 `CampusCode` 值对象（编码格式校验）
- [ ] **Application —** 定义 `ICampusRepository`
- [ ] **Application —** 创建 `CreateCampusDto` / `UpdateCampusDto` / `CampusDto`
- [ ] **Application —** 实现 `CampusApplicationService`（CRUD + 启用/停用）
- [ ] **Infrastructure —** 实现 `CampusRepository`（EF Core 或内存模拟）
- [ ] **API —** 创建 `CampusController`（GET 列表/详情、POST 创建、PUT 更新、PATCH 启停用）
- [ ] **WPF —** 创建 `ViewModels/CampusViewModel.cs`
- [ ] **WPF —** 创建 `Views/CampusView.xaml`（院区列表 + 新增/编辑弹窗）
- [ ] **WPF —** 替换占位页：`mdm.campus` 指向真实页面
- [ ] **WPF —** 顶栏院区选择器：联动后续模块的数据域

#### 2.2 科室维护（Department）

**业务说明：** 以树形结构管理院区下的科室。科室可以嵌套（如：内科→呼吸内科→呼吸内科门诊）。科室有类型属性（门诊科室、住院科室、医技科室、行政科室）。科室停用时需级联校验是否有在院患者或未完成号源。

- [ ] **Domain —** 创建 `Department` 实体（Id、Code、Name、ParentId、CampusId、Type、IsActive）
- [ ] **Domain —** 创建 `DepartmentCode` 值对象
- [ ] **Domain —** 创建 `DepartmentType` 枚举（门诊、住院、医技、行政、药房）
- [ ] **Application —** 定义 `IDepartmentRepository`
- [ ] **Application —** 创建对应 DTO
- [ ] **Application —** 实现 `DepartmentApplicationService`（CRUD + 树查询 + 停用校验）
- [ ] **Infrastructure —** 实现 `DepartmentRepository`
- [ ] **API —** 创建 `DepartmentController`（树结构查询、CRUD）
- [ ] **WPF —** 创建 `DepartmentViewModel`
- [ ] **WPF —** 创建 `Views/DepartmentView.xaml`（TreeView 展示 + 编辑面板）
- [ ] **WPF —** 替换占位页：`mdm.dept` 路由

#### 2.3 人员档案（Staff）

**业务说明：** 维护医护人员（医生、护士、药师、医技人员）的档案信息，包括执业资质、执业范围、有效期。过期资质自动禁止高风险操作（如处方权）。人员关联到院区和科室，是排班和权限的基础。

- [ ] **Domain —** 创建 `Staff` 实体（Id、Code、Name、CampusId、DeptId、LicenseType、LicenseNo、LicenseExpiry、IsActive）
- [ ] **Domain —** 创建 `LicenseNumber` 值对象
- [ ] **Domain —** 创建 `LicenseType` 枚举（执业医师、执业护士、药师、医技）
- [ ] **Application —** 定义 `IStaffRepository`
- [ ] **Application —** 创建对应 DTO
- [ ] **Application —** 实现 `StaffApplicationService`（CRUD + 资质到期预警）
- [ ] **Infrastructure —** 实现 `StaffRepository`
- [ ] **API —** 创建 `StaffController`
- [ ] **WPF —** 创建 `StaffListViewModel`
- [ ] **WPF —** 创建 `Views/StaffListView.xaml`（列表 + 搜索 + 资质状态标识）
- [ ] **WPF —** 创建 `Views/StaffEditView.xaml`（新增/编辑表单，含资质信息）
- [ ] **WPF —** 替换占位页：`mdm.staff`、`mdm.staff.edit` 路由

---

### 模块③：字典管理

> **模块描述：** 管理系统运行所需的各类基础字典数据。字典分为字典类型（如"ICD 诊断编码"、"药品单位"、"收费项目类别"）和字典项（具体的编码-名称映射）。字典变更需留痕，历史数据保留当时的编码含义。门诊医生站、收费等模块都依赖字典数据。
>
> **对应的业务角色：** 信息科管理员
>
> **依赖：** 模块②（科室关联）

#### 3.1 字典类型与字典项管理

- [ ] **Domain —** 创建 `DictionaryType` 实体（Id、Code、Name、Description、IsActive）
- [ ] **Domain —** 创建 `DictionaryItem` 实体（Id、TypeId、Code、Name、ParentId、SortOrder、IsActive）
- [ ] **Application —** 定义 `IDictionaryRepository`
- [ ] **Application —** 创建对应 DTO
- [ ] **Application —** 实现 `DictionaryApplicationService`（类型CRUD + 项CRUD）
- [ ] **Infrastructure —** 实现 `DictionaryRepository`
- [ ] **API —** 创建 `DictionaryController`（类型管理 + 项管理 + 按类型编码查询）
- [ ] **WPF —** 创建 `DictionaryViewModel`
- [ ] **WPF —** 创建 `Views/DictionaryView.xaml`（左侧类型树 + 右侧字典项列表）
- [ ] **WPF —** 替换占位页：`mdm.dict` 路由

---

### 模块④：患者建档

> **模块描述：** 患者主索引（EMPI）的创建与管理。支持姓名、身份证号、手机号、出生日期等多方式录入。建档时自动按身份证号查重，提示疑似重复记录。患者信息是后续所有业务流程（挂号、就诊、收费、发药）的数据源头。
>
> **对应的业务角色：** 挂号员
>
> **依赖：** 模块②（院区关联）

#### 4.1 患者建档后端
- [x] **Domain —** 创建 `Patient` 聚合根（Id、PatientNo、Name、Gender、BirthDate、IdCard、Phone、Address、CampusId、CreatedAt）
- [x] **Domain —** 创建 `PatientIdentifier` 子实体（多种标识符）
- [x] **Domain —** 创建 `PatientConsent` 子实体（隐私授权）
- [x] **Domain —** 创建 `Gender` / `IdCard` / `PhoneNumber` 值对象
- [x] **Domain —** `PatientEvents` 领域事件定义
- [x] **Application —** 实现 `PatientApplicationService`
- [x] **API —** 实现 `PatientController`（GET by id / POST create / GET by patientNo）
- [x] **Infrastructure —** 实现 `PatientRepository`（内存模拟，待集成 EF Core）
- [ ] **API —** 补充身份证查重接口（GET `by-idcard/{idCard}`）
- [ ] **API —** 补充疑似重复列表接口（POST `suspect-duplicates` 按姓名+手机号模糊匹配）

#### 4.2 患者建档 WPF UI
- [ ] **WPF —** 创建 `PatientRegisterViewModel`
- [ ] **WPF —** 创建 `Views/PatientRegisterView.xaml`
- [ ] **WPF ——** 建档表单：姓名（必填）、身份证号（必填，格式校验）、手机号、性别（下拉）、出生日期（日期选择器）、地址
- [ ] **WPF ——** 身份证/手机号输入时实时查重，弹窗提示疑似重复列表
- [ ] **WPF ——** 替换占位页：`pat.register` 路由
- [ ] **WPF ——** 患者条（PatientBanner）公共组件：患者基本信息头，后续所有模块复用

---

### 模块⑤：挂号

> **模块描述：** 门诊核心入口流程。包括两个子模块：排班号表（医生排班 + 号源管理）和挂号工作台（窗口挂号 + 退号）。排班决定哪天哪个医生在哪个科室出诊、有多少号源。挂号从可用号源中选取一个时段分配给患者，生成就诊记录（Encounter）。
>
> **对应的业务角色：** 挂号员
>
> **依赖：** 模块②（院区+科室+人员）、模块④（患者建档）

#### 5.1 排班号表

- [ ] **Domain —** 创建 `Schedule` 实体（Id、DoctorId、DeptId、CampusId、ScheduleDate、TimeSlot、TotalQuota）
- [ ] **Domain —** 创建 `TimeSlot` 值对象（时段名称、开始时间、结束时间）
- [ ] **Domain —** 创建 `ScheduleStatus` 枚举（已发布、已停用、已满）
- [ ] **Application —** 定义 `IScheduleRepository`
- [ ] **Application —** 创建对应 DTO
- [ ] **Application —** 实现 `ScheduleApplicationService`（CRUD + 号源发布/停用）
- [ ] **Infrastructure —** 实现 `ScheduleRepository`
- [ ] **API —** 创建 `ScheduleController`
- [ ] **WPF —** 创建 `ScheduleViewModel`
- [ ] **WPF —** 创建 `Views/ScheduleView.xaml`（按周/月视图展示排班表 + 号源编辑）
- [ ] **WPF —** 替换占位页：`opd.schedule` 路由

#### 5.2 挂号工作台

- [ ] **Domain —** 创建 `Registration` 实体（Id、PatientId、ScheduleId、DoctorId、DeptId、CampusId、RegisterTime、Status）
- [ ] **Domain —** 创建 `RegistrationStatus` 枚举（已挂号、已就诊、已退号）
- [ ] **Application —** 定义 `IRegistrationRepository`
- [ ] **Application —** 创建对应 DTO
- [ ] **Application —** 实现 `RegistrationApplicationService`
- [ ] **Application ——** 挂号：校验号源 → 扣减号源 → 创建 Registration → 创建 Encounter（就诊记录）
- [ ] **Application ——** 退号：校验状态 → 恢复号源 → 标记退号
- [ ] **Infrastructure —** 实现 `RegistrationRepository`
- [ ] **API —** 创建 `RegistrationController`
- [ ] **WPF —** 创建 `RegisterWorkbenchViewModel`
- [ ] **WPF —** 创建 `Views/RegisterWorkbenchView.xaml`
- [ ] **WPF ——** 流程：选院区 → 选科室 → 选医生 → 选日期时段 → 选择患者（新患者先建档）→ 确认挂号
- [ ] **WPF ——** 退号功能：已挂号列表 + 退号确认
- [ ] **WPF —** 替换占位页：`opd.register` 路由

---

### 模块⑥：门诊医生站

> **模块描述：** 临床核心模块，也是系统最复杂的模块。采用单页多 Tab 设计：患者队列 → 病历书写 → 诊断开立 → 处方开立 → 检验检查申请。医生在诊间完成接诊、记录、开立的全流程。病历支持草稿/终稿状态和版本留痕。处方开立后进入待缴费状态。
>
> **对应的业务角色：** 门诊医生
>
> **依赖：** 模块②（科室+人员）、模块③（ICD 字典+药品字典）、模块④（患者）、模块⑤（挂号）

#### 6.1 就诊与患者队列

- [ ] **Domain —** 创建 `Encounter` 实体（Id、PatientId、DoctorId、DeptId、CampusId、RegisterId、Status、StartTime、EndTime）
- [ ] **Domain —** 创建 `EncounterStatus` 枚举（待诊、就诊中、已完成）
- [ ] **Application —** 定义 `IEncounterRepository`
- [ ] **Application —** 创建对应 DTO
- [ ] **Application —** 实现 `EncounterApplicationService`（开始接诊、完成接诊）
- [ ] **Infrastructure —** 实现 `EncounterRepository`
- [ ] **API —** 创建 `EncounterController`
- [ ] **WPF —** 创建 `EncounterWorkbenchViewModel`
- [ ] **WPF —** 创建 `Views/EncounterWorkbenchView.xaml`（TabControl 容器页）
- [ ] **WPF ——** Tab1 患者队列：待诊列表（来自挂号） + 叫号按钮
- [ ] **WPF —** 替换占位页：`opd.encounter` 路由

#### 6.2 病历书写

- [ ] **Domain —** 创建 `MedicalRecord` 实体（Id、EncounterId、ChiefComplaint、PresentIllness、PastHistory、PhysicalExam、Diagnosis、RecordStatus、Version、CreatedAt）
- [ ] **Domain —** 创建 `RecordStatus` 枚举（草稿、终稿、已修改）
- [ ] **Application —** 定义 `IMedicalRecordRepository`
- [ ] **Application —** 创建对应 DTO
- [ ] **Application —** 实现 `MedicalRecordApplicationService`（保存草稿、提交终稿、版本留痕）
- [ ] **Infrastructure —** 实现 `MedicalRecordRepository`
- [ ] **API —** 创建 `MedicalRecordController`
- [ ] **WPF —** 创建 `EmrEditorViewModel`
- [ ] **WPF —** 创建 `Views/EmrEditorView.xaml`（结构化表单：主诉、现病史、既往史、体格检查等 TextArea）
- [ ] **WPF —** 嵌入 EncounterWorkbench Tab2

#### 6.3 诊断开立

- [ ] **Domain —** 创建 `Diagnosis` 实体（Id、EncounterId、DiagnosisType、IcdCode、Description、IsPrimary）
- [ ] **Domain —** 创建 `DiagnosisType` 枚举（主要诊断、次要诊断、疑似诊断）
- [ ] **Application —** 定义 `IDiagnosisRepository`
- [ ] **Application —** 实现 `DiagnosisApplicationService`
- [ ] **API —** 诊断 CRUD（可合并到 EncounterController 或独立诊断端点）
- [ ] **WPF —** 创建 `DiagnosisOrderViewModel`
- [ ] **WPF —** 创建 `Views/DiagnosisOrderView.xaml`（ICD 搜索选择 + 诊断列表）
- [ ] **WPF —** 嵌入 EncounterWorkbench Tab3

#### 6.4 处方开立

- [ ] **Domain —** 创建 `Prescription` / `PrescriptionItem` 实体
- [ ] **Domain —** 药品属性：DrugName、Specification、DosageForm、Frequency、Dosage、Duration、Quantity、Note
- [ ] **Domain —** 创建 `PrescriptionStatus` 枚举（待缴费、已缴费、已发药、已退药）
- [ ] **Application —** 定义 `IPrescriptionRepository`
- [ ] **Application —** 实现 `PrescriptionApplicationService`（开立、作废）
- [ ] **Infrastructure —** 实现 `PrescriptionRepository`
- [ ] **API —** 创建 `PrescriptionController`
- [ ] **WPF —** 创建 `PrescriptionEditorViewModel`
- [ ] **WPF —** 创建 `Views/PrescriptionEditorView.xaml`（处方网格：药品选择器 + 用法用量编辑）
- [ ] **WPF —** 嵌入 EncounterWorkbench Tab4

#### 6.5 检验检查申请

- [ ] **Domain —** 创建 `LabOrder` / `RadOrder` 实体（Id、EncounterId、ItemCode、ItemName、Status、CreatedAt）
- [ ] **Domain —** 创建 `OrderStatus` 枚举（已开立、已缴费、已执行、已报告、已取消）
- [ ] **Application —** 定义 `ILabOrderRepository` / `IRadOrderRepository`
- [ ] **Application —** 实现申请服务
- [ ] **API —** 创建 `LabOrderController` / `RadOrderController`
- [ ] **WPF —** 创建 `LabRadApplyViewModel`
- [ ] **WPF —** 创建 `Views/LabRadApplyView.xaml`（申请项目选择 + 申请单列表）
- [ ] **WPF —** 嵌入 EncounterWorkbench Tab5

---

### 模块⑦：发药

> **模块描述：** 门诊处方发药闭环。药师审核已缴费处方，确认后扣减库存并完成发药。支持退药回库。管控药品需要双人核对。
>
> **对应的业务角色：** 药师
>
> **依赖：** 模块⑥（处方）、药品目录数据

#### 7.1 发药工作台

- [ ] **Domain —** 创建 `Dispense` / `DispenseItem` 实体
- [ ] **Domain —** 创建 `DispenseStatus` 枚举（待审核、已审核、已发药、已退药）
- [ ] **Domain —** 创建 `DrugInventory` 实体（药品库存：批号、效期、数量）
- [ ] **Application —** 定义 `IDispenseRepository` / `IInventoryRepository`
- [ ] **Application —** 实现 `DispenseApplicationService`
- [ ] **Application ——** 处方审核：校验处方合法性
- [ ] **Application ——** 发药：扣减库存 + 更新处方状态
- [ ] **Application ——** 退药：回冲库存 + 更新状态
- [ ] **Infrastructure —** 实现仓储
- [ ] **API —** 创建 `DispenseController`
- [ ] **WPF —** 创建 `DispenseViewModel`
- [ ] **WPF —** 创建 `Views/DispenseView.xaml`（待发药列表 + 处方明细 + 发药确认 + 退药操作）
- [ ] **WPF —** 替换占位页：`pha.dispense` 路由

---

### 模块⑧：收费

> **模块描述：** 费用结算模块。处理挂号费和处方费的收取与退费。收费后更新对应业务单据（挂号单、处方）的状态为"已缴费"。退费需红冲处理。
>
> **对应的业务角色：** 收费员
>
> **依赖：** 模块⑤（挂号）、模块⑥（处方）

#### 8.1 收费工作台

- [ ] **Domain —** 创建 `Billing` / `BillingItem` / `Payment` 实体
- [ ] **Domain —** 创建 `BillingStatus` 枚举（待缴、已缴、已退）
- [ ] **Domain —** 创建 `PaymentMethod` 枚举（现金、微信、支付宝、银行卡、医保）
- [ ] **Application —** 定义 `IBillingRepository`
- [ ] **Application —** 实现 `BillingApplicationService`
- [ ] **Application ——** 计价：汇总待缴费项目（挂号费 + 处方药品费）
- [ ] **Application ——** 收费：创建账单 + 记录支付 + 更新关联单据状态
- [ ] **Application ——** 退费：红冲原账单 + 恢复关联单据状态
- [ ] **Infrastructure —** 实现 `BillingRepository`
- [ ] **API —** 创建 `BillingController`
- [ ] **WPF —** 创建 `CashierViewModel`
- [ ] **WPF —** 创建 `Views/CashierView.xaml`（患者搜索 → 待缴费清单 → 收费确认 → 打印小票）
- [ ] **WPF —** 替换占位页：`fin.cash` 路由

---

### 模块⑨：患者检索

> **模块描述：** 日常高频操作。支持按姓名、身份证号、手机号等条件模糊搜索患者，结果分页展示。患者 360 视图汇总展示患者基本信息、就诊历史、处方记录、检查报告等，提供全景式患者画像。
>
> **对应的业务角色：** 挂号员、医生、护士
>
> **依赖：** 模块④（患者数据）、模块⑤⑥（就诊/处方数据做 360 视图）

#### 9.1 患者检索后端
- [x] GET `/api/patient/{id}` 按 ID 查询
- [x] GET `/api/patient/by-patient-no/{patientNo}` 按病历号查询
- [ ] **API —** GET `/api/patient/search?keyword=&page=&size=` 模糊搜索（姓名/身份证/手机号）
- [ ] **API —** GET `/api/patient/{id}/profile` 患者 360 视图（基本信息 + 就诊历史 + 处方 + 检查报告摘要）

#### 9.2 患者检索 WPF UI
- [ ] **WPF —** 创建 `PatientSearchViewModel`
- [ ] **WPF —** 创建 `Views/PatientSearchView.xaml`
- [ ] **WPF ——** 搜索栏：关键词输入 + 搜索条件切换（姓名/身份证/手机号）
- [ ] **WPF ——** 结果列表：分页展示患者摘要信息
- [ ] **WPF ——** 点击行跳转患者 360 视图
- [ ] **WPF —** 替换占位页：`pat.search` 路由

#### 9.3 患者 360 视图
- [ ] **WPF —** 创建 `Patient360ViewModel`
- [ ] **WPF —** 创建 `Views/Patient360View.xaml`
- [ ] **WPF ——** 患者信息头（PatientBanner）：姓名、性别、年龄、病历号、身份证号、联系方式
- [ ] **WPF ——** Tab1 基本信息：详细资料 + 建档信息
- [ ] **WPF ——** Tab2 就诊历史：日期、科室、医生、诊断摘要（链接到详情）
- [ ] **WPF ——** Tab3 处方记录：药品、用量、状态
- [ ] **WPF ——** Tab4 检查报告：项目、结果、时间
- [ ] **WPF —** 注册路由：`pat.360`

---

### 模块⑩：用户与权限

> **模块描述：** 系统安全基座。基于 RBAC（Role-Based Access Control）模型：用户 → 角色 → 权限。权限控制到按钮级操作（如 `pat.read`、`opd.emr.edit`）。数据域隔离确保 A 院区用户无法查看 B 院区数据。审计日志记录关键操作（处方修改、费用变更、权限分配等）的可追溯信息。
>
> **对应的业务角色：** 系统管理员、安全审计员
>
> **依赖：** 模块②（院区+科室作为数据域）

#### 10.1 用户管理
- [ ] **Domain —** 创建 `User` 实体（Id、Username、DisplayName、PasswordHash、CampusId、DeptId、IsActive）
- [ ] **Domain —** 创建 `PasswordHash` 值对象
- [ ] **Application —** 定义 `IUserRepository`
- [ ] **Application —** 实现 `UserApplicationService`（CRUD + 重置密码 + 启用/停用）
- [ ] **API —** 创建 `UserController`
- [ ] **WPF —** 创建 `UserRoleViewModel`
- [ ] **WPF —** 用户列表 + 新增/编辑表单 + 重置密码

#### 10.2 角色与权限管理
- [ ] **Domain —** 创建 `Role` / `Permission` 实体
- [ ] **Domain —** 权限码常量定义（`Hospital.Domain/Enums/Permissions.cs`）
- [ ] **Application —** 实现 `RoleApplicationService`（角色CRUD + 权限分配）
- [ ] **API —** 创建 `RoleController` + 获取当前用户权限接口
- [ ] **WPF —** 在 UserRoleView 中增加角色 Tab
- [ ] **WPF —** 权限树展示 + 勾选分配
- [ ] **WPF —** 替换占位页：`sys.userrole` 路由

#### 10.3 数据域隔离
- [ ] 所有业务 API 查询自动追加当前用户的院区/科室过滤
- [ ] JWT Token 中提取 `campus_id` / `dept_id` claims
- [ ] 跨院区越权访问统一返回 403
- [ ] API 端过滤器/中间件统一处理数据域

#### 10.4 审计日志
- [ ] **Domain —** 创建 `AuditLog` 实体（UserId、Action、EntityType、EntityId、OldValue、NewValue、IpAddress、Timestamp）
- [ ] **Application —** 实现 `AuditLogApplicationService`（写入 + 查询）
- [ ] **API —** 创建 `AuditLogController`（日志查询、导出）
- [ ] **API —** 关键操作点位埋入审计日志（处方修改、费用变更、权限分配、用户登录）
- [ ] **WPF —** 创建 `AuditLogView.xaml`（搜索 + 列表 + 详情）
- [ ] **WPF —** 注册路由：`sys.audit`

---

### 额外任务：PatientRepository 集成 EF Core

- [ ] **Infrastructure —** 配置 DbContext + 实体映射（Fluent API）
- [ ] **Infrastructure —** 替代当前内存模拟 `PatientRepository` 为 EF Core 实现
- [ ] **Infrastructure —** 数据库迁移脚本与代码同步

---

## 三、执行顺序与依赖关系

```
第一阶段：认证与导航（P0）—— ✅ 已完成
    │
    ▼
第二阶段：组织主数据 + 字典（P1）← 先做，其他模块依赖院区/科室/字典
    │
    ▼
第三阶段：患者建档 + 检索（P1）← 就诊入口，挂号和医生站的依赖
    │
    ▼
第四阶段：挂号（P2）← 医生站的入口流程
    │
    ▼
第五阶段：门诊医生站（P2）← 最复杂的模块，临床核心
    │
    ▼
第六阶段：收费 + 发药闭环（P3）← 依赖处方才可收费发药
    │
    ▼
第七阶段：用户与权限（P3）← 可独立开发，但需在正式上线前完成
    │
    ▼
第八阶段：打磨与修复（最终）← UI 审查 + 异常处理 + 性能优化
```

> **建议并行策略：**
> 1. 第二阶段：Domain 实体 → Application Service → API Controller → WPF UI，按这个顺序一个模块做完再做下一个
> 2. 后端 API 可以比 WPF 提前 1~2 个阶段开发（先完成所有 Domain + API，再集中做 WPF 页面）
> 3. 第三阶段患者模块可以和第二阶段并行开发（患者领域模型已存在，只需要补 API + WPF）
> 4. 第七阶段用户权限可以在任何阶段插入，建议在第二阶段后端 API 完成后就开始
