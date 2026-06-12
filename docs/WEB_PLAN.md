# Hospital Web 管理端开发计划

基于现有 HIS 后端 API（ASP.NET Core）新增 **Web 管理端** SPA，与现有 WPF 桌面客户端并行共存，共享同一套后端服务和数据库。

> 参考 [PROJECT_PLAN.md](PROJECT_PLAN.md) 第 3 节的终端划分：WPF 桌面端负责一线业务（挂号、医生站、收费、发药），**Web 管理端负责管理后台**（主数据、排班、字典、设备、报表、权限）。

---

## 1. 核心目标

构建面向 **信息科、财务、运营人员** 的 Web 管理后台，覆盖医院日常管理职能。

| 维度 | 说明 |
|------|------|
| **目标用户** | 信息科管理员、财务人员、运营人员、设备科人员 |
| **负责模块** | 组织主数据、字典维护、排班管理、用户权限、设备台账、报表中心 |
| **不覆盖** | 门诊业务（挂号、医生站、收费、发药）→ 归属 WPF 桌面端 |
| **与 WPF 关系** | 共享同一套后端 API 和数据库，并行共存 |

---

## 2. 用户使用流程

```
信息科 → 维护院区/科室/人员档案 → 维护字典（ICD/收费项目等）
     ↓
排班员 → 排班管理 → 发布号源
     ↓
运营/财务 → 查看报表 → 数据统计
     ↓
信息科 → 用户管理 → 角色权限分配
     ↓
设备科 → 设备台账管理（V2）
```

管理端不涉及患者到院的临床流程（挂号→就诊→缴费→发药），那些由 WPF 桌面端完成。

---

## 3. 核心页面结构

| 模块 | 页面 | 说明 | 后端状态 |
|------|------|------|----------|
| **登录** | 登录页 | JWT 认证，根据角色加载菜单和权限 | ✅ 已有 |
| **首页** | 仪表盘 | 运营概览数据 | ✅ 已有基础 API |
| **主数据** | 院区管理 | 院区 CRUD，含启用/停用 | ✅ 已有 Controller |
| | 科室管理 | 树形科室结构，按院区过滤，含类型（门诊/住院/医技/行政/药房） | ✅ 已有 Controller |
| | 人员管理 | 医护人员档案 + 执业资质管理（含有效期校验） | ✅ 已有 Controller |
| | 字典管理 | 字典类型 → 字典项两级维护，支持排序 | ✅ 已有 Controller |
| **患者** | 患者建档 | 新建患者（病历号/姓名/性别/出生日期/电话/过敏史/身份证） | ✅ 已有 Controller |
| | 患者检索 | 关键词搜索（姓名/病历号/身份证）+ 分页 + 重复提醒 | ✅ 已有 Controller |
| | 患者详情 | 360° 视图（基本信息 + 就诊历史预留） | ✅ 已有 Controller |
| **排班** | 排班管理 | 按医生设置出诊时段和配额，发布/停用排班 | ✅ 已有 Controller |
| **系统** | 用户管理 | 系统用户 CRUD（登录名/密码/显示名/院区/角色） | ✅ 已有 Controller |
| | 角色管理 | 角色 CRUD + 权限勾选分配 | ✅ 已有 Controller |
| **设备** | 设备台账 | 设备资产 CRUD（V2） | ❌ 仅有数据库表 |
| **报表** | 报表中心 | 报表定义配置与数据导出（V2） | ❌ 仅有数据库表 |

---

## 4. 第一版功能范围（V1）

### 必须实现

| # | 模块 | 功能 | 对应 PROJECT_PLAN V1 |
|---|------|------|----------------------|
| 1 | **认证与导航** | JWT 登录、动态菜单、路由守卫 | ① |
| 2 | **组织主数据** | 院区/科室(树形)/人员 CRUD | ② |
| 3 | **字典管理** | 字典类型 + 字典项两级 CRUD | ③ |
| 4 | **患者管理** | 建档、检索、查重、详情 | ④⑨ |
| 5 | **排班管理** | 排班创建、时段配额、发布/停用 | ⑤ |
| 6 | **用户与权限** | 用户/角色 CRUD、权限分配、路由守卫 | ⑩ |

### V1 不包含（保留给 WPF 桌面端）

- ❌ 挂号工作台（PROJECT_PLAN 明确归属 WPF）
- ❌ 门诊医生站（病历/诊断/处方/检验检查）（PROJECT_PLAN 明确归属 WPF）
- ❌ 收费工作台（PROJECT_PLAN 明确归属 WPF）
- ❌ 发药工作台（PROJECT_PLAN 明确归属 WPF）

### 暂不实现（V2+）

- 设备台账管理（后端尚无 Controller，仅有数据库表）
- 报表中心（后端尚无 Controller，仅有数据库表）
- 审计日志页面（后端已有 AuditLogController）
- 住院管理（IPD 模块）
- 药品库存管理

---

## 5. 推荐技术栈

| 层面 | 技术 | 理由 |
|------|------|------|
| **前端框架** | Vue 3 + TypeScript | 组合式 API + 完善的类型推导 |
| **UI 组件库** | Naive UI | Vue 3 原生支持，TypeScript 友好，企业级组件齐全 |
| **路由** | Vue Router 4 | Vue 3 官方路由 |
| **状态管理** | Pinia | Vue 3 官方状态管理，轻量 |
| **HTTP 客户端** | Axios | 拦截器统一处理 JWT Token |
| **表单校验** | Naive UI Form + 内置校验 | Form 组件内置校验规则 |
| **构建工具** | Vite | 快速 HMR |
| **后端** | 保持现有 ASP.NET Core API | 无需改动后端 |
| **数据库** | 保持现有 SQL Server | 无需改动 |

**选择 Naive UI 的理由：**
- 专为 Vue 3 设计，Composition API 原生友好
- 完整的 TypeScript 类型推导
- 树形组件（DataTable Tree）、表格、表单等企业级功能完善
- 可按需加载，包体积小

---

## 6. 项目目录结构

```
hospital-web/
├── public/
├── src/
│   ├── api/                    # API 请求层
│   │   ├── request.ts          # Axios 实例 + JWT 拦截器
│   │   ├── auth.ts             # 登录 API
│   │   ├── campus.ts           # 院区 API
│   │   ├── department.ts       # 科室 API
│   │   ├── staff.ts            # 人员 API
│   │   ├── dictionary.ts       # 字典 API
│   │   ├── patient.ts          # 患者 API
│   │   ├── schedule.ts         # 排班 API
│   │   └── userRole.ts         # 用户角色 API
│   ├── components/             # 通用组件
│   │   └── AppLayout.vue       # 主布局（侧边栏+顶栏+内容区）
│   ├── composables/            # 组合式函数
│   │   ├── useAuth.ts          # 登录状态管理
│   │   └── usePermission.ts    # 权限判断
│   ├── pages/                  # 页面组件
│   │   ├── login/
│   │   ├── dashboard/          # 首页仪表盘
│   │   ├── campus/             # 院区管理
│   │   ├── department/         # 科室管理
│   │   ├── staff/              # 人员管理
│   │   ├── dictionary/         # 字典管理
│   │   ├── patient/            # 患者建档+检索+详情
│   │   ├── schedule/           # 排班管理
│   │   ├── user/               # 用户管理
│   │   └── role/               # 角色管理
│   ├── router/
│   │   └── index.ts            # 路由配置 + 导航守卫
│   ├── stores/
│   │   └── auth.ts             # 用户/权限状态
│   ├── types/
│   │   └── index.ts            # 全量 TypeScript 类型
│   └── utils/
│       ├── constants.ts        # 路由/权限常量
│       └── format.ts           # 日期/金额格式化
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
└── README.md
```

相比旧版目录结构的变化：

| 变更 | 说明 |
|------|------|
| ❌ 移除 `registration/` | 挂号工作台归属 WPF |
| ❌ 移除 `encounter/` | 门诊医生站归属 WPF |
| ❌ 移除 `cashier/` | 收费工作台归属 WPF |
| ❌ 移除 `dispense/` | 发药工作台归属 WPF |
| ❌ 移除 `api/` 中对应的临床 API 文件 | 相应 API 文件一并移除 |
| ✅ 保留 `patient/` | 患者建档与检索为管理端与 WPF 共享功能 |
| ✅ 保留 `schedule/` | 排班管理为管理端职能 |

---

## 7. 分阶段开发计划

### P0（已完成）：基础设施搭建

| 任务 | 状态 |
|------|------|
| Vite + Vue 3 + TypeScript 项目初始化 | ✅ |
| 安装依赖（vue-router、pinia、axios、naive-ui） | ✅ |
| Axios 实例 + JWT 请求拦截 + 401 自动跳转 | ✅ |
| Vue Router + 导航守卫 | ✅ |
| AppLayout 布局（侧边栏 + 顶栏 + 内容区） | ✅ |
| 登录页（表单 + 校验 + API 对接 + Token 管理） | ✅ |
| 所有管理型页面占位 + 路由注册 | ✅ |

---

### P1：院区 + 科室管理（主数据第一组）

院区和科室是组织机构的核心，两者关联紧密（科室依赖院区），作为一个迭代完成。

**任务清单：**

| # | 任务 | 文件 |
|---|------|------|
| 1 | 创建 `campus.ts` API 层（列表/创建/更新/启用停用） | `src/api/campus.ts` |
| 2 | 院区管理页面：表格列表 + 弹窗新建/编辑 + 启用/停用按钮 | `src/pages/campus/index.vue` |
| 3 | 创建 `department.ts` API 层（树形/列表/创建/更新） | `src/api/department.ts` |
| 4 | 科室管理页面：树形列表 + 弹窗新建/编辑（选择上级/院区/类型） | `src/pages/department/index.vue` |
| 5 | 院区选择切换时刷新科室树 | 联动逻辑 |

**验收标准：**
- [ ] 院区列表展示全部院区，支持新建、编辑、启用、停用
- [ ] 科室以树形结构展示，按院区过滤
- [ ] 新建科室时可选上级科室、所属院区、科室类型
- [ ] 操作后有成功/失败提示

---

### P2：人员 + 字典管理（主数据第二组）

人员和字典是主数据的另一部分，相对独立，可并行开发。

**任务清单：**

| # | 任务 | 文件 |
|---|------|------|
| 1 | 创建 `staff.ts` API 层 | `src/api/staff.ts` |
| 2 | 人员管理页面：表格列表 + 弹窗新建/编辑（院区/科室/资质） | `src/pages/staff/index.vue` |
| 3 | 创建 `dictionary.ts` API 层 | `src/api/dictionary.ts` |
| 4 | 字典管理页面：字典类型列表 → 点击查看字典项 → 项 CRUD | `src/pages/dictionary/index.vue` |

**验收标准：**
- [ ] 人员列表展示所有医护人员，支持按院区/科室过滤
- [ ] 人员编辑支持执业资质管理（类型/编号/有效期）
- [ ] 字典类型可增删改，字典项按类型分组维护
- [ ] 字典项支持排序、启用/停用

---

### P3：患者管理（建档 + 检索 + 详情）

患者是全院共享数据，管理端需要完成建档和检索功能。

**任务清单：**

| # | 任务 | 文件 |
|---|------|------|
| 1 | 创建 `patient.ts` API 层 | `src/api/patient.ts` |
| 2 | 患者建档页：表单 + 查重提醒（姓名+电话匹配） | `src/pages/patient/create.vue` |
| 3 | 患者检索页：关键词搜索 + 分页列表 | `src/pages/patient/search.vue` |
| 4 | 患者详情页：基本信息 + 就诊历史（预留） | `src/pages/patient/detail.vue`（新增） |

**验收标准：**
- [ ] 患者建档支持填写病历号/姓名/性别/出生日期/电话/过敏史/身份证
- [ ] 身份证查重提醒，重复患者提示
- [ ] 患者检索支持姓名/病历号/身份证模糊搜索，结果分页
- [ ] 患者详情展示基本信息，就诊历史区域预留

---

### P4：排班管理

排班是管理端的核心之一，信息科或排班员在此发布号源。

**任务清单：**

| # | 任务 | 文件 |
|---|------|------|
| 1 | 创建 `schedule.ts` API 层 | `src/api/schedule.ts` |
| 2 | 排班列表页：按医生/日期筛选 | `src/pages/schedule/index.vue` |
| 3 | 排班新建弹窗：选择医生+日期+时段（名称/起止/配额） | 同上 |
| 4 | 排班发布/停用操作，号源状态展示 | 同上 |

**验收标准：**
- [ ] 可创建排班（选择医生、院区、科室、日期、时段和配额）
- [ ] 排班可发布和停用
- [ ] 已发布排班的号源状态清晰展示（已约/剩余）

---

### P5：用户与权限

系统管理的核心，控制谁能访问什么功能。

**任务清单：**

| # | 任务 | 文件 |
|---|------|------|
| 1 | 创建 `userRole.ts` API 层 | `src/api/userRole.ts` |
| 2 | 用户管理：列表 + 新建/编辑（登录名/密码/显示名/院区/角色） | `src/pages/user/index.vue` |
| 3 | 角色管理：列表 + 新建/编辑（角色名/描述/权限勾选树） | `src/pages/role/index.vue` |
| 4 | 权限路由守卫增强：无权限页面拦截跳转 403 | `src/router/index.ts` |

**验收标准：**
- [ ] 管理员可创建/编辑/启用/禁用用户
- [ ] 创建用户时可选择所属院区和分配角色
- [ ] 角色权限勾选与后端 Permissions.cs 一致
- [ ] 无权限菜单自动隐藏，直接访问 URL 返回 403 页面

---

### P6：仪表盘 + 打磨

最后的打磨阶段，提升体验和稳定性。

**任务清单：**

| # | 任务 | 文件 |
|---|------|------|
| 1 | 首页仪表盘：统计数据展示 | `src/pages/dashboard/index.vue` |
| 2 | 全局错误处理：网络异常/业务异常统一提示 | `src/api/request.ts` |
| 3 | 全局加载状态 | `src/components/AppLayout.vue` |
| 4 | 表单校验完善 + UI 一致性审查 | 全部页面 |
| 5 | 构建优化 + 部署说明 | 项目根目录 |

**验收标准：**
- [ ] 首页展示关键运营数据
- [ ] 所有操作有 loading 状态和结果反馈
- [ ] 表单必填项、格式校验覆盖完整
- [ ] 整体 UI 风格统一

---

## 8. 后端 API 对接说明

前端直接对接现有 `Hospital.Api` 后端，无需改动后端代码。

| 项 | 值 |
|----|-----|
| 基础 URL（代理） | `/api` → `http://localhost:5075` |
| 认证方式 | JWT Bearer Token（请求头 `Authorization: Bearer xxx`） |
| 登录接口 | `POST /api/Authentication/login` |
| 登录请求体 | `{ "username": "admin", "password": "admin123" }` |
| 登录返回体 | `{ "token": "...", "displayName": "...", "campusName": "...", "roles": [...] }` |

**关键说明：**
- 登录失败返回 `401 { "message": "用户名或密码错误" }`
- 权限通过 JWT 中的 `permissions` claim（逗号分隔）传递
- 前端 Axios 请求拦截器自动注入 Token
- 响应拦截器捕获 401 后自动跳转登录页

测试账号：

| 账号 | 密码 | 角色 | 权限范围 |
|------|------|------|----------|
| admin | admin123 | 管理员 | 全部管理端功能 |
| doctor | doctor123 | 门诊医生 | 仅有门诊医生站权限（WPF 使用） |

---

## 9. 关键路由与权限映射

参考后端 [Constants.cs](../src/Hospital.Application/Constants/Constants.cs) 和 [Permissions.cs](../src/Hospital.Application/Constants/Permissions.cs)：

| 路由 | 路径 | 权限标识 | 说明 |
|------|------|----------|------|
| 首页 | `/dashboard` | `sys.shell.use` | 所有登录用户 |
| 院区管理 | `/campus` | `mdm.campus.manage` | 信息科 |
| 科室管理 | `/department` | `mdm.dept.manage` | 信息科 |
| 人员管理 | `/staff` | `mdm.staff.manage` | 信息科 |
| 字典管理 | `/dictionary` | `mdm.dict.manage` | 信息科 |
| 患者建档 | `/patient/create` | `pat.register` | 挂号员（WPF 和管理端共用） |
| 患者检索 | `/patient/search` | `pat.search` | 挂号员/医生（WPF 和管理端共用） |
| 排班管理 | `/schedule` | `opd.schedule` | 排班员 |
| 用户管理 | `/users` | `sys.security.manage` | 信息科 |
| 角色管理 | `/roles` | `sys.security.manage` | 信息科 |
