# WPF UI 清单与路由表

与《WPF 功能设计与 UI 清单》一致，并补充 **RouteKey**（导航键）、**Permission**（建议权限码）、**备注**。权限码前缀：`sys` `mdm` `pat` `opd` `ipd` `pha` `eqp` `rad` `fin` `rpt` `mon`。

---

## 1. 登录与壳

| UI Key | RouteKey | 类型 | 中文标题 | Permission | 备注 |
|--------|----------|------|----------|------------|------|
| LoginView | `auth.login` | Window | 登录 | — | 启动窗体 |
| MainShellView | `shell.main` | Window | 主框架 | `sys.shell.use` | 登录成功后 `MainWindow` |
| ChangePasswordDialog | — | Dialog | 修改密码 | `sys.user.password` | 模态 |
| AboutDialog | — | Dialog | 关于 | — | 模态 |

---

## 2. M1 组织与基础主数据

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| CampusListView | `mdm.campus` | Page | 院区管理 | `mdm.campus.manage` |
| DepartmentTreeView | `mdm.dept` | Page | 科室维护 | `mdm.dept.manage` |
| WardBedView | `mdm.bed` | Page | 病区床位 | `mdm.bed.manage` |
| StaffListView | `mdm.staff` | Page | 人员档案 | `mdm.staff.read` |
| StaffEditView | `mdm.staff.edit` | Page | 人员编辑 | `mdm.staff.manage` |
| DictionaryListView | `mdm.dict` | Page | 字典管理 | `mdm.dict.manage` |
| ChargeItemListView | `mdm.chargeitem` | Page | 收费项目 | `mdm.chargeitem.manage` |

---

## 3. M2 患者与 EMPI

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| PatientRegisterView | `pat.register` | Page | 患者建档 | `pat.demographic.manage` |
| PatientSearchView | `pat.search` | Page | 患者检索 | `pat.read` |
| Patient360View | `pat.360` | Page | 患者360 | `pat.read` |
| PatientMergeView | `pat.merge` | Page | 疑似重复合并 | `pat.merge.manage` |
| PatientConsentView | `pat.consent` | Page | 隐私授权 | `pat.consent.manage` |

---

## 4. M3 挂号与分诊

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| ScheduleTemplateView | `opd.schedule` | Page | 号源模板 | `opd.schedule.manage` |
| StopReplaceView | `opd.stopreplace` | Page | 停诊替诊 | `opd.schedule.manage` |
| RegisterWorkbenchView | `opd.register` | Page | 挂号工作台 | `opd.register.work` |
| RefundChangeView | `opd.refund` | Page | 退号改签 | `opd.register.refund` |
| TriageQueueView | `opd.triage` | Page | 分诊队列 | `opd.triage.manage` |
| CallDisplaySettingsView | `opd.calldisplay.cfg` | Page | 叫号屏设置 | `opd.calldisplay.manage` |
| CallDisplayWindow | — | Window | 叫号大屏 | `opd.calldisplay.show` | 第二屏全屏，见 [WPF_UI_DECISIONS.md](WPF_UI_DECISIONS.md) |

---

## 5. M4 预约

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| AppointmentRuleView | `opd.appt.rule` | Page | 预约规则 | `opd.appt.manage` |
| AppointmentQueryView | `opd.appt.query` | Page | 预约查询 | `opd.appt.read` |
| PhoneAppointmentView | `opd.appt.phone` | Page | 电话预约登记 | `opd.appt.manage` |
| InternetAppointmentAuditView | `opd.appt.audit` | Page | 互联网预约审核 | `opd.appt.audit` |

---

## 6. M5 门诊医生站（单页多 Tab）

| UI Key | RouteKey | 类型 | 中文标题 | Permission | 备注 |
|--------|----------|------|----------|------------|------|
| OutpatientEncounterView | `opd.encounter` | Page | 门诊诊间 | `opd.emr.use` | 容器页 |
| *(Tab)* OutpatientWorkbenchTab | `opd.encounter.workbench` | UserControl | 患者与叫号 | `opd.emr.use` | 子区 |
| *(Tab)* EmrEditorTab | `opd.encounter.emr` | UserControl | 电子病历 | `opd.emr.edit` | 子区 |
| *(Tab)* DiagnosisOrderTab | `opd.encounter.dx` | UserControl | 诊断处置 | `opd.order.dx` | 子区 |
| *(Tab)* LabRadApplyTab | `opd.encounter.apply` | UserControl | 检验检查申请 | `opd.order.apply` | 子区 |
| *(Tab)* OutpatientPrescriptionTab | `opd.encounter.rx` | UserControl | 门诊处方 | `opd.order.rx` | 子区 |
| *(Tab)* ReferralInpatientApplyTab | `opd.encounter.admitreq` | UserControl | 转诊住院申请 | `ipd.admit.request` | 子区 |

原独立 Page 名称（`EmrEditorView` 等）保留为 **UserControl 文件名** 亦可，与上表 Tab 一一对应。

---

## 7. M6 药品管理

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| DrugCatalogView | `pha.catalog` | Page | 药品目录 | `pha.drug.manage` |
| InventoryInboundView | `pha.inv.in` | Page | 采购入库 | `pha.inv.in` |
| InventoryOutboundView | `pha.inv.out` | Page | 出库调拨 | `pha.inv.out` |
| InventoryCheckView | `pha.inv.check` | Page | 盘点 | `pha.inv.check` |
| OutpatientDispenseView | `pha.disp.opd` | Page | 门诊发药 | `pha.disp.opd` |
| InpatientDispenseView | `pha.disp.ipd` | Page | 住院发药 | `pha.disp.ipd` |
| ReturnDrugView | `pha.return` | Page | 退药 | `pha.return` |
| ControlledDrugDoubleCheckView | `pha.ctrl.double` | Page | 管控药双人核对 | `pha.ctrl.double` |

---

## 8. M7 设备管理

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| EquipmentLedgerView | `eqp.ledger` | Page | 设备台账 | `eqp.asset.manage` |
| EquipmentDispatchView | `eqp.dispatch` | Page | 领用借还 | `eqp.dispatch` |
| EquipmentInspectionView | `eqp.inspect` | Page | 巡检计划 | `eqp.inspect` |
| WorkOrderView | `eqp.workorder` | Page | 维修工单 | `eqp.workorder` |
| CalibrationView | `eqp.calib` | Page | 计量校准 | `eqp.calib` |

---

## 9. M8 病人监护

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| VitalsEntryView | `ipd.vitals` | Page | 生命体征录入 | `ipd.nursing.vitals` |
| NursingRecordView | `ipd.nursing.record` | Page | 护理记录 | `ipd.nursing.record` |
| CriticalValueInboxView | `clin.critical.inbox` | Page | 危急值收件箱 | `clin.critical.handle` |
| IcuMonitorDashboardView | `mon.icu.dashboard` | Page | 重症监护仪表板 | `mon.icu.view` |
| IcuScoreSheetView | `mon.icu.score` | Page | 重症评分 | `mon.icu.score` |
| AlarmRuleView | `mon.alarm.rule` | Page | 报警分级规则 | `mon.alarm.manage` |
| RemotePatientBindView | `mon.remote.bind` | Page | 远程患者绑定 | `mon.remote.manage` |
| RemoteDataReviewView | `mon.remote.review` | Page | 远程数据审阅 | `mon.remote.review` |

---

## 10. M9 住院与护理

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| BedMapView | `ipd.bedmap` | Page | 床位图 | `ipd.bed.read` |
| AdmissionView | `ipd.admit` | Page | 入院登记 | `ipd.admit.manage` |
| TransferView | `ipd.transfer` | Page | 转科转床 | `ipd.transfer` |
| DischargeSettlementView | `ipd.discharge` | Page | 出院结算 | `ipd.discharge` |
| InpatientOrderView | `ipd.orders` | Page | 医嘱管理 | `ipd.order.medical` |
| OrderExecutionView | `ipd.exec` | Page | 医嘱执行 | `ipd.order.execute` |
| TemperatureSheetView | `ipd.temp` | Page | 体温单 | `ipd.nursing.temp` |
| NursingPlanView | `ipd.plan` | Page | 护理计划 | `ipd.nursing.plan` |

---

## 11. M10 检验检查与医技

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| TechAppointmentView | `rad.appt` | Page | 医技预约 | `rad.appt.manage` |
| TechRegisterView | `rad.register` | Page | 到检登记 | `rad.register` |
| ReportBrowserView | `rad.report` | Page | 报告浏览 | `rad.report.read` |
| CriticalValueSendView | `rad.critical.send` | Page | 危急值上报 | `rad.critical.send` |

---

## 12. M11 收费与医保

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| CashierOutpatientView | `fin.cash.opd` | Page | 门诊收费 | `fin.cash.opd` |
| CashierInpatientView | `fin.cash.ipd` | Page | 住院收费 | `fin.cash.ipd` |
| RefundReviewView | `fin.refund` | Page | 退费审核 | `fin.refund.approve` |
| InvoiceBridgeView | `fin.invoice` | Page | 发票接口日志 | `fin.invoice.admin` |
| InsuranceReadCardView | `fin.ins.readcard` | Page | 医保读卡 | `fin.ins.read` |
| InsuranceSettlementView | `fin.ins.settle` | Page | 医保结算 | `fin.ins.settle` |
| InsuranceReconcileView | `fin.ins.recon` | Page | 医保对账 | `fin.ins.recon` |

---

## 13. M12 报表与运营

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| ReportGalleryView | `rpt.gallery` | Page | 报表中心 | `rpt.run` |
| ReportExportQueueView | `rpt.export` | Page | 导出队列 | `rpt.export.manage` |

---

## 14. M13 系统与安全

| UI Key | RouteKey | 类型 | 中文标题 | Permission |
|--------|----------|------|----------|------------|
| UserRoleView | `sys.userrole` | Page | 用户与角色 | `sys.security.manage` |
| PermissionMatrixView | `sys.perm` | Page | 权限矩阵 | `sys.security.manage` |
| AuditLogView | `sys.audit` | Page | 审计日志 | `sys.audit.read` |
| SystemParameterView | `sys.param` | Page | 系统参数 | `sys.param.manage` |
| IntegrationEndpointView | `sys.integration` | Page | 集成端点 | `sys.integration.manage` |

---

## 15. 公共组件（无独立 RouteKey）

| 组件 | XAML 建议路径 | 说明 |
|------|----------------|------|
| PatientBanner | `Views/Shared/PatientBanner.xaml` | 患者条 |
| OrderGrid | `Views/Shared/OrderGrid.xaml` | 医嘱/处方网格 |
| CampusDepartmentPicker | `Views/Shared/CampusDepartmentPicker.xaml` | 院区科室 |
| CardReaderStatus | `Views/Shared/CardReaderStatus.xaml` | 读卡状态 |
| ToastNotification | 行为或服务触发 | 消息提示 |

---

**文档版本**：1.0
