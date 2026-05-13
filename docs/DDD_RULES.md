# DDD 代码规范

本文档定义本项目的 Domain-Driven Design 代码规则。所有新代码必须遵循以下约定。

---

## 1. 项目分层与依赖

```
Hospital.Domain          -- 纯领域模型，零外部依赖
Hospital.Application     -- 应用服务 + DTO + Repository 接口，只依赖 Domain
Hospital.Infrastructure  -- 基础设施实现，依赖 Domain + Application
Hospital.Api             -- ASP.NET Core Web API，依赖 Application + Infrastructure
Hospital.App             -- WPF 桌面客户端，依赖 Application + Infrastructure
```

**规则：**
- Domain 层不得引用任何其他项目
- Application 层只能引用 Domain 层
- Infrastructure 层可以引用 Domain 和 Application 层
- 表示层（Api / App）可以引用 Application 和 Infrastructure 层
- 严禁反向依赖（如 Domain 引用 Infrastructure）

---

## 2. Domain 层规范

### 2.1 Entity（实体）

所有实体继承 `Hospital.Domain.Entity`。

```csharp
public class YourEntity : Entity
{
    // 业务属性，仅含 getter
    public string Name { get; private set; }

    // 私有无参构造器（EF Core 要求）
    private YourEntity() { }

    // 有业务含义的构造器
    public YourEntity(string name)
    {
        Name = name;
    }
}
```

**规则：**
- 继承 `Entity`（提供 `Id` + 基于身份的比较）
- `Id` 由 `Entity` 基类提供（`long` 类型，`protected set`）
- 属性 setter 为 `private`，禁止公开 setter
- 通过构造器或业务方法修改状态
- 保留私有无参构造器供 EF Core 使用
- 覆盖 `Equals`/`GetHashCode` 由基类统一实现，子类不需要重复

### 2.2 Aggregate Root（聚合根）

聚合根继承 `Hospital.Domain.AggregateRoot`。

```csharp
public class YourAggregate : AggregateRoot
{
    private readonly List<ChildEntity> _children = new();

    public IReadOnlyCollection<ChildEntity> Children => _children.AsReadOnly();

    private YourAggregate() { }

    public YourAggregate(string name) : this()
    {
        // 构造时引发领域事件
        AddDomainEvent(new YourAggregateCreatedEvent(Id, name));
    }

    public void AddChild(string childName)
    {
        var child = new ChildEntity(childName);
        _children.Add(child);
        AddDomainEvent(new ChildAddedEvent(Id, child.Id));
    }
}
```

**规则：**
- 聚合根是外部访问的唯一入口
- 子实体只能通过聚合根操作，禁止外部直接引用子实体集合引用（返回 `IReadOnlyCollection<T>`）
- 聚合边界内保证最终一致性
- 跨聚合修改使用领域事件
- 状态变更方法命名体现业务意图（如 `AddConsent()`、`MergeWith()`），而非 `SetPropertyX()`
- 聚合根构造器和业务方法中引发领域事件

### 2.3 Value Object（值对象）

```csharp
public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty", nameof(value));
        // 可扩展格式校验
        Value = value;
    }

    public bool Equals(PhoneNumber? other) =>
        other is not null && StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public override bool Equals(object? obj) => obj is PhoneNumber other && Equals(other);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;

    public static bool operator ==(PhoneNumber? left, PhoneNumber? right) =>
        left is null ? right is null : left.Equals(right);
    public static bool operator !=(PhoneNumber? left, PhoneNumber? right) => !(left == right);
}
```

**规则：**
- 不可变（`init` 或 `readonly` 属性 + 构造器只赋值一次）
- 实现 `IEquatable<T>`，重写 `Equals`/`GetHashCode`/`ToString`/`==`/`!=`
- 基于属性值比较（而非身份）
- 构造器中执行自验证，无效状态拒绝创建
- `sealed class` 或 `readonly record struct`
- 简单的枚举值使用 `enum`（如 `Gender`）

### 2.4 Domain Event（领域事件）

```csharp
public sealed class PatientCreatedEvent : DomainEvent
{
    public long PatientId { get; }
    public string PatientNo { get; }

    public PatientCreatedEvent(long patientId, string patientNo)
    {
        PatientId = patientId;
        PatientNo = patientNo;
        // OccurredOn 由基类在构造时自动设为 UtcNow
    }
}
```

**规则：**
- 继承 `DomainEvent`（`OccurredOn` 由基类自动赋值）
- 命名为 `{实体}{过去时动作}Event`（如 `PatientCreatedEvent`、`PatientMergedEvent`）
- 承载事件发生时所需的业务数据（`Id` 或其他上下文）
- 不可变（`get` only 属性）
- 事件仅记录"已发生的事实"，不包含处理逻辑

### 2.5 Domain Service（领域服务）

**适用场景：** 当业务逻辑跨越多个聚合或无法归属于单一实体时。

```csharp
// 定义在 Domain 层（而非 Application 层）
public interface ISomeDomainService
{
    bool CanPerformAction(EntityA a, EntityB b);
}
```

**规则：**
- 定义在 `Hospital.Domain` 项目内
- 接口命名贴近业务概念（而非技术概念）
- 实现放在 Infrastructure 层
- 仅包含无副作用的业务判断

---

## 3. Application 层规范

### 3.1 Application Service（应用服务）

```csharp
public sealed class PatientApplicationService : IPatientApplicationService
{
    private readonly IPatientRepository _repository;

    public PatientApplicationService(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> CreateAsync(CreatePatientDto request)
    {
        // 1. DTO → 领域对象
        var patient = new Patient(request.PatientNo, request.Name, ...);

        // 2. 调用仓储持久化
        await _repository.AddAsync(patient);

        // 3. 派发领域事件（待实现）
        // DispatchDomainEvents(patient);

        // 4. 返回结果
        return patient.Id;
    }
}
```

**规则：**
- 编排任务：接收 DTO → 调用领域对象/仓储 → 派发事件 → 返回结果
- 不含业务逻辑（业务逻辑在领域层）
- 事务边界：一个方法对应一个完整用例
- 接口定义在 `Hospital.Application`，实现也在 `Hospital.Application`
- 方法命名体现用户意图（`CreateAsync`、`GetByIdAsync`），而非技术操作

### 3.2 DTO（数据传输对象）

```csharp
public sealed record PatientDto(
    long Id,
    string PatientNo,
    string Name,
    string? Gender,
    string? BirthDate,
    string? Phone,
    string? AllergiesText,
    string? IdCard);

public sealed record CreatePatientDto(string PatientNo, string Name, ...);
```

**规则：**
- 使用 `sealed record`（不可变，值比较，解构支持）
- 扁平的纯数据结构，不含行为
- 与领域对象一一对应但不相同（DTO 可以展平/组合多个领域对象）
- 命名为 `{Entity}Dto`（查询）或 `Create{Entity}Dto`（命令）
- 可选字段用 `string?` / `T?` 标记
- **严禁**在 Application 层以外引用领域对象（如将 `Patient` 直接返回给 Controller）

### 3.3 Repository 接口

```csharp
public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(long id);
    Task<Patient?> GetByPatientNoAsync(string patientNo);
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(long id);
}
```

**规则：**
- 定义在 `Hospital.Application` 层
- 接口方法使用领域对象（而非 DTO）
- 命名体现仓储语义（`AddAsync` / `UpdateAsync` / `DeleteAsync` / `GetByIdAsync`）
- 查询方法返回 `T?`（可能不存在）
- 方法名不用 `Save` / `Fetch`，使用 DDD 标准命名

---

## 4. Infrastructure 层规范

### 4.1 Repository 实现

```csharp
public sealed class PatientRepository : IPatientRepository
{
    public Task<Patient?> GetByIdAsync(long id) { ... }
    public Task AddAsync(Patient patient) { ... }
    public Task UpdateAsync(Patient patient) { ... }
    public Task DeleteAsync(long id) { ... }
}
```

**规则：**
- 实现 `Application` 层定义的接口
- 内部可自由选择持久化技术（EF Core、Dapper、内存等）
- 负责领域对象与持久化格式的双向映射
- 保持无状态（Scoped 或 Transient 生命周期）

### 4.2 外部服务

```csharp
// ApiClient 等基础设施服务
public sealed class ApiClient : IApiClient { ... }
```

**规则：**
- 接口定义在 `Hospital.Application` 或 `Hospital.Infrastructure`（视情况）
- 实现封装具体技术细节（HttpClient、文件系统、第三方 SDK）
- 异常应转换为领域可理解的类型，而非暴露基础设施异常

---

## 5. 命名约定

| 元素 | 约定 | 示例 |
|------|------|------|
| 项目 | `Hospital.{Layer}` | `Hospital.Domain` |
| 命名空间 | `Hospital.{Layer}.{SubFolder}` | `Hospital.Domain.Aggregates.Patient` |
| 实体类 | 单数名词 | `Patient`, `PatientConsent` |
| 值对象 | 单数名词 | `IdCard`, `PhoneNumber` |
| 聚合根 | 继承 `AggregateRoot` 的实体 | `Patient` |
| 领域事件 | `{Entity}{过去时动作}Event` | `PatientCreatedEvent`, `PatientMergedEvent` |
| 应用服务 | `{Entity}ApplicationService` | `PatientApplicationService` |
| 应用服务接口 | `I{Entity}ApplicationService` | `IPatientApplicationService` |
| 领域服务接口 | `I{业务概念}Service` | `IAuthenticationService` |
| 仓储接口 | `I{Entity}Repository` | `IPatientRepository` |
| 仓储实现 | `{Entity}Repository` | `PatientRepository` |
| DTO | `{Entity}Dto` / `Create{Entity}Dto` | `PatientDto`, `CreatePatientDto` |
| 文件夹结构 | 按业务概念/聚合分组 | `Aggregates/Patient/`, `Services/Authentication/` |

---

## 6. 编码纪律

### 6.1 必须遵守

- **setter 私有化：** 所有 Entity/ValueObject 属性 `get` 公开、`set` 私有（或 `init`）
- **构造即有效：** 构造器参数验证，无效状态拒绝创建
- **业务方法封装：** 状态变更通过业务方法（`AddConsent()`），不通过 setter
- **聚合边界：** 外部只能通过 Aggregate Root 操作聚合内的实体
- **集合封装：** 子实体集合对外暴露 `IReadOnlyCollection<T>`，禁止暴露 `List<T>` 或 `IEnumerable<T>`
- **事件驱动：** 跨聚合的副作用通过领域事件实现，禁止直接在方法内修改多个聚合
- **依赖注入：** 所有服务通过构造器注入，禁止 `new` 创建服务类
- **异步优先：** 所有 I/O 操作使用 `async Task` / `ValueTask`

### 6.2 禁止行为

| ❌ 禁止 | ✅ 替代 |
|---------|--------|
| Entity 公开 setter | 构造器或业务方法 |
| 跨聚合直接修改 | 领域事件 |
| Application Service 包含业务逻辑 | 委托给领域对象 |
| 将领域对象直接暴露给 Controller | DTO 映射 |
| Infrastructure 层定义接口 | 定义在 Application 或 Domain |
| 仓储返回 DTO | 仓储返回领域对象 |
| 使用 `new` 实例化服务 | 依赖注入 |
| 值对象创建后修改 | 创建时确定，不可变 |
| `async void`（事件处理器除外） | `async Task` |

### 6.3 领域事件派发（待办事项）

当前领域事件收集在 `AggregateRoot.DomainEvents` 中但未被派发。后续实现事件总线时应：

1. Application Service 在 `AddAsync`/`UpdateAsync` 后读取 `aggregate.DomainEvents`
2. 通过事件总线派发对应的事件处理器
3. 调用 `aggregate.ClearDomainEvents()`
4. 事件处理器位于 Application 或 Infrastructure 层，依赖注入

---

## 7. 包引用规则

| 层 | 允许的包 |
|----|---------|
| Domain | 仅 .NET 基础类库（`System.*`） |
| Application | `Hospital.Domain` + 仅 .NET 基础类库 |
| Infrastructure | `Hospital.Domain` + `Hospital.Application` + EF Core / Dapper / HttpClient 等基础设施包 |
| Api | `Hospital.Application` + `Hospital.Infrastructure` + ASP.NET Core 包 + Swagger |
| App | `Hospital.Application` + `Hospital.Infrastructure` + WPF / CommunityToolkit.Mvvm + DI 容器 |

---

## 8. 文件组织

```
src/Hospital.Domain/
  Entity.cs                        -- 基类
  AggregateRoot.cs                 -- 基类
  Events/
    DomainEvent.cs                 -- 基类
  ValueObjects/
    {ValueObject}.cs               -- 值对象，每个文件一个
  Aggregates/
    {BoundedContext}/
      {AggregateRoot}.cs           -- 聚合根
      {ChildEntity}.cs             -- 子实体
      Events/
        {Entity}{Action}Event.cs   -- 领域事件，每个文件一个

src/Hospital.Application/
  DTOs/
    {Entity}DTOs.cs                -- 相关 DTO 集中在一个文件
  Repositories/
    I{Entity}Repository.cs         -- 仓储接口，每个文件一个
  Services/
    {Entity}/
      I{Entity}ApplicationService.cs
      {Entity}ApplicationService.cs

src/Hospital.Infrastructure/
  Repositories/
    {Entity}Repository.cs          -- 仓储实现
  ExternalServices/
    {Service}.cs                   -- 外部服务实现
```

---

## 9. 边界上下文（Bounded Context）规划

当前只实现了 `Patient` 上下文，后续新增应遵循以下映射：

| BC | Aggregates | 项目位置 |
|----|-----------|---------|
| Patient | Patient | `Domain/Aggregates/Patient/` |
| 后续 BC | ... | `Domain/Aggregates/{BC}/` |

每个 BC 在 Domain 中拥有独立文件夹，Application 中拥有独立的 Services 子文件夹。

---

> **原则：** 所有代码应先写在该放的位置，再写实现。不确定归属时问：*"如果换掉持久化层/UI/框架，这段代码需要改吗？"* 需要 → Infrastructure；不需要 → Domain 或 Application。
