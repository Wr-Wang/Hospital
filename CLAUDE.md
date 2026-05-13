# Hospital 项目规范

## 架构

- **DDD 分层架构**：Domain → Application → Infrastructure → Presentation（Api / App）
- 所有 DDD 详细规则见 [docs/DDD_RULES.md](docs/DDD_RULES.md)
- 生成新代码前先查阅该文档，确保符合分层、命名和编码规范

## 项目结构

```
src/
  Hospital.Domain        -- 领域模型（实体、值对象、聚合根、领域事件）
  Hospital.Application   -- 应用服务、DTO、仓储接口
  Hospital.Infrastructure-- 基础设施（仓储实现、外部服务）
  Hospital.Api           -- ASP.NET Core Web API
  Hospital.App           -- WPF 桌面客户端
database/                -- 数据库脚本
docs/                    -- 设计文档
```

## 快速命令

```bash
# 启动 API 后端
dotnet run --project src/Hospital.Api --launch-profile "http"

# 启动 WPF 客户端
dotnet run --project src/Hospital.App

# 构建全部
dotnet build

# 测试账号：admin / password
```

## 关键约定

- 所有 `async` 方法使用 `Async` 后缀
- 仓储操作使用 `AddAsync` / `UpdateAsync` / `DeleteAsync` / `GetByIdAsync`
- DTO 使用 `sealed record`
- Entity setter 私有化，状态变更通过业务方法
- 依赖注入通过构造器

## 当前状态

- Domain 事件已收集但未派发（待实现事件总线）
- PatientRepository 为内存模拟实现（待集成 EF Core）
- 仅实现了 Patient 边界上下文
