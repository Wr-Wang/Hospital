# V1 未实现功能清单

> 基于 V1_TASKS.md 与实际代码审计，按优先级排序。
> 状态：awaiting confirmation — 请审阅后确认是否开始开发。

---

## P0 — 阻塞项（影响核心流程完整性）

### 0.1 Billing 领域实体未实现

**涉及模块：** 收费工作台（CashierWorkbench）

审计发现收费工作台 View/ViewModel/API Controller 均已实现，但**无 Billing 领域实体**：
- `Hospital.Domain` 缺少 `BillingAggregate`（含 `Billing`、`BillingItem`、`Payment` 实体）
- `Hospital.Infrastructure.Repositories` 没有 `BillingRepository` / `PaymentRepository`
- CashierController 直接依赖 `IBillingRepository` — 引用了尚未创建的接口

**需要创建：**
- `Hospital.Domain/Aggregates/BillingAggregate/Billing.cs`
- `Hospital.Domain/Aggregates/BillingAggregate/BillingItem.cs`
- `Hospital.Domain/Aggregates/BillingAggregate/Payment.cs`
- `Hospital.Application/Interfaces/IBillingRepository.cs`
- `Hospital.Infrastructure/Repositories/BillingRepository.cs`（内存模拟）

### 0.2 Dispense 领域实体未实现

**涉及模块：** 发药工作台（DispenseWorkbench）

与 Billing 相同，View/ViewModel/API Controller 已实现，但**无 Dispense 领域实体**：
- `Hospital.Domain` 缺少 `DispenseAggregate`（含 `Dispense`、`DispenseItem`、`DrugInventory` 实体）
- `Hospital.Infrastructure.Repositories` 没有 `DispenseRepository` / `DrugInventoryRepository`
- DispenseController 依赖 `IDispenseRepository` / `IDrugInventoryRepository` — 引用尚未创建的接口

**需要创建：**
- `Hospital.Domain/Aggregates/DispenseAggregate/Dispense.cs`
- `Hospital.Domain/Aggregates/DispenseAggregate/DispenseItem.cs`
- `Hospital.Domain/Aggregates/DispenseAggregate/DrugInventory.cs`
- `Hospital.Application/Interfaces/IDispenseRepository.cs`
- `Hospital.Application/Interfaces/IDrugInventoryRepository.cs`
- `Hospital.Infrastructure/Repositories/DispenseRepository.cs`
- `Hospital.Infrastructure/Repositories/DrugInventoryRepository.cs`

---

## P1 — 高优先级（缺 UI = 功能不可见）

### 1.1 科室管理页面（占位符）

**文件：** [Views/CampusView.xaml](src/Hospital.App/Views/CampusView.xaml)（占位符）
- 仅有 `TextBlock("院区管理页面 — 开发中")`
- 后端 API `CampusController` 已完整实现（CRUD）
- 需创建：`CampusViewModel` + 完整 `CampusView.xaml`

### 1.2 院区管理页面（占位符）

**文件：** [Views/DepartmentView.xaml](src/Hospital.App/Views/DepartmentView.xaml)（占位符）
- 仅有 `TextBlock("科室管理页面 — 开发中")`
- 后端 API `DepartmentController` 已完整实现（CRUD）
- 需创建：`DepartmentViewModel` + 完整 `DepartmentView.xaml`

### 1.3 职工管理页面（占位符）

**文件：** [Views/StaffView.xaml](src/Hospital.App/Views/StaffView.xaml)（占位符）
- 仅有 `TextBlock("职工管理页面 — 开发中")`
- 后端 API `StaffController` 已完整实现（CRUD + 排班查询）
- 需创建：`StaffListViewModel` + 完整 `StaffView.xaml`

### 1.4 数据字典页面（占位符）

**文件：** [Views/DictionaryView.xaml](src/Hospital.App/Views/DictionaryView.xaml)（占位符）
- 仅有 `TextBlock("数据字典管理页面 — 开发中")`
- 后端 API `DictionaryController` 已完整实现（CRUD + 按类型查询）
- 需创建：`DictionaryViewModel` + 完整 `DictionaryView.xaml`

### 1.5 Patient360 路由未注册

**文件：** [Services/NavigationService.cs](src/Hospital.App/Services/NavigationService.cs)
- `Patient360View` / `Patient360ViewModel` 类文件存在
- 但路由 `pat.360` **未在 NavigationService 中注册**
- 导致 PatientSearchView 中的"360 视图"按钮无法导航

**修复：**
- 在 `NavigationService` 构造函数中添加 `CreateView<Patient360View, Patient360ViewModel>("pat.360")`

---

## P2 — 中优先级（基础设施/质量）

### 2.1 AuditLog 实体与仓储

**现状：** 系统无任何操作审计日志。
- `Hospital.Domain` 缺少 `AuditLog` 实体
- 无 `IAuditLogRepository` 接口
- 所有 Controller 未记录关键操作（登录、创建用户、发药、退费等）
- 虽 V1_TASKS.md 标记为"V1 不实现"，但 Phase 8 风格修复后保留为低优先级任务

### 2.2 V1_TASKS.md 状态过期

**现状：** 大量已实现功能标记为"未开始"或"进行中"，与代码实际状态严重不符。
- 需逐项核对并更新所有复选框状态
- 建议保留实际与计划的差异记录

---

## P3 — 低优先级（架构优化）

### 3.1 EF Core 集成

**现状：** 所有仓储为内存模拟实现。

### 3.2 数据域隔离中间件

**现状：** 无多院区数据隔离机制。

---

## 汇总

| 优先级 | 任务 | 预估工作量 | 涉及文件 |
|--------|------|-----------|---------|
| P0 | Billing 领域实体 | 5 文件 | Domain 3 + Application 1 + Infrastructure 1 |
| P0 | Dispense 领域实体 | 7 文件 | Domain 3 + Application 2 + Infrastructure 2 |
| P1 | 科室管理页面 | 2 文件 | View + ViewModel |
| P1 | 院区管理页面 | 2 文件 | View + ViewModel |
| P1 | 职工管理页面 | 2 文件 | View + ViewModel |
| P1 | 数据字典页面 | 2 文件 | View + ViewModel |
| P1 | Patient360 路由注册 | 1 文件 | NavigationService 1处添加 |
| P2 | AuditLog 实体与仓储 | 5 文件 | Domain 1 + Application 1 + Infrastructure 1 + Api 2 |
| P2 | 更新 V1_TASKS.md | 1 文件 | 文档更新 |
| P3 | EF Core 集成 | 大 | 跨多项目 |
| P3 | 数据域隔离中间件 | 中 | Api 中间件 |

> **总计：** P0 约 12 文件，P1 约 9 文件，P2 约 5 文件，P3 待定。
