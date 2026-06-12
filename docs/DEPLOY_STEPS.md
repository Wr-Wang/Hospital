# Hospital 项目部署指南

> 本文档包含 Docker Compose 和 Kubernetes 两种部署方式的完整步骤。
> 适用场景：本地开发与测试（本地镜像 + 本地 SQL Server 容器）。

---

## 一、环境要求

| 工具 | 版本要求 | 当前状态 |
|------|----------|----------|
| Docker Desktop | ≥ 24.x | ✅ 已安装 v29.4.3，**需手动启动** |
| .NET SDK | 8.0+ | ✅ 已安装 v10.0（兼容 net8.0） |
| Node.js | 18.x+ | 需确认 |
| kubectl | ≥ 1.28 | ✅ 已安装 v1.34.1 |
| minikube **或** kind | 任选其一 | ❌ 未安装（K8s 部署需要） |

---

## 二、部署文件清单

### Docker 层（6 个）

| 文件 | 用途 |
|------|------|
| `.dockerignore` | 过滤构建上下文（bin/obj/node_modules） |
| `src/Hospital.Api/Dockerfile` | API 多阶段构建（sdk:8.0 → aspnet:8.0） |
| `hospital-web/Dockerfile` | 前端构建（node:20 → nginx:alpine） |
| `hospital-web/nginx.conf` | nginx 配置：SPA 路由 + `/api` 反代到后端 |
| `hospital-web/.dockerignore` | 过滤前端构建上下文（node_modules） |
| `docker-compose.yml` | 本地三容器编排（sqlserver + api + web） |

### K8s 层（10 个，在 `k8s/` 目录）

| 文件 | 用途 |
|------|------|
| `namespace.yaml` | 命名空间 `hospital` |
| `api-secret.yaml` | JWT 密钥 + SA 密码 |
| `api-configmap.yaml` | 连接字符串 + ASPNETCORE 环境变量 |
| `api-deployment.yaml` | API 部署（2 副本、tcpSocket 探针、资源限制） |
| `api-service.yaml` | API ClusterIP 服务（端口 8080） |
| `sqlserver-statefulset.yaml` | SQL Server 有状态部署（10Gi PVC + 脚本挂载） |
| `sqlserver-service.yaml` | SQL Server Headless 服务 |
| `web-deployment.yaml` | 前端部署（1 副本） |
| `web-service.yaml` | 前端 ClusterIP 服务（端口 80） |
| `ingress.yaml` | 域名路由：`/api` → API，其余 → 前端 |

---

## 三、Docker Compose 部署

### 3.1 启动 Docker Desktop

```bash
# 确保 Docker daemon 运行中
docker info
```

### 3.2 确认代码修改（已处理）

`src/Hospital.Api/Program.cs` 中 `app.UseHttpsRedirection()` 已注释掉，容器部署不会出现 HTTPS 重定向问题。

> 或者添加转发的标头中间件来适配反向代理场景，但注释掉这行是最简洁的方式。

### 3.3 构建并启动

```bash
# 构建并后台启动所有服务
docker compose up -d --build

# 查看启动日志
docker compose logs -f
```

### 3.4 初始化数据库

SQL Server 容器启动后，按数字顺序执行数据库脚本：

```bash
# 查看 SQL Server 是否就绪
docker logs hospital-sqlserver

# 执行建库脚本（000 ~ 015）
for f in database/0*.sql; do
  echo "执行 $f ..."
  docker exec -i hospital-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "Hospital@2024" -C -I -f i:65001 \
    -i /dev/stdin < "$f"
  if [ $? -ne 0 ]; then
    echo "失败: $f"
    exit 1
  fi
done

# 执行种子数据（900 ~ 999）
for f in database/9*.sql; do
  echo "执行 $f ..."
  docker exec -i hospital-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "Hospital@2024" -C -I -f i:65001 \
    -i /dev/stdin < "$f"
done

echo "数据库初始化完成"
```

> 手动逐条执行示例：
> ```bash
> docker exec -i hospital-sqlserver /opt/mssql-tools18/bin/sqlcmd \
>   -S localhost -U sa -P "Hospital@2024" -C -I -f i:65001 \
>   -i /dev/stdin < database/000_init_database.sql
> ```

### 3.5 验证部署

```bash
# 确认三个容器都在运行
docker ps

# 测试 API（Swagger）
curl http://localhost:5075/swagger/index.html

# 测试 API 业务接口（返回 401 说明有 JWT 鉴权，接口可通即正常）
curl -s -o /dev/null -w "%{http_code}" http://localhost:5075/api/campus

# 测试前端（应为 SPA 页面）
curl -s -o /dev/null -w "%{http_code}" http://localhost:8080
```

浏览器访问：
- 前端：http://localhost:8080
- API Swagger：http://localhost:5075/swagger/index.html

### 3.6 停止与清理

```bash
# 停止容器
docker compose down

# 停止并删除数据卷（会丢失数据库数据）
docker compose down -v
```

---

## 四、Kubernetes 部署

### 4.1 安装本地 K8s 集群（二选一）

```bash
# 选项 A：安装 minikube
# 下载：https://minikube.sigs.k8s.io/docs/start/
minikube start --cpus 4 --memory 8192

# 选项 B：安装 kind
# 下载：https://kind.sigs.k8s.io/docs/user/quick-start/
kind create cluster --name hospital
```

### 4.2 确认代码修改（已处理）

`Program.cs` 中 `UseHttpsRedirection()` 已注释掉，无需额外操作。

### 4.3 构建 Docker 镜像

```bash
# 构建 API 镜像（构建上下文为项目根目录）
docker build -t hospital-api:latest -f src/Hospital.Api/Dockerfile .

# 构建前端镜像（构建上下文为 hospital-web 目录）
docker build -t hospital-web:latest -f hospital-web/Dockerfile hospital-web
```

### 4.4 导入镜像到本地 K8s 集群

```bash
# minikube 方式
minikube image load hospital-api:latest
minikube image load hospital-web:latest

# kind 方式
kind load docker-image hospital-api:latest
kind load docker-image hospital-web:latest
```

### 4.5 注入数据库脚本 ConfigMap

```bash
kubectl create configmap hospital-db-scripts \
  --from-file=database/ \
  -n hospital
```

> 注意：此步骤需在部署 StatefulSet 之前执行，否则 Pod 因找不到 ConfigMap 而启动失败。

### 4.6 部署所有资源

```bash
# 创建命名空间
kubectl apply -f k8s/namespace.yaml

# 创建 Secret 和 ConfigMap
kubectl apply -f k8s/api-secret.yaml
kubectl apply -f k8s/api-configmap.yaml

# 部署 SQL Server（需等待 Pod Ready）
kubectl apply -f k8s/sqlserver-service.yaml
kubectl apply -f k8s/sqlserver-statefulset.yaml

# 查看 SQL Server 启动进度
kubectl get pods -n hospital -w
# 等待显示 Ready（约 1-2 分钟）
```

### 4.7 初始化 SQL Server 数据库

```bash
# 确认 SQL Server Pod 名称
SQL_POD=$(kubectl get pods -n hospital -l app=hospital-sqlserver -o jsonpath='{.items[0].metadata.name}')

# 按顺序执行建库脚本
for f in database/0*.sql; do
  echo "执行 $f ..."
  kubectl exec -i -n hospital "$SQL_POD" -- /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "Hospital@2024" -C -I -f i:65001 \
    -i /dev/stdin < "$f"
  if [ $? -ne 0 ]; then
    echo "失败: $f"
    exit 1
  fi
done

# 执行种子数据
for f in database/9*.sql; do
  echo "执行 $f ..."
  kubectl exec -i -n hospital "$SQL_POD" -- /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "Hospital@2024" -C -I -f i:65001 \
    -i /dev/stdin < "$f"
done
```

### 4.8 部署 API 和前端

```bash
# 部署 API
kubectl apply -f k8s/api-deployment.yaml
kubectl apply -f k8s/api-service.yaml

# 部署前端
kubectl apply -f k8s/web-deployment.yaml
kubectl apply -f k8s/web-service.yaml

# 部署 Ingress（可选，如无 ingress controller 可用 port-forward）
kubectl apply -f k8s/ingress.yaml
```

### 4.9 验证部署

```bash
# 查看所有 Pod 状态
kubectl get pods -n hospital

# 查看所有 Service
kubectl get svc -n hospital

# 查看所有 ConfigMap
kubectl get cm -n hospital
```

### 4.10 访问服务

**方式一：port-forward（无需 ingress controller）**

```bash
# 新开两个终端，分别执行：
kubectl port-forward -n hospital svc/hospital-api 5075:8080
kubectl port-forward -n hospital svc/hospital-web 8080:80
```

浏览器访问：
- 前端：http://localhost:8080
- API Swagger：http://localhost:5075/swagger/index.html（需确认环境为 Development）

**方式二：Ingress（需安装 nginx-ingress-controller）**

```bash
# 安装 ingress controller
minikube addons enable ingress
# 或：kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/kind/deploy.yaml

# 添加 hosts 记录（Windows: C:\Windows\System32\drivers\etc\hosts）
# 127.0.0.1 hospital.local

# 浏览器访问
# http://hospital.local
```

### 4.11 清理 K8s 资源

```bash
# 删除命名空间（会级联删除所有资源）
kubectl delete namespace hospital

# 删除数据库脚本 ConfigMap（保留给其他命名空间使用）
kubectl delete configmap hospital-db-scripts -n hospital

# 停止 minikube
minikube stop

# 删除 kind 集群
kind delete cluster --name hospital
```

---

## 五、注意事项

### 5.1 HTTPS 重定向（已处理）

`Program.cs` 中 `UseHttpsRedirection()` 已注释掉，由前置的反向代理（nginx / ingress）负责 HTTPS 终止。如需直接公网暴露，可取消注释并配置 Kestrel HTTPS 证书。

### 5.2 环境变量与 Swagger

- **Docker Compose** 默认使用 `Development` 环境，Swagger UI 可访问
- **K8s ConfigMap** 默认使用 `Production` 环境，Swagger 被禁用
- 如需在 K8s 中启用 Swagger，修改 `k8s/api-configmap.yaml` 中 `ASPNETCORE_ENVIRONMENT` 为 `Development`

### 5.3 连接字符串说明

当前连接字符串指向容器内的 SQL Server：

```
Server=sqlserver;Database=Hospital;User Id=sa;Password=Hospital@2024;TrustServerCertificate=True
```

但仓储实现目前是**内存 Singleton**（`AddSingleton<IPatientRepository, PatientRepository>`），容器重启会导致数据丢失。后续接入 EF Core 后连接字符串才会生效，届时需将仓储注册改为：

```csharp
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPatientRepository, EfPatientRepository>();
```

### 5.4 数据库脚本执行顺序

`database/` 下的脚本按文件名数字升序执行：

```
建表阶段：000 → 001 → 002 → ... → 015
种子数据：900 → 901 → 902 → 903 → 904 → 999
```

### 5.5 安全性（本地开发用）

部署文件中包含的密码和密钥仅用于本地开发：

| 配置项 | 当前值 | 说明 |
|--------|--------|------|
| SA_PASSWORD | Hospital@2024 | SQL Server 管理员密码 |
| JWT SecretKey | ThisIsADevelopmentSecretKey_... | JWT 签名密钥 |

**上线前必须更换**为强密钥，并通过 K8s Secret 管理。

### 5.6 K8s 本地集群选择

| 工具 | 优点 | 缺点 |
|------|------|------|
| **minikube** | 功能完整，内置 dashboard、ingress 插件 | 资源占用较大 |
| **kind** | 启动快，资源占用少，适合 CI | 功能较 minikube 少 |

建议本地开发使用 **minikube**，CI/CD 场景使用 **kind**。

### 5.7 常见问题排查

| 问题 | 原因 | 解决方案 |
|------|------|----------|
| API 返回 307 | UseHttpsRedirection 强制跳转 HTTPS | 已注释掉，如重现可检查 `Program.cs` |
| SQL Server Pod CrashLoopBackOff | SA 密码不符合复杂度要求 | 使用 `Hospital@2024` 含特殊字符 |
| 前端白屏/404 | SPA 路由未 fallback 到 index.html | 确认 nginx.conf 中 `try_files` 配置 |
| API 连接数据库失败 | SQL Server 未就绪即启动 API | `depends_on` + healthcheck 确保顺序 |
| Swagger 返回 404 | 生产环境禁用了 Swagger | 将环境改为 Development |

---

> 本文档自动生成 — 所有部署文件位于项目 `k8s/` 目录和 `docker-compose.yml`。
> 数据库脚本位于 `database/` 目录，按文件名数字升序执行。
