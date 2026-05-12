# WPF 技术架构（.NET 8 单一客户端）

## 1. 目标与约束

- **运行时**：.NET 8，`Windows` 目标，`WPF`。
- **形态**：单一主程序（`Hospital.App`），登录后按 **RBAC + 院区/科室数据域** 显示导航。
- **后端**：通过 **HTTP API**（REST 或医院集成网关）访问业务服务；本仓库骨架仅含客户端与契约占位。

## 2. 解决方案结构（建议）

```
Hospital.sln
  src/Hospital.App          # WPF 启动项、Views、ViewModels、资源
  src/Hospital.Contracts    # （可选）DTO、Api 路由常量、权限码枚举
```

首期可仅 `Hospital.App`；模块增大后再拆类库。

## 3. 分层与 MVVM

| 层 | 职责 |
|----|------|
| **Views** | `*.xaml` + code-behind 仅做极薄胶水；复杂交互拆 `UserControl`。 |
| **ViewModels** | `CommunityToolkit.Mvvm`：`ObservableObject`、`RelayCommand`、源生成器。 |
| **Services** | `IApiClient`、`INavigationService`、`IDialogService`、`IAppContext`、`IReportExportService`。 |
| **Models** | 展示模型与校验；与 API DTO 映射可用扩展方法或 Mapster（后续）。 |

## 4. 导航契约

- **主壳**：`MainWindow` — 顶栏（院区、用户、消息）、左侧 `TreeView` 菜单、右侧 `ContentControl` 主内容区。
- **路由键**：与 [WPF_UI_INVENTORY.md](WPF_UI_INVENTORY.md) 中 `RouteKey` 一致；`INavigationService.Navigate(routeKey, parameter?)`。
- **门诊诊间**：`RouteKey = outpatient.encounter`，单页内 `TabControl` 切换子区（见 [WPF_UI_DECISIONS.md](WPF_UI_DECISIONS.md)）。

## 5. 横切关注点

- **IAppContext**：`UserId`、`RoleIds`、`CampusId`、`DepartmentId`、`Shift`；院区切换触发导航缓存清理与菜单重载。
- **IApiClient**：`HttpClient` + Polly 重试 + 401 刷新令牌（接口占位）。
- **审计**：关键操作写本地队列 + 异步上报（接口占位）。
- **主题**：`ResourceDictionary` 统一字体、间距；支持 **高 DPI**（PerMonitorV2 在 `app.manifest` 中启用）。

## 6. 重症波形扩展

```text
IIcuWaveformHost (Contracts 或 App/Services/Monitoring)
  <- PlaceholderWaveformHost
  <- WebView2WaveformHost
  <- WinFormsHostWaveformHost   # 独立文件夹，按需引用厂商 interop
```

## 7. 与需求文档映射

- 需求基线：[PRODUCT_SCOPE_CONFIRMATIONS.md](PRODUCT_SCOPE_CONFIRMATIONS.md)
- 权限矩阵：[ROLE_PERMISSION_MATRIX.md](ROLE_PERMISSION_MATRIX.md)
- UI 清单与路由：[WPF_UI_INVENTORY.md](WPF_UI_INVENTORY.md)

**文档版本**：1.0
