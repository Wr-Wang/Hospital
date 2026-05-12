# 医院在线管理系统

本仓库为**医院在线管理系统**相关工程，包含《医院管理系统》**需求基线文档**与 **.NET 8 WPF** 客户端骨架（`Hospital.sln` / `src/Hospital.App`）。

**注意**：Cursor 计划文件（`.cursor/plans/...`）不在此仓库内维护；需求正文以 `docs/` 为准迭代版本号。

## 运行客户端

**环境**：Windows，已安装 [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)。

```powershell
cd e:\Demo\Cursor\Hospital
dotnet build Hospital.sln -c Release
dotnet run --project src\Hospital.App\Hospital.App.csproj -c Release
```

启动后主窗为**顶栏 + 左侧菜单 + 右侧内容区**；`NavigationService` 已注册 `shell.home`、`mdm.campus`、`opd.register`（暂均指向占位页，后续按 [docs/WPF_UI_INVENTORY.md](docs/WPF_UI_INVENTORY.md) 拆分真实视图）。

## 文档索引

| 文档 | 说明 |
|------|------|
| [docs/PRODUCT_SCOPE_CONFIRMATIONS.md](docs/PRODUCT_SCOPE_CONFIRMATIONS.md) | 服务范围、监护、收费医保、部署等**确认结论** |
| [docs/ROLE_PERMISSION_MATRIX.md](docs/ROLE_PERMISSION_MATRIX.md) | 角色 × 功能域权限矩阵 |
| [docs/CORE_FLOWS.md](docs/CORE_FLOWS.md) | 核心业务流程（Mermaid） |
| [docs/USER_STORIES_EPICS.md](docs/USER_STORIES_EPICS.md) | Epic 与用户故事初版 |
| [docs/WPF_ARCHITECTURE.md](docs/WPF_ARCHITECTURE.md) | WPF 分层、MVVM、导航契约 |
| [docs/WPF_UI_INVENTORY.md](docs/WPF_UI_INVENTORY.md) | UI Key、RouteKey、权限码对照 |
| [docs/WPF_UI_DECISIONS.md](docs/WPF_UI_DECISIONS.md) | 门诊 Tab、叫号大屏、重症波形等 UI 实施结论 |
| [docs/DATABASE_SCHEMA_PLAN.md](docs/DATABASE_SCHEMA_PLAN.md) | 数据库表清单、分表设计与脚本索引（与 `database/` 同步） |
| [database/README.md](database/README.md) | **SQL Server** 建库/建表脚本说明与执行顺序 |

## 解决方案结构

| 路径 | 说明 |
|------|------|
| [Hospital.sln](Hospital.sln) | Visual Studio / `dotnet` 解决方案 |
| [src/Hospital.App](src/Hospital.App) | WPF 启动项目 |

## 下一步（建议）

1. 在 `PRODUCT_SCOPE_CONFIRMATIONS.md` 中补充**医保首发省市**与接口厂商信息。  
2. 在 SQL Server 上按 [database/README.md](database/README.md) 顺序执行 `000`–`015` 与可选 `900_seed_minimal.sql`，验证建表与扩展属性。  
3. 实现 `IApiClient` 与后端 API 契约，按 `WPF_UI_INVENTORY` 逐路由注册真实 `UserControl`。  
4. 增加登录窗体并将 `ApplicationContext` 与令牌从登录结果注入。
