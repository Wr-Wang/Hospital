# SQL Server 数据库脚本

与 [docs/DATABASE_SCHEMA_PLAN.md](../docs/DATABASE_SCHEMA_PLAN.md) 对齐的 **T-SQL** 建库与建表脚本。字段说明通过 **`MS_Description` 扩展属性** 与存储过程 `dbo.sp_AddDescription` 维护（可在 SSMS「对象资源管理器 → 表 → 列属性」中查看）。

## 重要说明

- **目标版本**：SQL Server **2019+**。
- **安全架构名**：应用侧安全表使用 **`sec`** 架构（**勿使用 `sys`**，其为系统保留）。
- **数据库名**：默认在 [000_init_database.sql](000_init_database.sql) 中创建为 **`Hospital`**，可按需修改脚本内 `@DbName`。
- **执行顺序**：请按文件名数字升序执行（`000` → `015`，最后可选 `900`）。

## 执行方式示例

在 **sqlcmd** 或 **SSMS** 中按顺序打开执行；或使用 PowerShell：

脚本文件为 **UTF-8**（含中文注释）。在 **中文 Windows** 上若 `sqlcmd` 默认代码页不是 UTF-8，**必须**加 **`-f i:65001`**，否则批次可能被误解析（例如外键指向的表尚未创建却执行到后续语句）。**ODBC Driver 18** 本机开发常见证书校验失败，可加 **`-C`**（信任服务器证书，仅建议本机）。含索引/筛选索引的脚本建议加 **`-I`**（`SET QUOTED_IDENTIFIER ON`），否则可能出现错误 1934。

```powershell
$server = "localhost"
$base = "e:\Demo\Cursor\Hospital\database"   # 按本机仓库路径修改
$dbScripts = @(
  "000_init_database.sql",
  "001_mdm_organization.sql",
  "002_mdm_dictionary.sql",
  "003_sec_security.sql",
  "004_pat_empi.sql",
  "005_opd_schedule.sql",
  "006_opd_registration.sql",
  "007_clinical_encounter.sql",
  "008_clinical_orders_split.sql",
  "009_pha_drug.sql",
  "010_eqp_asset.sql",
  "011_mon_monitoring.sql",
  "012_ipd_inpatient.sql",
  "013_rad_report.sql",
  "014_fin_billing.sql",
  "015_rpt_meta.sql",
  "900_seed_minimal.sql"
)
foreach ($f in $dbScripts) {
  sqlcmd -S $server -C -I -f i:65001 -E -b -i "$base\$f"
  if ($LASTEXITCODE -ne 0) { throw "Failed on $f" }
}
```

（若使用 SQL 身份验证，将 `-E` 改为 `-U` / `-P`；**勿**把生产口令写入仓库脚本。）

## 种子数据

[900_seed_minimal.sql](900_seed_minimal.sql) 会插入演示机构/院区/根科室、管理员用户（登录名 **`admin`**）及占位密码哈希。**上线前必须**接入真实密码哈希方案并轮换。

## 与文档差异

- `fin.ChargeLines` 未加复杂互斥 `CHECK`，由应用层计费引擎保证不重复计费；可按院规再收紧。
