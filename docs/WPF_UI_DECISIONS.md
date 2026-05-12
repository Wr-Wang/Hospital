# WPF UI 与交互确认结论（实施基线）

本文档冻结《WPF 医院管理系统 — 功能设计与 UI 清单》§6 三项待确认问题，作为工程实现的默认假设。若与院方或合同冲突，以变更单更新本文件版本号。

| 议题 | 结论 | 实施说明 |
|------|------|----------|
| Page 拆分粒度 | **门诊诊间采用「单页多 Tab」**；其余模块保持独立 Page | `OutpatientEncounterView` 为主容器，内嵌 `UserControl`：`Workbench`、`Emr`、`Diagnosis`、`LabRadApply`、`Prescription`、`Referral`；原 UI Key 保留为子控件名便于权限与埋点。 |
| 叫号大屏 | **提供独立 WPF `Window`，支持第二显示器全屏** | `CallDisplayWindow` 可由 `MainShell` 菜单或分诊台按钮打开；`CallDisplaySettingsView` 配置队列源与屏参。院方若坚持用纯 Web 大屏，可关闭该窗体入口，仅保留接口数据。 |
| 重症波形呈现 | **默认占位 + WebView2 外链调阅**；预留 **WinFormsHost / 原生 SDK** 扩展点 | `IcuMonitorDashboardView` 中波形区实现 `IIcuWaveformHost` 接口；默认实现 `PlaceholderWaveformHost`；可选 `WebView2WaveformHost`（厂商 H5）；可选 `WinFormsHostWaveformHost`（ActiveX/SDK）由单独程序集引用，避免主工程强依赖 32 位控件。 |

**文档版本**：1.0
