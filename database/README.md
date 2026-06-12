# SQL Server 数据库脚本

EF Core 自动生成的建库与建表脚本。数据库表结构由 `HospitalDbContext` + `EnsureCreated()` 管理。

## 架构

- **目标版本**：SQL Server 2019+
- **数据库名**：`Hospital`（在 `000_init_database.sql` / `init_full.sql` 中创建）
- **安全架构**：`sec`（勿用 `sys`，其为系统保留）
- **模型**：EF Core 8 Fluent API 映射，共 27 张表，9 个架构（sec, mdm, fin, enc, pha, rad, lab, pat, opd）

## 快速开始（推荐）

`init_full.sql` 是合并后的完整脚本，包含建库 + 建表 + 种子数据，一步执行：

```powershell
sqlcmd -S localhost -C -I -f i:65001 -U sa -P "Hospital@2024" -b -i "database\init_full.sql"
```

## 分步执行

如已存在数据库，可单独执行建表或种子数据：

```
000_init_database.sql   -- 创建数据库、架构、所有 EF Core 表
900_seed_minimal.sql    -- 最小演示数据（院区、科室、人员、用户、字典）
901_seed_data.sql       -- 全量测试数据（患者、排班、挂号、就诊、发药等）
999_verify_seed_data.sql-- 验证数据行数
```

旧架构脚本（001-015）已归档到 `old_schema/`。

## 辅助脚本

`scripts/` 目录包含数据填充、迁移等辅助脚本：

| 脚本 | 说明 |
|------|------|
| `seed_doctor1.py` | 医生演示数据生成 |
| `seed_schedule.py` | 排班数据生成 |
| `seed_schedule.sql` / `_final.sql` / `_fix.sql` | 排班相关 SQL 脚本 |
| `_patch_staff.py` | 员工数据修补脚本 |

## 种子数据

| 登录名 | 密码 | 角色 |
|--------|------|------|
| admin | admin123 | 系统管理员 |
| doctor | doctor123 | 门诊医生 |
| reg | reg123 | 挂号员 |
| cash | cash123 | 收费员 |
| pharm | pharm123 | 药剂师 |

包含 12 名患者（张三/李四 + P20250001~P20250010）、3 个院区、11 名员工、角色/用户/字典、过去 14 天排班及时段、过去 7 天挂号数据、就诊/诊断/病历/处方/发药/收费数据。

## 执行方式示例（PowerShell）

```powershell
$server = "localhost"
$base = "e:\Demo\Cursor\Hospital\database"
$scripts = @(
  "000_init_database.sql",
  "900_seed_minimal.sql",
  "901_seed_data.sql"
)
foreach ($f in $scripts) {
  sqlcmd -S $server -C -I -f i:65001 -U sa -P "Hospital@2024" -b -i "$base\$f"
  if ($LASTEXITCODE -ne 0) { throw "Failed on $f" }
}
```

（ODBC Driver 18 本机开发常见证书校验失败，可加 `-C` 信任服务器证书；含索引脚本建议加 `-I` 启用 `QUOTED_IDENTIFIER`。）

## 与文档差异

- 表结构由 EF Core `EnsureCreated()` 管理，不单独维护 DDL
- 种子数据密码为明文（与 `LocalUserStore` 内存认证对应），生产需替换为真实哈希
