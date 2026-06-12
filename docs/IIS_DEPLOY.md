# Hospital API — IIS 部署指南 (Windows 11)

## 环境概览

| 项目 | 值 |
|------|-----|
| 目标框架 | net10.0 (LTS) |
| 数据库 | SQL Server (EF Core + Windows 身份认证) |
| 认证 | JWT Bearer |
| API 文档 | Swagger (仅在 Development 环境启用) |

---

## 1. 安装 IIS

打开 **控制面板 → 程序和功能 → 启用或关闭 Windows 功能**，勾选以下组件：

- **Internet Information Services**
  -  Web 管理工具
     -  IIS 管理控制台
  -  万维网服务
     -  常见 HTTP 功能 → 全部勾选
     -  应用程序开发功能
        -  ASP.NET 4.8（可选）
        -  ISAPI 扩展
        -  ISAPI 筛选器
     -  安全性 → 勾选"Windows 身份认证"（如需集成认证）

或使用 PowerShell（**管理员**）：

```powershell
# 安装 IIS 核心组件
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All

# 安装 Windows 身份认证（可选，项目为匿名认证）
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WindowsAuthentication
```

安装完成后访问 `http://localhost` 确认 IIS 默认页面正常显示。

---

## 2. 安装 .NET 10 Hosting Bundle

IIS 通过 **ASP.NET Core Module (ANCM)** 反向代理到 Kestrel，需要安装 Hosting Bundle。

> **下载地址**：https://dotnet.microsoft.com/download/dotnet/10.0
>
> 选择 **ASP.NET Core 10.0 Runtime** 或 **.NET Desktop Runtime**，
> 实际对应文件名为 `dotnet-hosting-10.0.x-win.exe`

安装完成后重启终端，验证运行时已安装：

```powershell
dotnet --list-runtimes
```

应看到类似输出：

```
Microsoft.AspNetCore.App 10.0.x [path]
Microsoft.NETCore.App 10.0.x [path]
```

---

## 3. 发布项目

### 3.1 方式 A：终端发布（推荐）

在项目根目录执行：

```bash
dotnet publish src/Hospital.Api -c Release -o publish/iis
```

参数说明：

| 参数 | 含义 |
|------|------|
| `-c Release` | 发布 Release 配置 |
| `-o publish/iis` | 输出目录（路径可自定义） |

### 3.2 方式 B：指定运行时独立部署

如需指定目标运行时（不依赖服务器已安装的运行时）：

```bash
dotnet publish src/Hospital.Api -c Release -o publish/iis ^
  --runtime win-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=true
```

注意：

- `self-contained` 会包含运行时文件，输出体积较大（约 100MB+）
- `PublishSingleFile` 会合并为单文件，首次启动稍慢

### 3.3 方式 C：Visual Studio 图形界面发布

1. 右键 `Hospital.Api` → **发布**
2. 选择 **文件夹** 目标
3. 配置路径（如 `bin/Release/net10.0/publish`）
4. 点击 **发布**

---

## 4. 配置 IIS 站点

### 4.1 复制发布文件

将发布输出目录（如 `publish/iis/`）复制到 IIS 站点目录，例如：

```
C:\inetpub\wwwroot\hospital-api\
```

确保 IIS 进程帐户（`IIS_IUSRS` 或 `ApplicationPoolIdentity`）对该目录有**读取/执行**权限。

### 4.2 创建应用程序池

1. 打开 **IIS 管理器**
2. 左侧 **应用程序池** → **添加应用程序池**
3. 名称：`HospitalApiPool`
4. **.NET CLR 版本**：选择 **无托管代码（No Managed Code）**
   > ASP.NET Core 不依赖 IIS 的 CLR，由 ANCM 托管，必须选"无托管代码"
5. 托管管道模式：**集成（Integrated）**

### 4.3 创建网站或应用程序

**方式 A — 新建网站（推荐）：**

1. IIS 管理器 → **网站** → **添加网站**
2. 网站名称：`HospitalApi`
3. 应用程序池：`HospitalApiPool`
4. 物理路径：`C:\inetpub\wwwroot\hospital-api`
5. 绑定：`http` / 所有未分配 / 端口 `8080`（或任意未被占用的端口）
6. 确定

**方式 B — 添加到现有网站：**

1. 右键现有网站 → **添加应用程序**
2. 别名：`hospital-api`
3. 应用程序池：`HospitalApiPool`
4. 物理路径：`C:\inetpub\wwwroot\hospital-api`

### 4.4 配置 web.config（自动生成）

`dotnet publish` 会自动在输出目录生成 `web.config`，内容类似：

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*"
             modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet"
                  arguments=".\Hospital.Api.dll"
                  stdoutLogEnabled="false"
                  stdoutLogFile=".\logs\stdout"
                  hostingModel="inprocess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
```

如需启用 stdout 日志（调试用），将 `stdoutLogEnabled` 改为 `true` 并确保 `logs` 目录有写入权限。

---

## 5. 配置数据库连接

编辑目标目录下的 `appsettings.json`：

```json
{
  "ConnectionStrings": {
    "HospitalDb": "Server=localhost;Database=Hospital;Integrated Security=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "SecretKey": "你的生产密钥_至少32位字符_建议用随机字符串",
    "Issuer": "HospitalApi",
    "Audience": "HospitalApp",
    "ExpirationHours": 24
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5000"
      }
    }
  }
}
```

> **⚠️ 生产环境注意事项：**
>
> - `SecretKey` **必须更换**为随机长字符串（至少 32 位），不要使用默认开发密钥
> - SQL Server 默认使用 Windows 集成认证。如果 IIS 和 SQL Server 在不同机器，使用 SQL Server 账号：
>   ```
>   "Server=192.168.x.x;Database=Hospital;User Id=sa;Password=***;TrustServerCertificate=True"
>   ```
> - Kestrel 端点端口需与 IIS 站点端口保持一致，或让 ANCM 自动协商

---

## 6. 设置目录权限

```powershell
# 为 IIS 应用程序池标识添加权限
icacls "C:\inetpub\wwwroot\hospital-api" /grant "IIS AppPool\HospitalApiPool:(RX)"
icacls "C:\inetpub\wwwroot\hospital-api" /grant "IIS_IUSRS:(RX)"
```

如果启用了 stdout 日志：

```powershell
mkdir C:\inetpub\wwwroot\hospital-api\logs
icacls "C:\inetpub\wwwroot\hospital-api\logs" /grant "IIS AppPool\HospitalApiPool:(M)"
```

---

## 7. 启动并验证

### 7.1 启动网站

1. IIS 管理器 → **应用程序池** → 选中 `HospitalApiPool` → **启动**
2. IIS 管理器 → **网站** → 选中 `HospitalApi` → **启动**

或使用 PowerShell：

```powershell
# 启动应用程序池
Start-WebAppPool -Name "HospitalApiPool"

# 启动网站
Start-Website -Name "HospitalApi"
```

### 7.2 测试接口

浏览器访问：

```
http://localhost:8080/swagger
```

> ⚠️ Swagger 仅在 Development 环境启用。生产环境需要在 `web.config` 中将 `ASPNETCORE_ENVIRONMENT` 设为 `Development` 才能查看 Swagger，或通过其他方式（如反向代理规则）限制 Swagger 的访问来源。

也可以测试健康端点：

```powershell
curl http://localhost:8080/api/weatherforecast
```

### 7.3 查看日志

如果站点无法启动，检查：

1. **Windows 事件查看器** → Windows 日志 → 应用程序（来源：IIS AspNetCore Module）
2. web.config 中启用 `stdoutLogEnabled="true"` 后查看 `logs/stdout_*.log`
3. 发布目录下的 `appsettings.json` 是否正确

---

## 8. 常见问题

### 8.1 HTTP Error 500.35 — ANCM 进程启动失败

**原因**：服务器未安装 .NET 10.0 Runtime 或版本不匹配。

**解决**：运行 `dotnet --info` 确认运行时版本 ≥ 10.0.x，重新安装 Hosting Bundle。

### 8.2 HTTP Error 502.5 — 进程崩溃

**原因**：应用程序启动时发生致命错误，通常为配置文件错误、端口冲突或代码异常。

**解决**：

1. 启用 stdout 日志查看详细异常
2. 直接运行 `dotnet Hospital.Api.dll` 看控制台输出
3. 检查 `appsettings.json` 数据库连接字符串

### 8.3 访问 Swagger 返回 404

**原因**：生产环境（Production）默认不启用 Swagger。

**解决**：临时将 `web.config` 中 `ASPNETCORE_ENVIRONMENT` 改为 `Development` 后重启站点，验证完后再改回。或直接调用业务接口进行验证。

### 8.4 数据库连接失败

**原因**：SQL Server 未启动、连接字符串配置错误、或 IIS 进程账户无权访问数据库。

**解决**：

- SQL Server Management Studio 确认数据库可达
- 确认连接字符串中的 `Server` 地址正确
- 使用 `User Id / Password` 替代 Windows 集成认证（跨机器场景）
- 为 SQL Server 启用 TCP/IP 协议（SQL Server 配置管理器）

### 8.5 端口冲突

**解决**：在 `appsettings.json` 中修改 Kestrel 端口，或在 IIS 绑定中修改端口。

---

## 9. 快速部署脚本

将以下脚本保存为 `deploy-iis.ps1`，以**管理员身份**执行：

```powershell
# Hospital API IIS 一键部署脚本
$projectPath = "src/Hospital.Api"
$publishPath = "publish/iis"
$iisPath = "C:\inetpub\wwwroot\hospital-api"
$siteName = "HospitalApi"
$poolName = "HospitalApiPool"

# 1. 发布
Write-Host ">>> 发布项目..." -ForegroundColor Green
dotnet publish $projectPath -c Release -o $publishPath
if ($LASTEXITCODE -ne 0) { throw "发布失败" }

# 2. 停止旧网站/池
Write-Host ">>> 停止旧站点..." -ForegroundColor Green
if (Get-Website -Name $siteName) { Stop-Website -Name $siteName }
if (Get-WebAppPoolState -Name $poolName) { Stop-WebAppPool -Name $poolName }

# 3. 复制文件
Write-Host ">>> 复制发布文件..." -ForegroundColor Green
if (Test-Path $iisPath) { Remove-Item "$iisPath\*" -Recurse -Force }
Copy-Item "$publishPath\*" $iisPath -Recurse

# 4. 创建应用程序池
Write-Host ">>> 配置应用程序池..." -ForegroundColor Green
if (-not (Get-WebAppPoolState -Name $poolName -ErrorAction SilentlyContinue)) {
  New-WebAppPool -Name $poolName
}
Set-ItemProperty "IIS:\AppPools\$poolName" managedRuntimeVersion ""

# 5. 创建/更新网站
Write-Host ">>> 配置网站..." -ForegroundColor Green
if (-not (Get-Website -Name $siteName -ErrorAction SilentlyContinue)) {
  New-Website -Name $siteName -Port 8080 -PhysicalPath $iisPath -ApplicationPool $poolName
} else {
  Set-ItemProperty "IIS:\Sites\$siteName" physicalPath $iisPath
  Set-ItemProperty "IIS:\Sites\$siteName" applicationPool $poolName
}

# 6. 启动
Write-Host ">>> 启动..." -ForegroundColor Green
Start-WebAppPool -Name $poolName
Start-Website -Name $siteName

Write-Host ">>> 部署完成！访问 http://localhost:8080/swagger" -ForegroundColor Green
```

---

## 参考链接

- [Host ASP.NET Core on IIS](https://learn.microsoft.com/aspnet/core/host-and-deploy/iis)
- [ASP.NET Core Module 配置参考](https://learn.microsoft.com/aspnet/core/host-and-deploy/aspnet-core-module)
- [.NET 10 下载](https://dotnet.microsoft.com/download/dotnet/10.0)
