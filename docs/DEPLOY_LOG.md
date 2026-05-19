# Hospital 项目部署执行日志

> 生成时间：2026-05-19
> 执行方式：Docker Compose + Kubernetes (minikube)

---

## 一、环境检查

### 1.1 基础环境

| 项目 | 状态 | 详情 |
|------|------|------|
| Docker Desktop | ✅ 运行中 | v29.4.3 |
| minikube | ✅ 运行中 | v1.38.1, Kubernetes v1.28.3 |
| kubectl | ✅ 可用 | v1.34.1（集群 v1.28.3） |
| .NET SDK | ✅ 已安装 | v10.0（兼容 net8.0） |
| Node.js | ✅ 已安装 | v24.15.0 |

---

## 二、Docker 镜像构建

### 2.1 API 镜像 `hospital-api:latest`

- 构建方式：`docker build -t hospital-api:latest -f src/Hospital.Api/Dockerfile .`
- 基础镜像：`sdk:8.0` → `aspnet:8.0`
- 结果：✅ 构建成功

### 2.2 前端镜像 `hospital-web:latest`

- 构建方式：`docker build -t hospital-web:latest -f hospital-web/Dockerfile hospital-web`
- 基础镜像：`node:20-alpine` → `nginx:alpine`
- 构建输出：3688 modules, 1m35s
- 结果：✅ 构建成功

---

## 三、Kubernetes 部署

### 3.1 资源部署清单

| 资源 | 文件 | 状态 |
|------|------|------|
| Namespace | `k8s/namespace.yaml` | ✅ 已创建 |
| Secret | `k8s/api-secret.yaml` | ✅ 已创建 |
| ConfigMap | `k8s/api-configmap.yaml` | ✅ 已创建 |
| SQL Server StatefulSet | `k8s/sqlserver-statefulset.yaml` | ✅ 运行中 (1/1) |
| SQL Server Service | `k8s/sqlserver-service.yaml` | ✅ 已创建 |
| API Deployment | `k8s/api-deployment.yaml` | ✅ 运行中 (2/2) |
| API Service | `k8s/api-service.yaml` | ✅ 已创建 |
| Web Deployment | `k8s/web-deployment.yaml` | ✅ 运行中 (1/1) |
| Web Service | `k8s/web-service.yaml` | ✅ 已创建 |

### 3.2 数据库脚本执行

| 阶段 | 脚本数 | 状态 |
|------|--------|------|
| 建表 (000~015) | 16 个 | ✅ 全部成功 |
| 种子数据 (900~999) | 7 个 | ⚠️ 部分有误（见下） |

**种子数据问题记录：**
- `901_seed_data.sql` — StaffDepartments.DepartmentId 非空冲突
- `902_seed_data_clinical.sql` — 缺少变量 `@Campus2` 声明
- `903_seed_data_ipd_finance.sql` — LineNo 语法错误 + 缺少变量 `@DrugCef`
- `999_verify_seed_data.sql` — RowCount 语法错误

> 当前仓储使用内存实现（`AddSingleton`），数据库暂时仅为后续 EF Core 迁移做准备，上述问题不影响 API 功能。

### 3.3 Ingress 说明

`registry.k8s.io` 不可达导致 `ingress-nginx` 无法启动，改用 `port-forward` 方式访问：

```bash
# 启动两个终端分别执行：
kubectl port-forward -n hospital svc/hospital-api 5075:8080
kubectl port-forward -n hospital svc/hospital-web 8080:80
```

代理方式可选 minikube tunnel 或自行安装兼容的 ingress-controller。

---

## 四、验证结果

| 检查项 | 期望 | 实际 |
|--------|------|------|
| API Swagger | 200 | 404（Production 环境禁用） |
| API 业务接口 | 401（需 JWT） | ✅ 401 |
| 前端首页 | 200 | ✅ 200 |
| 前端 SPA 路由 | 200 | ✅ 200（nginx try_files 正常） |

---

## 五、访问方式

- **前端**：`http://localhost:8080`
- **API**：`http://localhost:5075`
- **API Swagger**：需将环境改为 Development 后可用

---

## 六、已知问题

1. `registry.k8s.io` 网络不可达 — ingress-nginx 无法安装
2. 种子数据部分脚本有语法/数据问题 — 当前无影响
3. 内存仓储 — 重启容器数据丢失
4. 资源有限 — minikube 仅分配 3072MB 内存

---

> 部署执行日期：2026-05-19
> 执行人：Claude Code + 用户协作
