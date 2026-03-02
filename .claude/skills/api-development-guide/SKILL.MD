---
name: api-development-guide
description: 遵守此规范生成项目代码
---

# 开发规范指南

#### 分层例子
```text
├── 📂 aspire
│   ├── 🚀 AppHost          # Aspire 编排层：负责数据库容器、API服务分发及环境变量配置
│   └── 🛠️ ServiceDefaults  # 基础设施默认配置：包含 OpenTelemetry、健康检查、弹性重试等
│
├── 📂 src
│   ├── 🏛️ Domain                                     # 领域层(领域集合)
│   │   ├── 📂 DatabaseConnections                    # [DatabaseConnection] 领域层
│   │   │   ├── 📄 DatabaseConnection.cs              # 领域实体
│   │   │   ├── 📄 DatabaseConnectionManager.cs       # [领域服务]:处理业务逻辑，供 ApplicationService 调用
│   │   │   └── 📄 IDatabaseConnectionRepository.cs   # 领域仓储接口:供 ApplicationService 调用
│   │   ├── 📂 TableFields                            # [TableFields] 领域层
│   │   ├── 📂 TableStructures                        # [TableStructures] 领域层
│   │   └── 📄 DomainExtensions.cs                    # 此层扩展方法：用于注册 [领域服务]
│   │
│   ├── 📦 Domain.Shared                              # 领域共享层
│   │   └── 📂 Enums                                  # 全局枚举定义
│   │
│   ├── ⚙️ Application                                # 应用层(应用层集合)
│   │   ├── 📂 DatabaseConnections                    # [DatabaseConnection] 应用层
│   │   │   └── 📄 DatabaseConnectionService.cs       # [DatabaseConnection] 的 ApplicationService:编排 IRepository 与 Manager
│   │   ├── 📂 TableFields                            # [TableFields] 应用层
│   │   └── 📄 ApplicationExtensions.cs               # 此层扩展方法：用于注册 [ApplicationService]
│   │
│   ├── 📜 Application.Contracts                      # 应用契约层(应用契约层集合)
│   │   └──  📂 DatabaseConnections                   # [DatabaseConnection] 应用契约层
│   │        ├── 📂 Dtos                              # DTOs：定义输入输出的数据传输结构
│   │        └── 📄 IDatabaseConnectionService.cs     # IApplicationService 接口:供 API层调用
│   │
│   ├── 💾 EFCore                                     # 基础设施层 (数据持久化)
│   │   ├── 📂 Mappings                               # EFCore实体配置
│   │   │   └── 📄 DatabaseConnectionConfiguration.cs # [DatabaseConnection] 的EFCore实体配置
│   │   ├── 📂 Repositorys                            # 领域仓储实现
│   │   │   └── 📄 DatabaseConnectionRepository.cs    # [DatabaseConnection] 的仓储实现
│   │   ├── 📄 SqlBotxDBContext.cs                    # SqlBotx数据库上下文
│   │   ├── 📄 ApplicationDbContextBase.cs            # DbContext基类
│   │   └── 📄 EFCoreExtensions.cs                    # 扩展方法：注册DBContext、注册仓储
│   │
│   ├── 🌐 Api (ApiService)                           # 表现层 (入口) .NET Web API
│   │   ├── 📄 appsettings.json                       # 配置文件
│   │   └── 📄 Program.cs                             # 程序启动点：配置中间件、跨域、ProblemDetails 等
│   │
│   ├── 🏗️ Core                                       # 框架核心层 (跨层通用)
│   │   ├── 📂 Controller                             # BaseController 或通用响应结构 (ApiResponse.cs)
│   │   └── 📂 Startups                               # 启动配置封装 (CorsExtensions.cs)
│   │
│   ├── 🌐 DbManager                                  # 数据库管理工具 .NET Web API
│   │   └── 📂 Migrations                             # 数据库迁移文件
│   │
│   └── 🤝 Share                                      # 共享工具类库
```

#### SqlBoTx项目物理文件结构
```text
# 📂 src/SqlBoTx.Net
    ├── 📂 SqlBoTx.Net.AppHost                  # 应用主机（Aspire）
    ├── 📂 SqlBoTx.Net.ServiceDefaults          # Aspire 服务默认配置
    ├── 📂 SqlBoTx.Net.DbManager                # .NET Web API 数据库管理 (用于EF Core 迁移/种子数据)
    ├── 📂 SqlBoTx.Net.Domain                   # 领域层
    ├── 📂 SqlBoTx.Net.Domain.Share             # 领域层共享库
    ├── 📂 SqlBoTx.Net.Application.Contracts    # 应用契约 (接口定义)
    ├── 📂 SqlBoTx.Net.Application              # 应用服务实现 (业务编排)
    ├── 📂 SqlBoTx.Net.EFCore                   # EF Core 实现
    ├── 📂 SqlBoTx.Net.ApiService               # Web API 入口
    ├── 📂 SqlBoTx.Net.Core                     # 核心通用组件 (跨层)
    └── 📂 SqlBoTx.Net.Share                    # 通用工具类库 (Utils/Helpers)
```

#### 分层详细说明
 Read [LayeredExplanation.MD](LayeredExplanation.MD)