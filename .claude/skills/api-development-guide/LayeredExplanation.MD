# Domain
### 项目
`SqlBoTx.Net.Domain`

### 结构:
```text
Domain                                
├── 📂 DatabaseConnections                      # [DatabaseConnection] 领域层
│   ├── 📄 DatabaseConnection.cs                # 领域实体
│   ├── 📄 DatabaseConnectionManager.cs         # 领域服务:处理业务逻辑，供 ApplicationService 调用
│   └── 📄 IDatabaseConnectionRepository.cs     # 领域仓储接口:供 ApplicationService 调用
├── 📂 TableFields                              # [TableFields] 领域层
├── 📂 ...                                      # 其他领域层
└── 📄 DomainExtensions.cs                      # 扩展方法：注册服务
```
### 职责
    领域实体（Entities）- 包含业务状态和行为
    领域服务（Domain Services）- 封装领域业务规则
    领域事件（Domain Events）- 领域状态变化通知
    仓储接口（Repository Interfaces）- 数据访问抽象
    值对象（Value Objects）- 不可变的业务概念
    领域规则与业务逻辑

### 依赖关系
```
Domain → Domain.Share (领域共享层)
Domain → Share (共享层)
```

### 类命名约定
类型 | 命名模式 | 示例 |
|---------|------|------|
**实体** | PascalCase | `DatabaseConnection`, `TableStructure` 
**领域服务** | `[实体名]Manager` | `DatabaseConnectionManager` 
**仓储接口** | `I[实体名]Repository` | `IDatabaseConnectionRepository` 

### 实体类约定 
- 字段仅能包括 xml注释 和 [Description] 特性
- 示例代码：
  ```csharp
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    [Description("数据库连接配置")]
    public class DatabaseConnection
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 连接ID（主键）
        /// </summary>
        [Description("连接ID（主键）")]
        public int ConnectionId { get; set; }

        /// <summary>
        /// 连接名称
        /// </summary>
        [Description("连接名称")]
        public string? ConnectionName { get; set; }

        /// <summary>
        /// 连接类型（SQL Server、MySQL、Oracle、PostgreSQL等）
        /// </summary>
         [Description("连接类型（SQL Server、MySQL、Oracle、PostgreSQL等）")]
        public ConnectionType? ConnectionType { get; set; }

        /// <summary>
        /// 导航属性
        /// </summary>
        public virtual ICollection<TableStructure> TableStructures { get; set; } = new List<TableStructure>();
    }
  ```

### 领域服务约定
- 示例代码：
  ```csharp
   /// <summary>
  /// Manager 用来执行 DatabaseConnection 相关的业务逻辑
  /// </summary>
  public class DatabaseConnectionManager
  {
      /// <summary>
      /// Encrypt the database user password
      /// </summary>
      /// <param name="password"></param>
      /// <returns></returns>
      private string EncryptPassword(string password){...}

      /// <summary>
      /// Create a new database connection
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      public async Task<DatabaseConnection> CreateAsync(DatabaseConnection input){...}

      /// <summary>
      /// Update an existing database connection
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      public async Task<DatabaseConnection> UpdateAsync(DatabaseConnection input){...}
  }
  ```



# Domain.Share
### 项目
`SqlBoTx.Net.Domain.Share`

### 结构:
```text
Domain.Share                                
└── 📂 Enums                    # 全局枚举定义
```

### 职责
    枚举定义（业务枚举）
    领域通用类型
    领域公共常量
    领域共享的值对象

# EFCore
### 项目
`SqlBoTx.Net.EFCore`

### 结构
  ```text
    EFCore                                       
    ├── 📂 Mappings                               # EFCore实体配置
    │   └── 📄 DatabaseConnectionConfiguration.cs     # [DatabaseConnection] 的 EFCore实体配置
    ├── 📂 Repositorys                            # 领域仓储实现
    │   └── 📄 DatabaseConnectionRepository.cs    # [DatabaseConnection] 的仓储实现
    ├── 📄 SqlBotxDBContext.cs                    # SqlBotx数据库上下文
    ├── 📄 ApplicationDbContextBase.cs            # DbContext基类
    └── 📄 EFCoreExtensions.cs                    # 扩展方法：注册DBContext、注册仓储
  ```
### 职责
    EF Core 数据库上下文（DbContext）
    仓储接口的具体实现
    EF Core 配置与映射（EntityConfiguration）
    数据访问层实现

### 依赖关系:
    EFCore → Domain (领域层)
    EFCore → Share (共享层)

### 类命名约定
类型 | 命名模式 | 示例 
---------|------|------
**仓储实现** | `[实体名]Repository` | `DatabaseConnectionRepository` 
**实体配置** | `[实体名]Configuration` | `DatabaseConnectionConfiguration` 

### 仓储实现类约定
- 示例代码：
  ```csharp
  /// <summary>
  /// database connection repository
  /// </summary>
  public class DatabaseConnectionRepository : IDatabaseConnectionRepository
  {
      private readonly SqlBotxDBContext _dbContext;

      public DatabaseConnectionRepository(SqlBotxDBContext dbContext)
      {
          _dbContext = dbContext;
      }

      /// <summary>
      /// Get IQueryable
      /// </summary>
      /// <returns><see cref="IQueryable<DatabaseConnection>"/></returns>
      public IQueryable<DatabaseConnection> IQueryable()
      {
          return _dbContext.Set<DatabaseConnection>();
      }

      /// <summary>
      /// Get List of DatabaseConnection
      /// </summary>
      /// <param name="includeFunc"></param>
      /// <returns></returns>
      public async Task<List<DatabaseConnection>> ListAsync(Func<IQueryable<DatabaseConnection>, IQueryable<DatabaseConnection>>? includeFunc = null)
      {
          var query = _dbContext.Set<DatabaseConnection>().AsQueryable();
          if (includeFunc != null)
          {
              query = includeFunc(query);
          }
          return await query.ToListAsync();
      }

      /// <summary>
      /// Inster
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      public async Task<int> InsterAsync(DatabaseConnection entity)
      {
          _dbContext.Set<DatabaseConnection>().Add(entity);
          return await _dbContext.SaveChangesAsync();
      }

      /// <summary>
      /// Update
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      public async Task<int> UpdateAsync(DatabaseConnection entity)
      {
          return
          await _dbContext.Set<DatabaseConnection>()
            .Where(x => x.ConnectionId == entity.ConnectionId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.ConnectionName, entity.ConnectionName)
                .SetProperty(x => x.ConnectionType, entity.ConnectionType)
                .SetProperty(x => x.ConnectionString, entity.ConnectionString)
                .SetProperty(x => x.UserName, entity.UserName)
                .SetProperty(x => x.UserPassword, entity.UserPassword)
                .SetProperty(x => x.Description, entity.Description)
                .SetProperty(x => x.LastModifiedDate, DateTime.Now)
            );
      }
  }
  ```

### 实体配置约定
1. 无需设置HasComment
2. 示例代码:
    ```csharp
    public class DatabaseConnectionConfiguration : IEntityTypeConfiguration<DatabaseConnection>
    {
        public void Configure(EntityTypeBuilder<DatabaseConnection> builder)
        {
            //主键
            builder.HasKey(x => x.ConnectionId);
            builder.Property(x => x.ConnectionId).ValueGeneratedOnAdd();

            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.LastModifiedDate).IsRequired(false);
            builder.Property(x => x.ConnectionName).IsRequired(true).HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.ConnectionType).IsRequired(true);
            builder.Property(x => x.ConnectionString).IsRequired(true).HasColumnType("NVARCHAR(MAX)");
            builder.Property(x => x.UserName).IsRequired(false).HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.UserPassword).IsRequired(false).HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.Description).IsRequired(false).HasColumnType("NVARCHAR(255)");

            //设置外键关系
            builder.HasMany(x => x.TableStructures)
                .WithOne()
                .HasForeignKey(x => x.ConnectionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    ```


# DbManager
### 项目
`SqlBoTx.Net.DbManager`

### 结构
  ```text
    DbManager                                       
    ├── 📂 Migrations                               # EFCore 迁移文件
    ├── 📄 01_AddMigrations.bat                     # 用来执行 dotnet ef  migrations  add
    ├── 📄 02_DelMigrations.bat                     # 用来执行 dotnet ef  migrations  remove  --json
    └── 📄 DbInitializer.cs                         # 执行迁移文件
  ```
### 职责
    管理EF Core的迁移文件
    执行迁移文件、数据库初始化

### 依赖关系:
    DbManager → EFCore


# Application.Contracts
### 项目
 `SqlBoTx.Net.Application.Contracts`

### 结构:
```text
Application.Contracts                                
└── 📂 DatabaseConnections                    # A业务应用层
    ├── 📂 Dtos                               # A业务Dto集合
    └── 📄 IDatabaseConnectionService.cs      # A业务应用IApplicationServices
```

### 职责
    定义应用服务接口（契约）
    定义 DTO（数据传输对象）

### 依赖关系
    Application.Contracts → Domain.Share

### 类命名约定
类型 | 命名模式 | 示例 
---------|------|------
**DTO** |`[动作][实体名]Dto` | `AddDatabaseConnectionDto`, `UpdateDatabaseConnectionDto` 
**应用服务接口** | `I[功能名]Service` | `IDatabaseConnectionService` |


# Application
### 项目
`SqlBoTx.Net.Application`

### 结构
```text
Application                                
├── 📂 DatabaseConnections                    # A业务应用层
│   └── 📄 DatabaseConnectionService.cs       # A业务应用ApplicationServices：编排业务领域服务和业务仓储
├── 📂 TableFields                            # B业务应用层
└── 📄 ApplicationExtensions.cs               # 扩展方法：注册服务
```

### 职责:
    应用业务逻辑编排
    领域服务的协调与调用
    事务管理
    DTO 与领域实体的转换（使用 Mapster）
    应用级业务规则
    外部服务集成（如需要）

### 依赖关系
    Application → Application.Contracts
    Application → Domain
 
### 类命名约定
类型 | 命名模式 | 示例 
---------|------|------
**应用服务** | `[功能名]Service` | `DatabaseConnectionService` 

# Api (ApiService) 
### 项目
`SqlBoTx.Net.ApiService`

### 结构
```text
Api (ApiService)             # 表现层 (入口) .NET Web API
├── 📂 Controllers          # API 控制器
├── 📄 appsettings.json     # 配置文件
└── 📄 Program.cs           # 程序启动点：配置中间件、跨域、ProblemDetails 等
```

### 职责:
    HTTP API 入口点
    请求参数绑定与验证
    响应格式化与返回
    跨域配置 (CORS)
    Swagger/OpenAPI 文档生成
    全局异常处理
    认证与授权（如果需要）

### 依赖关系
    ApiService → Core
    ApiService → Application
    ApiService → EFCore
    ApiService → Domain.Share 
    ApiService → Share 
    ApiService → ServiceDefaults 
 
### 类命名约定
类型 | 命名模式 | 示例 
---------|------|------
**Controllers** | `[功能名]Controller` | `DatabaseSettingController` 

### Controller约定
1. Tags、Consumes、Route
2. 方法必须标记HttpGet/HttpPost/HttpDelete
3. 方法必须标记ProducesResponseType 将返回可能状态码以及返回类型
4. 示例代码
    ```csharp
    /// <summary>
    /// 数据库连接配置管理
    /// </summary>
    [Route("[controller]")]
    [Consumes("application/json")]
    [Tags("数据库连接配置管理")]
    public class DatabaseSettingController : LarApi
    {
        private readonly IDatabaseConnectionService _databaseConnectionService;

        public DatabaseSettingController(IDatabaseConnectionService databaseConnectionService)
        {
            _databaseConnectionService = databaseConnectionService;
        }

        /// <summary>
        /// 列表（不包含表结构）
        /// </summary>
        /// <returns>返回数组（不包含表结构）</returns>
        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(List<ListDatabaseConnectionDto>))]
        public async Task<IActionResult> List()
        {
            var list = await _databaseConnectionService.ListAsync();
            return Ok(list);
        }

        /// <summary>
        /// 列表（包含表结构）
        /// </summary>
        /// <returns>返回数组（包含表结构）</returns>
        [HttpGet("list-with-tables")]
        [ProducesResponseType(200, Type = typeof(List<ListDatabaseConnectionDto>))]
        public async Task<IActionResult> ListWithTables()
        {
            var list = await _databaseConnectionService.ListWithTablesAsync();
            return Ok(list);
        }

        /// <summary>
        /// 新增数据库连接
        /// </summary>
        /// <returns>返回受影响的行数</returns>
        [HttpPost("add")]
        [HttpDelete]
        [ProducesResponseType(200, Type = typeof(Int32))]
        public async Task<IActionResult> Add([FromBody] AddDatabaseConnectionDto input)
        {
            var result = await _databaseConnectionService.AddAsync(input);
            return Ok(result);
        }

        /// <summary>
        /// 更新数据库连接
        /// </summary>
        /// <returns>返回受影响的行数</returns>
        [HttpPost("update")]
        [ProducesResponseType(200, Type = typeof(Int32))]
        public async Task<IActionResult> Update([FromBody] UpdateDatabaseConnectionDto input)
        {
            var result = await _databaseConnectionService.UpdateAsync(input);
            return Ok(result);
        }
    }
    ```

# Core
### 项目
`SqlBoTx.Net.Core`

### 职责:
    封装的供API层调用的核心方法

# Share
### 项目
`SqlBoTx.Net.Share`

### 职责:
    共享工具类库

# AppHost
### 项目
`SqlBoTx.Net.AppHost`

### 职责:
    Aspire 编排层：负责数据库容器、API服务分发及环境变量配置

# ServiceDefaults
### 项目
`SqlBoTx.Net.ServiceDefaults`

### 职责:
    注册启动项目服务默认信息 # 基础设施默认配置：包含 OpenTelemetry、健康检查、弹性重试等