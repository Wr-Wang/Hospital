# 部署命令速查

## Docker Compose

```bash
# ============================================
# 1. 构建并启动所有服务
# ============================================
docker compose up -d --build

# 查看启动日志
docker compose logs -f

# ============================================
# 2. 初始化数据库（SQL Server 就绪后执行）
# ============================================
# 建表脚本
for f in database/0*.sql; do
  echo "执行 $f ..."
  docker exec -i hospital-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "Hospital@2024" -C -I -f i:65001 \
    -i /dev/stdin < "$f"
  if [ $? -ne 0 ]; then echo "失败: $f"; exit 1; fi
done

# 种子数据
for f in database/9*.sql; do
  echo "执行 $f ..."
  docker exec -i hospital-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "Hospital@2024" -C -I -f i:65001 \
    -i /dev/stdin < "$f"
done

echo "数据库初始化完成"

# ============================================
# 3. 验证
# ============================================
curl -s -o /dev/null -w "%{http_code}" http://localhost:5075/api/campus
# 返回 401（需 JWT）说明 API 正常

# ============================================
# 4. 停止
# ============================================
docker compose down        # 保留数据卷
docker compose down -v     # 删除数据卷（丢失 DB 数据）
```

---

## Kubernetes (minikube)

```bash
# ============================================
# 0. 启动 minikube（如未运行）
# ============================================
minikube start --memory=3072

# ============================================
# 1. 构建 Docker 镜像
# ============================================
docker build -t hospital-api:latest -f src/Hospital.Api/Dockerfile .
docker build -t hospital-web:latest -f hospital-web/Dockerfile hospital-web

# 加载到 minikube
minikube image load hospital-api:latest
minikube image load hospital-web:latest

# ============================================
# 2. 注入数据库脚本 ConfigMap
# ============================================
kubectl create configmap hospital-db-scripts \
  --from-file=database/ -n hospital

# ============================================
# 3. 部署所有资源（按顺序）
# ============================================
kubectl apply -f k8s/namespace.yaml

kubectl apply -f k8s/api-secret.yaml
kubectl apply -f k8s/api-configmap.yaml

kubectl apply -f k8s/sqlserver-service.yaml
kubectl apply -f k8s/sqlserver-statefulset.yaml

# 等待 SQL Server 就绪
kubectl wait --for=condition=ready pod -l app=hospital-sqlserver \
  -n hospital --timeout=120s

# ============================================
# 4. 初始化数据库
# ============================================
SQL_POD=$(kubectl get pods -n hospital -l app=hospital-sqlserver \
  -o jsonpath='{.items[0].metadata.name}')

# 建表
for f in database/0*.sql; do
  echo "执行 $f ..."
  kubectl exec -i -n hospital "$SQL_POD" -- /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "Hospital@2024" -C -I -f i:65001 \
    -i /dev/stdin < "$f"
  if [ $? -ne 0 ]; then echo "失败: $f"; exit 1; fi
done

# 种子数据
for f in database/9*.sql; do
  echo "执行 $f ..."
  kubectl exec -i -n hospital "$SQL_POD" -- /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "Hospital@2024" -C -I -f i:65001 \
    -i /dev/stdin < "$f"
done

echo "数据库初始化完成"

# ============================================
# 5. 部署 API 和前端
# ============================================
kubectl apply -f k8s/api-deployment.yaml
kubectl apply -f k8s/api-service.yaml
kubectl apply -f k8s/web-deployment.yaml
kubectl apply -f k8s/web-service.yaml

# 等待 Pod 就绪
kubectl wait --for=condition=ready pod -l app=hospital-api \
  -n hospital --timeout=120s
kubectl wait --for=condition=ready pod -l app=hospital-web \
  -n hospital --timeout=120s

# ============================================
# 6. 端口转发（Ingress 不可用时）
# ============================================
# 开两个终端分别执行：
kubectl port-forward -n hospital svc/hospital-api 5075:8080
kubectl port-forward -n hospital svc/hospital-web 8080:80

# ============================================
# 7. 验证
# ============================================
curl -X POST http://localhost:5075/api/Authentication/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'

# 用返回的 token 测试业务接口
TOKEN="<上一步返回的token>"
curl -s http://localhost:5075/api/campus \
  -H "Authorization: Bearer $TOKEN"
curl -s http://localhost:5075/api/Department/tree/1 \
  -H "Authorization: Bearer $TOKEN"
curl -s http://localhost:5075/api/Dictionary/types \
  -H "Authorization: Bearer $TOKEN"

# ============================================
# 8. 清理
# ============================================
kubectl delete namespace hospital
minikube stop
```

---

## 代码更新后热修复部署

```bash
# 当代码修改后不想重新构建完整 Docker 镜像时：
# 1. 发布 Release 版本
dotnet publish src/Hospital.Api/Hospital.Api.csproj \
  -c Release -o /tmp/hospital-publish

# 2. 通过 minikube image build 直接构建（在 minikube 内部构建，无需网络）
minikube image build -t hospital-api:latest \
  -f src/Hospital.Api/Dockerfile .

# 3. 重启 Deployment
kubectl rollout restart -n hospital deploy/hospital-api
kubectl rollout status -n hospital deploy/hospital-api
```

---

## 已知问题与注意事项

### 连接字符串名称不匹配
ConfigMap 中设置的是 `ConnectionStrings__DefaultConnection`，但代码读取的是 `ConnectionStrings:HospitalDb`（`GetConnectionString("HospitalDb")`）。  
**需要统一**：或将 ConfigMap 改为 `ConnectionStrings__HospitalDb`，或将代码改为读取 `DefaultConnection`。

### 种子数据问题
`901~903_seed_data` 部分脚本有轻微语法错误（变量声明、外键冲突），但建表脚本（000~015）全部成功，不影响正常使用。

### enum 值与数据库不匹配
C# 枚举用中文名（如 `DepartmentType.门诊`），数据库存英文名（如 `Clinical`），需要用 `ValueConverter` 做映射。

### 测试账号
| 账号 | 密码 | 角色 |
|------|------|------|
| admin | admin123 | 系统管理员 |
| doctor | doctor123 | 门诊医生 |
