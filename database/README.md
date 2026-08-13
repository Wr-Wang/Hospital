# SQL Server 数据库脚本

数据库脚本已合并为两个文件：**建库建表** 与 **种子数据**。

| 文件 | 说明 |
|------|------|
| `01_create_schema.sql` | 创建数据库 `Hospital` + 全部 29 张表（27 张 EF Core 业务表 + 微信登录表 `sec.WeChatAccounts` / `sec.PatientRefreshTokens`）。表结构由 `HospitalDbContext` 模型生成，排序规则 `Chinese_PRC_CI_AS`。 |
| `02_seed_data.sql` | 初始化种子数据（院区、科室、员工、用户、字典、患者、排班时段、挂号、就诊、发药等），末尾自带数据验证汇总。全部 `IF NOT EXISTS` 幂等写法，可重复执行。 |

> 两个文件均已通过从零建库实跑验证（临时库 `Hospital_Test`：29 张表、种子数据 691 行、所有批次无错误）。

## 架构

- **目标版本**：SQL Server 2019+
- **数据库名**：`Hospital`
- **安全架构**：`sec`（勿用 `sys`，其为系统保留）
- **模型**：EF Core Fluent API 映射，共 **29 张表**，9 个架构（sec, mdm, fin, enc, pha, rad, lab, pat, opd）

## 快速开始

按顺序执行两个脚本即可完成建库 + 建表 + 种子数据：

```powershell
sqlcmd -S localhost -C -I -f i:65001 -U sa -P "Hospital@2024" -b -i "database\01_create_schema.sql"
sqlcmd -S localhost -C -I -f i:65001 -U sa -P "Hospital@2024" -b -i "database\02_seed_data.sql"
```

- `01` 可重复执行（建库/建架构用 `IF NOT EXISTS`，建表用 `IF OBJECT_ID ... IS NULL` 保护）。
- `02` 可重复执行（全部 `IF NOT EXISTS`），末尾会打印各表行数与达标汇总。

## 补充说明：EnsureCreated 的坑

`EnsureCreated()` **只在数据库不存在时**建全表；对已存在的库是空操作，不会增量添加新表。
`01_create_schema.sql` 就是用于手动建库建表、或补建 `EnsureCreated()` 对已存在数据库不会创建的表
（本仓库历史上因微信表缺失导致登录 500，即根因）。

若后续 EF Core 模型新增实体，需把对应 DDL 合并进 `01_create_schema.sql`，或改用 EF Core Migrations（见
[../docs/DEPLOY_STEPS.md](../docs/DEPLOY_STEPS.md)）。

## 种子数据

| 登录名 | 密码 | 角色 |
|--------|------|------|
| admin | admin123 | 系统管理员 |
| doctor | doctor123 | 门诊医生 |
| reg | reg123 | 挂号员 |
| cash | cash123 | 收费员 |
| pharm | pharm123 | 药剂师 |

包含 12 名患者（张三/李四 + P20250001~P20250010）、3 个院区、11 名员工、角色/用户/字典、
过去 14 天排班及时段（7 个每小时时段，每时段配额 5）、过去 7 天挂号数据、就诊/诊断/病历/处方/发药/收费数据。

## 执行方式示例（PowerShell）

```powershell
$server = "localhost"
$base = "e:\Demo\Cursor\Hospital\database"
$scripts = @(
  "01_create_schema.sql",
  "02_seed_data.sql"
)
foreach ($f in $scripts) {
  sqlcmd -S $server -C -I -f i:65001 -U sa -P "Hospital@2024" -b -i "$base\$f"
  if ($LASTEXITCODE -ne 0) { throw "Failed on $f" }
}
```

（ODBC Driver 18 本机开发常见证书校验失败，可加 `-C` 信任服务器证书；含索引脚本建议加 `-I` 启用 `QUOTED_IDENTIFIER`。）

## 注意事项

- 种子数据密码为明文（与 `LocalUserStore` 内存认证对应），生产需替换为真实哈希
- 已合并删除的旧脚本（`000_init_database.sql`、`010_wechat_login.sql`、`init_full.sql`、
  `900/901_seed_*.sql`、`999_verify_seed_data.sql`、`old_schema/`、`scripts/` 下 6 个一次性脚本）
  可经 git 历史找回
