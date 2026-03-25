using ImTools;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives.Embeddings;
using SqlBoTx.Net.Application.Vectors;
using SqlBoTx.Net.Domain.BusinessEntities;
using SqlBoTx.Net.Domain.BusinessObjectives;
using SqlBoTx.Net.Domain.DatabaseConnections;
using SqlBoTx.Net.Domain.DomainEntities;
using SqlBoTx.Net.Domain.Share.Enums;
using SqlBoTx.Net.Domain.Share.Enums.BusinessObjective;
using SqlBoTx.Net.Domain.Share.Enums.DomainEntity;
using SqlBoTx.Net.Domain.Share.TableRelationships;
using SqlBoTx.Net.Domain.TableRelationships;
using SqlBoTx.Net.Domain.TableStructures;
using SqlBoTx.Net.EFCore;
using System.Runtime.Intrinsics.Arm;

namespace SqlBoTx.Net.DbManager.SeedDatas
{
    public class LocalDb_CRM
    {
        private readonly SqlBotxDBContext _db;
        private readonly QdrantVectorService _vectorService;

        private const int ConnectionId = 1;

        // ── 实体 ID 常量（避免魔法字符串，同时防止重复）─────────────────────
        private static class EntityIds
        {
            // 组织架构域
            public const string Organization = "CRM-ENT-ORG-001";
            public const string Department = "CRM-ENT-DEP-001";
            public const string UserInfo = "CRM-ENT-USR-001";

            // 产品域
            public const string Product = "CRM-ENT-PRD-001";

            // 销售域
            public const string Customer = "CRM-ENT-CUS-001";
            public const string Opportunity = "CRM-ENT-OPP-001";
            public const string Quotation = "CRM-ENT-QUO-001";
            public const string QuotationDetail = "CRM-ENT-QUO-002";
            public const string ProjectApproval = "CRM-ENT-PAP-001";   
            public const string Contract = "CRM-ENT-CON-001";
            public const string ContractDetail = "CRM-ENT-CON-002";
            public const string ContractChange = "CRM-ENT-CON-003";
            public const string ContractHandover = "CRM-ENT-CON-004";

            // 投标域
            public const string Bid = "CRM-ENT-BID-001";
        }

        public LocalDb_CRM(SqlBotxDBContext db, QdrantVectorService vectorService)
        {
            _db = db;
            _vectorService = vectorService;
        }

        // ─────────────────────────────────────────────────────────────────────
        // 入口
        // ─────────────────────────────────────────────────────────────────────

        public async Task RunAsync()
        {
            await InitAsync();
        }

        private string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        public async Task InitAsync()
        {
            // 幂等守卫：仅首次运行
            if (await _db.Set<TableStructure>().AnyAsync()) return;

            const string dbName = "CRM1";
            var connectionString = $"Server=.;Database={dbName};Trusted_Connection=True;TrustServerCertificate=True;";

            // ── 1. 创建业务库 + 执行 SQL 脚本 ──────────────────────────────
            await CreateDatabaseAsync(dbName);

            string assetsPath = GetAbsolutePath("SeedDatas");
            await ExecuteSqlFileAsync(connectionString, Path.Combine(assetsPath, "业务库_CRM系统_表结构.sql"));
            await ExecuteSqlFileAsync(connectionString, Path.Combine(assetsPath, "业务库_CRM系统_数据.sql"));

            // ── 2. 注册数据库连接 ─────────────────────────────────────────
            var dbConn = new DatabaseConnection
            {
                ConnectionName = dbName,
                ConnectionType = ConnectionType.SQLServer,
                ConnectionString = connectionString,
                UserName = "sa",
                UserPassword = "123456",
            };
            await _db.AddAsync(dbConn);
            await _db.SaveChangesAsync();

            // ── 3. 同步表结构 ─────────────────────────────────────────────
            var tables = await SyncTableStructuresAsync(connectionString, dbConn.Id);
            await _db.AddRangeAsync(tables);
            await _db.SaveChangesAsync();

            // SaveChanges 后才有正确的自增 ID，重新从数据库读取确保 ID 准确
            tables = await _db.Set<TableStructure>()
                .Include(t => t.Columns)
                .Where(t => t.ConnectionId == dbConn.Id)
                .ToListAsync();

            var tableMap = tables.ToDictionary(t => t.TableName);

            // ── 4. 表级物理关系 ───────────────────────────────────────────
            var tableRelationships = BuildTableRelationships(tableMap);
            await _db.AddRangeAsync(tableRelationships);
            await _db.SaveChangesAsync();

            // ── 5. 业务域 ─────────────────────────────────────────────────
            var domainRoots = BuildDomains();
            await _db.AddRangeAsync(domainRoots);
            await _db.SaveChangesAsync();

            // 按名称建立域快查表
            var allDomains = await _db.Set<BusinessObjective>().ToListAsync();
            var domainMap = allDomains.ToDictionary(d => d.BusinessName);

            // ── 6. 实体 ───────────────────────────────────────────────────
            var entities = BuildEntities(domainMap, tableMap);
            await _db.AddRangeAsync(entities);
            await _db.SaveChangesAsync();

            var entityMap = entities.ToDictionary(e => e.Id);

            // ── 7. 实体属性（含语义标注）──────────────────────────────────
            var attrs = BuildEntityAttrs(entities, tableMap, entityMap);
            await _db.AddRangeAsync(attrs);
            await _db.SaveChangesAsync();

            // ── 8. 实体间关系（本体论层）─────────────────────────────────
            var entityRels = BuildEntityRelations(entityMap);
            await _db.AddRangeAsync(entityRels);
            await _db.SaveChangesAsync();

            // ── 9. 向量入库 ────────────────────────────────────────────────
            await UpsertFieldEmbeddingsAsync(entities, allDomains);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 1-2. 数据库初始化
        // ─────────────────────────────────────────────────────────────────────

        private static async Task CreateDatabaseAsync(string dbName)
        {
            const string master = "Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
            await using var conn = new SqlConnection(master);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"IF DB_ID(N'{dbName}') IS NULL CREATE DATABASE [{dbName}]";
            await cmd.ExecuteNonQueryAsync();
        }

        private static async Task ExecuteSqlFileAsync(string connectionString, string sqlFilePath)
        {
            var sql = await File.ReadAllTextAsync(sqlFilePath);
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // SQL Server GO 批次拆分
            var batches = sql.Split(new[] { "\nGO", "\r\nGO" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var batch in batches)
            {
                var trimmed = batch.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                await using var cmd = conn.CreateCommand();
                cmd.CommandTimeout = 300;
                cmd.CommandText = trimmed;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // 3. 表结构同步（从 SQL Server 系统表读取）
        // ─────────────────────────────────────────────────────────────────────

        private async Task<List<TableStructure>> SyncTableStructuresAsync(
            string connectionString, int connId)
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // 表
            var tables = new List<TableStructure>();
            const string tableSql = @"
                SELECT t.name AS TableName, s.name AS SchemaName, ep.value AS TableComment
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                LEFT JOIN sys.extended_properties ep
                    ON ep.major_id = t.object_id AND ep.minor_id = 0 AND ep.name = 'MS_Description'
                ORDER BY s.name, t.name";

            await using (var cmd = new SqlCommand(tableSql, conn))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    tables.Add(new TableStructure
                    {
                        ConnectionId = connId,
                        TableName = reader.GetString(0),
                        SchemaName = reader.GetString(1),
                        Description = reader["TableComment"] as string,
                        Alias = reader.GetString(0),
                    });
                }
            }

            // 列（完整元数据）
            const string columnSql = @"
                SELECT
                    t.name AS TableName, c.name AS ColumnName,
                    ty.name AS DataType,
                    CASE
                        WHEN ty.name IN ('varchar','nvarchar','char','nchar')
                            THEN ty.name + '(' + CASE WHEN c.max_length = -1 THEN 'MAX'
                                 ELSE CAST(c.max_length / CASE WHEN ty.name LIKE 'n%' THEN 2 ELSE 1 END AS VARCHAR) END + ')'
                        WHEN ty.name IN ('decimal','numeric')
                            THEN ty.name + '(' + CAST(c.precision AS VARCHAR) + ',' + CAST(c.scale AS VARCHAR) + ')'
                        ELSE ty.name
                    END AS DataTypeSchema,
                    dc.definition AS DefaultValue,
                    ep.value AS Description,
                    CASE WHEN pk.column_id IS NOT NULL THEN 1 ELSE 0 END AS IsPrimaryKey,
                    c.is_identity AS IsIdentity,
                    CASE WHEN c.is_nullable = 0 THEN 1 ELSE 0 END AS IsRequired,
                    CASE WHEN uq.column_id IS NOT NULL THEN 1 ELSE 0 END AS IsUnique,
                    CASE WHEN fk.parent_column_id IS NOT NULL THEN 1 ELSE 0 END AS IsReference,
                    rt.name AS ReferenceTableName,
                    rc.name AS ReferenceColumn,
                    c.is_computed AS IsComputed,
                    cc.definition AS Expression,
                    CASE WHEN i.index_id IS NOT NULL THEN 1 ELSE 0 END AS IsIndex,
                    STUFF((SELECT ',' + i2.name FROM sys.index_columns ic2
                           JOIN sys.indexes i2 ON ic2.object_id = i2.object_id AND ic2.index_id = i2.index_id
                           WHERE ic2.object_id = c.object_id AND ic2.column_id = c.column_id
                           FOR XML PATH('')), 1, 1, '') AS Indexs
                FROM sys.columns c
                JOIN sys.tables t ON c.object_id = t.object_id
                JOIN sys.types ty ON c.user_type_id = ty.user_type_id
                LEFT JOIN sys.default_constraints dc ON c.default_object_id = dc.object_id
                LEFT JOIN sys.extended_properties ep
                    ON ep.major_id = c.object_id AND ep.minor_id = c.column_id AND ep.name = 'MS_Description'
                LEFT JOIN (SELECT ic.object_id, ic.column_id FROM sys.indexes i
                           JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                           WHERE i.is_primary_key = 1) pk ON pk.object_id = c.object_id AND pk.column_id = c.column_id
                LEFT JOIN (SELECT ic.object_id, ic.column_id FROM sys.indexes i
                           JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                           WHERE i.is_unique = 1) uq ON uq.object_id = c.object_id AND uq.column_id = c.column_id
                LEFT JOIN sys.foreign_key_columns fk
                    ON fk.parent_object_id = c.object_id AND fk.parent_column_id = c.column_id
                LEFT JOIN sys.tables rt ON fk.referenced_object_id = rt.object_id
                LEFT JOIN sys.columns rc
                    ON fk.referenced_object_id = rc.object_id AND fk.referenced_column_id = rc.column_id
                LEFT JOIN sys.computed_columns cc ON cc.object_id = c.object_id AND cc.column_id = c.column_id
                LEFT JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                LEFT JOIN sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                ORDER BY t.name, c.column_id";

            var tableIndex = tables.ToDictionary(t => t.TableName);

            await using (var cmd = new SqlCommand(columnSql, conn))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var tableName = reader.GetString(0);
                    if (!tableIndex.TryGetValue(tableName, out var table)) continue;

                    table.Columns.Add(new TableStructureColumn
                    {
                        ColumnName = reader.GetString(1),
                        DataType = reader["DataType"] as string,
                        DataTypeSchema = reader["DataTypeSchema"] as string,
                        DefaultValue = CleanDefaultValue(reader["DefaultValue"]),
                        Label = reader["Description"] as string,
                        Description = reader["Description"] as string,
                        IsPrimaryKey = Convert.ToBoolean(reader["IsPrimaryKey"]),
                        IsIdentity = Convert.ToBoolean(reader["IsIdentity"]),
                        IsRequired = Convert.ToBoolean(reader["IsRequired"]),
                        IsUnique = Convert.ToBoolean(reader["IsUnique"]),
                        IsReference = Convert.ToBoolean(reader["IsReference"]),
                        ReferenceTableName = reader["ReferenceTableName"] as string,
                        ReferenceColumn = reader["ReferenceColumn"] as string,
                        IsComputed = Convert.ToBoolean(reader["IsComputed"]),
                        Expression = reader["Expression"] as string,
                        IsIndex = Convert.ToBoolean(reader["IsIndex"]),
                        Indexs = reader["Indexs"] is DBNull
                            ? null
                            : reader["Indexs"].ToString()!.Split(','),
                    });
                }
            }

            return tables;
        }

        // ─────────────────────────────────────────────────────────────────────
        // 4. 表级物理关系（TableRelationship）
        // ─────────────────────────────────────────────────────────────────────
        private static List<TableRelationship> BuildTableRelationships(
            Dictionary<string, TableStructure> t)
        {
            var rels = new List<TableRelationship>();

            // 快捷方法：减少重复代码
            void Add(string src, string tgt,
                     TableRelationshipType srcCard, TableRelationshipType tgtCard,
                     params TableRelationshipCondition[] conditions)
            {
                if (!t.ContainsKey(src) || !t.ContainsKey(tgt)) return;
                rels.Add(new TableRelationship
                {
                    SourceTableId = t[src].TableId,
                    TargetTableId = t[tgt].TableId,
                    SourceCardinality = srcCard,
                    TargetCardinality = tgtCard,
                    Conditions = conditions.ToList(),
                });
            }

            var N = TableRelationshipType.Many;
            var One = TableRelationshipType.One;

            // ── 组织架构 ──────────────────────────────────────────────────
            Add("Department", "Organization", N, One, TableRelationshipCondition.Key("OrganizationId", "OrganizationId"));
            Add("UserInfo", "Department", N, One, TableRelationshipCondition.Key("DepartmentId", "DepartmentId"));
            Add("UserInfo", "Organization", N, One, TableRelationshipCondition.Key("OrganizationId", "OrganizationId"));

            // ── 客户 ──────────────────────────────────────────────────────
            Add("Customer", "UserInfo", N, One, TableRelationshipCondition.Key("OwnerUserId", "UserId"));
            Add("Customer", "Organization", N, One, TableRelationshipCondition.Key("CreateOrganizationId", "OrganizationId"));

            // ── 商机 ──────────────────────────────────────────────────────
            Add("Opportunity", "Customer", N, One, TableRelationshipCondition.Key("CustomerId", "CustomerId"));

            // ── 报价 ──────────────────────────────────────────────────────
            Add("Quotation", "Opportunity", N, One, TableRelationshipCondition.Key("OpportunityId", "OpportunityId"));
            Add("QuotationDetail", "Quotation", N, One, TableRelationshipCondition.Key("QuotationId", "QuotationId"));
            Add("QuotationDetail", "Product", N, One, TableRelationshipCondition.Key("ProductId", "ProductId"));

            // ── 投标 ──────────────────────────────────────────────────────
            Add("Bid", "Opportunity", N, One, TableRelationshipCondition.Key("OpportunityId", "OpportunityId"));

            // ── 立项（多态：ReferenceType=1→Quotation, 2→Bid）─────────────
            Add("ProjectApproval", "Quotation", One, One,
                TableRelationshipCondition.Key("ReferenceId", "ReferenceId"),
                TableRelationshipCondition.SourceConstant("ReferenceType", "1"));
            Add("ProjectApproval", "Bid", One, One,
                TableRelationshipCondition.Key("ReferenceId", "ReferenceId"),
                TableRelationshipCondition.SourceConstant("ReferenceType", "2"));
            Add("ProjectApproval", "Customer", N, One, TableRelationshipCondition.Key("CustomerId", "CustomerId"));

            // ── 合同 ──────────────────────────────────────────────────────
            Add("Contract", "ProjectApproval", One, One, TableRelationshipCondition.Key("ProjectApprovalId", "ProjectApprovalId"));
            Add("ContractDetail", "Contract", N, One, TableRelationshipCondition.Key("ContractId", "ContractId"));
            Add("ContractDetail", "Product", N, One, TableRelationshipCondition.Key("ProductId", "ProductId"));
            Add("ContractChange", "Contract", One, One, TableRelationshipCondition.Key("ContractId", "ContractId"));
            Add("ContractHandover", "Contract", One, One, TableRelationshipCondition.Key("ContractId", "ContractId"));

            return rels;
        }

        // ─────────────────────────────────────────────────────────────────────
        // 5. 业务域
        // ─────────────────────────────────────────────────────────────────────

        private static List<BusinessObjective> BuildDomains()
        {
            var org = new BusinessObjective
            {
                BusinessName = "组织架构",
                Description = "企业组织架构信息：组织/部门/用户",
                Tags = ["支撑域"],
            };
            var product = new BusinessObjective
            {
                BusinessName = "产品管理",
                Description = "管理公司销售产品的基础信息、类型和定价",
                Tags = ["核心域"],
            };
            var sales = new BusinessObjective
            {
                BusinessName = "销售业务域",
                Description = "销售业务域：客户管理、商机登记、销售报价、销售合同、销售立项",
                Tags = ["核心域"],
            };
            var bid = new BusinessObjective
            {
                BusinessName = "投标业务域",
                Description = "管理招标项目与投标信息，跟踪投标状态与结果",
                Tags = ["核心域"],
            };

            return
            [
                new BusinessObjective
                {
                    BusinessName = "CRM系统",
                    Synonyms     = "CRM,客户关系管理,销售管理系统",
                    Description  = "领创CRM+工程项目管理系统",
                    KeyWords     = "CRM",
                    Tags         = ["系统域"],
                    Children     = [org, product, sales, bid],
                }
            ];
        }

        // ─────────────────────────────────────────────────────────────────────
        // 6. 实体
        // ─────────────────────────────────────────────────────────────────────

        private static List<DomainEntity> BuildEntities(
            Dictionary<string, BusinessObjective> domainMap,
            Dictionary<string, TableStructure> tableMap)
        {
            var entities = new List<DomainEntity>();

            // 快捷工厂
            DomainEntity Make(string id, string domainName, string tableName,
                              string name, string[] tags)
            {
                var table = tableMap[tableName];
                var domain = domainMap[domainName];
                return new DomainEntity
                {
                    Id = id,
                    DomainId = domain.Id,
                    ReferenceConnectId = table.ConnectionId,
                    ReferenceTableId = table.TableId,
                    Name = name,
                    Alias = tableName,
                    Description = table.Description,
                    Tags = tags,
                };
            }

            // ── 组织架构 ──────────────────────────────────────────────────
            entities.Add(Make(EntityIds.Organization, "组织架构", "Organization", "公司组织结构", ["核心实体"]));
            entities.Add(Make(EntityIds.Department, "组织架构", "Department", "部门管理", ["核心实体"]));
            entities.Add(Make(EntityIds.UserInfo, "组织架构", "UserInfo", "用户管理", ["核心实体"]));

            // ── 产品 ──────────────────────────────────────────────────────
            entities.Add(Make(EntityIds.Product, "产品管理", "Product", "产品管理", ["核心实体"]));

            // ── 销售 ──────────────────────────────────────────────────────
            entities.Add(Make(EntityIds.Customer, "销售业务域", "Customer", "客户管理", ["核心实体"]));
            entities.Add(Make(EntityIds.Opportunity, "销售业务域", "Opportunity", "商机管理", ["核心实体"]));
            entities.Add(Make(EntityIds.Quotation, "销售业务域", "Quotation", "商机报价单", ["核心实体"]));
            entities.Add(Make(EntityIds.QuotationDetail, "销售业务域", "QuotationDetail", "商机报价明细", ["明细实体"]));
            entities.Add(Make(EntityIds.ProjectApproval, "销售业务域", "ProjectApproval", "销售立项", ["核心实体"]));
            entities.Add(Make(EntityIds.Contract, "销售业务域", "Contract", "销售合同", ["核心实体"]));
            entities.Add(Make(EntityIds.ContractDetail, "销售业务域", "ContractDetail", "销售合同明细", ["明细实体"]));
            entities.Add(Make(EntityIds.ContractChange, "销售业务域", "ContractChange", "销售合同变更", ["副作用实体"]));
            entities.Add(Make(EntityIds.ContractHandover, "销售业务域", "ContractHandover", "销售合同交底", ["副作用实体"]));

            // ── 投标 ──────────────────────────────────────────────────────
            entities.Add(Make(EntityIds.Bid, "投标业务域", "Bid", "投标管理", ["核心实体"]));

            return entities;
        }

        // ─────────────────────────────────────────────────────────────────────
        // 7. 实体属性语义标注
        // ─────────────────────────────────────────────────────────────────────

        private static List<DomainEntityAttr> BuildEntityAttrs(
            List<DomainEntity> entities,
            Dictionary<string, TableStructure> tableMap,
            Dictionary<string, DomainEntity> entityMap)
        {
            var result = new List<DomainEntityAttr>();

            foreach (var entity in entities)
            {
                var table = tableMap[entity.Alias!];
                var attrs = table.Columns.Select(col =>
                {
                    var attr = new DomainEntityAttr
                    {
                        EntityId = entity.Id,
                        ColumnId = col.FieldId,
                        ColumnName = col.ColumnName,
                        Label = col.Label,
                        IsRequired = col.IsRequired,
                        DataType = DataTypeConverter.Convert(col.DataType!),
                        DataTypeSchema = col.DataTypeSchema,
                        DefaultValue = col.DefaultValue,
                        Description = col.Description,
                        StructureRole = col.IsPrimaryKey ? StructureRole.PrimaryKey : StructureRole.None,
                        // 外键：优先由下方 ApplySemantics 覆盖精确设置；此处仅做兜底
                        ForeignKeyMetaData = col.IsReference
                            ? new ForeignKeyMetaData
                            {
                                Type = ForeignKeyType.Normal,
                                TargetEntityId = entityMap.Values
                                    .FirstOrDefault(e => e.Alias == col.ReferenceTableName)?.Id
                            }
                            : new ForeignKeyMetaData(),
                        SemanticType = SemanticType.Attribute,
                    };

                    ApplySemantics(attr, entity.Alias!, entityMap);
                    return attr;
                }).ToList();

                entity.Attrs = attrs;
                result.AddRange(attrs);
            }

            return result;
        }

        /// <summary>
        /// 对实体属性施加业务语义标注。
        /// 先处理全局公共字段（CreateTime/ModifyTime），再按表名分支处理各实体的字段。
        /// </summary>
        private static void ApplySemantics(
            DomainEntityAttr x, string tableName,
            Dictionary<string, DomainEntity> entityMap)
        {
            // 快捷方法
            string? EntityId(string alias) =>
                entityMap.Values.FirstOrDefault(e => e.Alias == alias)?.Id;

            void FK(string alias, ForeignKeyType type = ForeignKeyType.Normal,
                    PolymorphicForeignKey? poly = null)
            {
                x.StructureRole = StructureRole.ForeignKey;
                x.ForeignKeyMetaData = new ForeignKeyMetaData
                { Type = type, TargetEntityId = EntityId(alias), Polymorphic = poly };
            }

            void Dim(DimensionCategory cat)
            {
                x.SemanticType = SemanticType.Dimension;
                x.DimensionCategory = cat;
            }

            void Measure(MeasureAggregation agg = MeasureAggregation.Basic)
            {
                x.SemanticType = SemanticType.Measure;
                x.SupportedAggregations = agg;
            }

            void Date(TimeDimensionGranularity gran)
            {
                x.SemanticType = SemanticType.Date;
                x.TimeGranularity = gran;
            }

            // ── 全局公共字段 ─────────────────────────────────────────────
            switch (x.ColumnName)
            {
                case "CreateTime":
                case "ModifyTime":
                    Date(TimeDimensionGranularity.秒);
                    return;
            }

            // ── 按表分支 ─────────────────────────────────────────────────
            switch (tableName)
            {
                case "Organization":
                    switch (x.ColumnName)
                    {
                        case "OrganizationName": x.StructureRole = StructureRole.TitleKey; break;
                        case "Status": Dim(DimensionCategory.分类); break;
                    }
                    break;

                case "Department":
                    switch (x.ColumnName)
                    {
                        case "DepartmentName":
                            x.StructureRole = StructureRole.TitleKey; break;
                        case "ParentDepartmentId":
                            FK("Department"); break;
                        case "OrganizationId":
                            FK("Organization"); break;
                        case "ManagerUserId":
                            FK("UserInfo"); Dim(DimensionCategory.角色); break;
                        case "DepartmentLevel":
                        case "Status":
                            Dim(DimensionCategory.分类); break;
                    }
                    break;

                case "UserInfo":
                    switch (x.ColumnName)
                    {
                        case "UserName": x.StructureRole = StructureRole.TitleKey; break;
                        case "OrganizationId": FK("Organization"); break;
                        case "DepartmentId": FK("Department"); break;
                        case "Status": Dim(DimensionCategory.分类); break;
                    }
                    break;

                case "Product":
                    switch (x.ColumnName)
                    {
                        case "ProductName": x.StructureRole = StructureRole.TitleKey; break;
                        case "ProductType":
                        case "ProductCategory":
                        case "Status": Dim(DimensionCategory.分类); break;
                    }
                    break;

                case "Customer":
                    switch (x.ColumnName)
                    {
                        case "CustomerName": x.StructureRole = StructureRole.TitleKey; break;
                        case "OwnerUserId": FK("UserInfo"); Dim(DimensionCategory.角色); break;
                        case "OwnerOrganizationId": FK("Organization"); Dim(DimensionCategory.组织); break;
                        case "CustomerType":
                        case "Industry":
                        case "CustomerLevel":
                        case "CreditStatus":
                        case "CustomerStatus":
                        case "CustomerTransactionType":
                        case "IsPublic": Dim(DimensionCategory.分类); break;
                        case "TotalOrderAmount":
                        case "TotalOrderCount": Measure(); break;
                    }
                    break;

                case "Opportunity":
                    switch (x.ColumnName)
                    {
                        case "OpportunityName": x.StructureRole = StructureRole.TitleKey; break;
                        case "CustomerId": FK("Customer"); Dim(DimensionCategory.主体); break;
                        case "OpportunityType":
                        case "WinProbability":
                        case "OpportunityStage":
                        case "NeedBid":
                        case "OpportunityStatus": Dim(DimensionCategory.分类); break;
                        case "ExpectedAmount": Measure(); break;
                        case "ExpectedClosingDate": Date(TimeDimensionGranularity.秒); break;
                    }
                    break;

                case "Quotation":
                    switch (x.ColumnName)
                    {
                        case "QuotationName": x.StructureRole = StructureRole.TitleKey; break;
                        case "OpportunityId": FK("Opportunity"); Dim(DimensionCategory.主体); break;
                        case "CustomerId": FK("Customer"); Dim(DimensionCategory.主体); break;
                        case "TotalAmount": Measure(); break;
                        case "QuotationDate": Date(TimeDimensionGranularity.日); break;
                        case "ValidityDate": Date(TimeDimensionGranularity.日); break;
                        case "QuotationStatus": Dim(DimensionCategory.分类); break;
                    }
                    break;

                case "QuotationDetail":
                    switch (x.ColumnName)
                    {
                        case "ProductName": x.StructureRole = StructureRole.TitleKey; break;
                        case "QuotationId": FK("Quotation"); Dim(DimensionCategory.主体); break;
                        case "ProductId": FK("Product"); Dim(DimensionCategory.主体); break;
                        case "Amount": Measure(); break;
                    }
                    break;

                case "Bid":
                    switch (x.ColumnName)
                    {
                        case "BidProjectName": x.StructureRole = StructureRole.TitleKey; break;
                        case "OpportunityId": FK("Opportunity"); Dim(DimensionCategory.主体); break;
                        case "CustomerId": FK("Customer"); Dim(DimensionCategory.主体); break;
                        case "ApprovalUserId": FK("UserInfo"); Dim(DimensionCategory.主体); break;
                        case "BidBondAmount": Measure(); break;
                        case "BidSubmissionDeadline":
                        case "BidBondPaidTime": Date(TimeDimensionGranularity.日); break;
                        case "BidBondPaid":
                        case "PerformanceBondRequired":
                        case "BidStatus":
                        case "ApprovalStatus": Dim(DimensionCategory.分类); break;
                    }
                    break;

                case "ProjectApproval":
                    switch (x.ColumnName)
                    {
                        case "ProjectName": x.StructureRole = StructureRole.TitleKey; break;
                        case "OpportunityId": FK("Opportunity"); Dim(DimensionCategory.主体); break;
                        case "CustomerId": FK("Customer"); Dim(DimensionCategory.主体); break;
                        case "ReferenceType": Dim(DimensionCategory.分类); break;
                        case "ReferenceId":
                            // 多态外键：ReferenceType=1→Quotation，2→Bid
                            FK("Quotation", ForeignKeyType.Polymorphic,
                                new PolymorphicForeignKey
                                {
                                    Mappings =
                                    [
                                        new PolymorphicMapping {DiscriminatorColumn = "ReferenceType",DiscriminatorValue = "1", TargetEntityId = EntityId("Quotation") },
                                        new PolymorphicMapping {DiscriminatorColumn = "ReferenceType",DiscriminatorValue = "2", TargetEntityId = EntityId("Bid") },
                                    ]
                                });
                            Dim(DimensionCategory.主体);
                            break;
                        case "EstimatedAmount":
                        case "EstimatedCost":
                        case "EstimatedProfit": Measure(); break;
                        case "ApprovalStatus": Dim(DimensionCategory.分类); break;
                    }
                    break;

                case "Contract":
                    switch (x.ColumnName)
                    {
                        case "ContractName": x.StructureRole = StructureRole.TitleKey; break;
                        case "ProjectApprovalId": FK("ProjectApproval"); Dim(DimensionCategory.主体); break;
                        case "OpportunityId": FK("Opportunity"); Dim(DimensionCategory.主体); break;
                        case "CustomerId": FK("Customer"); Dim(DimensionCategory.主体); break;
                        case "ApprovalUserId": FK("UserInfo"); Dim(DimensionCategory.主体); break;
                        case "ParentContractId": FK("Contract"); Dim(DimensionCategory.主体); break;
                        case "ContractType":
                        case "ContractStatus":
                        case "ApprovalStatus":
                        case "PaymentMethod": Dim(DimensionCategory.分类); break;
                        case "ContractAmount":
                        case "ContractAmountWithTax": Measure(); break;
                        case "SignDate":
                        case "ExpiryDate": Date(TimeDimensionGranularity.日); break;
                    }
                    break;

                case "ContractDetail":
                    switch (x.ColumnName)
                    {
                        case "ProductName": x.StructureRole = StructureRole.TitleKey; break;
                        case "ContractId": FK("Contract"); Dim(DimensionCategory.主体); break;
                        case "ProductId": FK("Product"); Dim(DimensionCategory.主体); break;
                        case "Unit": Dim(DimensionCategory.分类); break;
                        case "Quantity":
                        case "UnitPrice": Measure(); break;
                        case "DeliveryDate": Date(TimeDimensionGranularity.日); break;
                    }
                    break;

                case "ContractChange":
                    switch (x.ColumnName)
                    {
                        // 注：原代码用中文字段名"合同编号"是错误的，已修正为 ChangeNo
                        case "ContractCode": x.StructureRole = StructureRole.TitleKey; break;
                        case "ContractId": FK("Contract"); Dim(DimensionCategory.主体); break;
                        case "ChangeType": Dim(DimensionCategory.分类); break;
                    }
                    break;

                case "ContractHandover":
                    switch (x.ColumnName)
                    {
                        case "ContractId": FK("Contract"); Dim(DimensionCategory.主体); break;
                    }
                    break;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // 8. 实体间关系（DomainEntityRel）— 本体论层
        // ─────────────────────────────────────────────────────────────────────

        private static List<DomainEntityRel> BuildEntityRelations(
            Dictionary<string, DomainEntity> entityMap)
        {
            var rels = new List<DomainEntityRel>();
            int seq = 1;
  
            var N = "Many";
            var One = "One";
            var Ass = DomainEntityRelType.Association;
            var Agg = DomainEntityRelType.Aggregation;
            var Comp = DomainEntityRelType.Composition;
            var Dep = DomainEntityRelType.Dependency;

            // ── 组织架构 ──────────────────────────────────────────────────
            AddRel(rels, ref seq,
                EntityIds.Department, EntityIds.Organization,
                Agg, N, One, "下属部门", "所属组织",
                OntologyDeleteBehavior.Restrict,
                joins: DomainEntityRelJoin.Key("OrganizationId", "OrganizationId"));

            AddRel(rels, ref seq, 
                EntityIds.Department, EntityIds.Department,
                Ass, N, One, "子部门", "父部门",
                OntologyDeleteBehavior.Restrict,
                joins: DomainEntityRelJoin.Key("ParentDepartmentId", "DepartmentId"));

            AddRel(rels, ref seq,
               EntityIds.Department, EntityIds.UserInfo,
               Ass, One, One, "被管理部门", "负责人",
               OntologyDeleteBehavior.SetNull,
               joins: DomainEntityRelJoin.Key("ManagerUserId", "UserId"));

            AddRel(rels, ref seq,
                EntityIds.UserInfo, EntityIds.Department,
                Agg, N, One, "成员", "所属部门",
                OntologyDeleteBehavior.Restrict,
                joins: DomainEntityRelJoin.Key("DepartmentId", "DepartmentId"));

            AddRel(rels, ref seq, 
                EntityIds.UserInfo, EntityIds.Organization,
                Agg, N, One, "成员", "所属组织",
                OntologyDeleteBehavior.Restrict,
                joins: DomainEntityRelJoin.Key("OrganizationId", "OrganizationId"));

            AddRel(rels, ref seq,
                EntityIds.Product, EntityIds.Organization,
                Ass, N, One, "被创建产品", "创建组织",
                OntologyDeleteBehavior.SetNull,
                joins: DomainEntityRelJoin.Key("CreateOrganizationId", "OrganizationId"));

            AddRel(rels, ref seq,
                EntityIds.Product, EntityIds.UserInfo,
                Ass, N, One, "被创建产品", "创建者",
                OntologyDeleteBehavior.SetNull,
                joins: DomainEntityRelJoin.Key("CreateUserId", "UserId"));


            // ── 客户 ──────────────────────────────────────────────────────
            AddRel(rels, ref seq, 
                EntityIds.Customer, EntityIds.UserInfo,
                Ass, N, One, "所负责客户", "负责人",
                OntologyDeleteBehavior.NoAction,
                joins: DomainEntityRelJoin.Key("OwnerUserId", "UserId"));

            AddRel(rels, ref seq,
                EntityIds.Customer, EntityIds.Organization,
                Ass, N, One, "被创建客户", "所属组织",
                joins: DomainEntityRelJoin.Key("CreateOrganizationId", "OrganizationId"));

            AddRel(rels, ref seq,
                EntityIds.Customer, EntityIds.UserInfo,
                Ass, N, One, "被创建客户", "创建者",
                joins: DomainEntityRelJoin.Key("CreateUserId", "UserId"));

            // ── 商机 ──────────────────────────────────────────────────────
            AddRel(rels, ref seq,
                EntityIds.Opportunity, EntityIds.Customer,
                Agg, N, One, "客户商机", "关联客户",
                joins: DomainEntityRelJoin.Key("CustomerId", "CustomerId"));

            // ── 报价 ──────────────────────────────────────────────────────
            AddRel(rels, ref seq, 
                EntityIds.Quotation, EntityIds.Opportunity,
                Agg, N, One, "商机报价", "关联商机",
                joins: DomainEntityRelJoin.Key("OpportunityId", "OpportunityId"));

            AddRel(rels, ref seq, 
                EntityIds.QuotationDetail, EntityIds.Quotation,
                Comp, N, One, "报价明细", "报价单",
                cascade: OntologyDeleteBehavior.Cascade,
                joins: DomainEntityRelJoin.Key("QuotationId", "QuotationId"));

            AddRel(rels, ref seq, 
                EntityIds.QuotationDetail, EntityIds.Product,
                Ass, N, One, "被报价", "引用产品",
                joins: DomainEntityRelJoin.Key("ProductId", "ProductId"));

            // ── 投标 ──────────────────────────────────────────────────────
            AddRel(rels, ref seq,
                EntityIds.Bid, EntityIds.Opportunity,
                Agg, N, One, "商机投标", "关联商机",
                joins: DomainEntityRelJoin.Key("OpportunityId", "OpportunityId"));

            // ── 立项（多态：ReferenceType=1→Quotation, ReferenceType=2→Bid）
            AddRel(rels, ref seq, 
                EntityIds.ProjectApproval, EntityIds.Quotation,
                Ass, One, One, "触发立项", "来源报价",
                joins: [DomainEntityRelJoin.Key("ReferenceId", "QuotationId"), DomainEntityRelJoin.SourceConstant("ReferenceType", "1")]
                   );

            AddRel(rels, ref seq, 
                EntityIds.ProjectApproval, EntityIds.Bid,
                Ass, One, One, "触发立项", "来源投标",
                joins: [DomainEntityRelJoin.Key("ReferenceId", "BidId"), DomainEntityRelJoin.SourceConstant("ReferenceType", "2")]
                    );

            AddRel(rels, ref seq,
                EntityIds.ProjectApproval, EntityIds.Customer,
                Ass, N, One, "客户立项", "关联客户",
                joins: DomainEntityRelJoin.Key("CustomerId", "CustomerId"));

            // ── 合同 ──────────────────────────────────────────────────────
            AddRel(rels, ref seq,
                EntityIds.Contract, EntityIds.ProjectApproval,
                Agg, One, One, "来源立项", "立项合同",
                joins: DomainEntityRelJoin.Key("ProjectApprovalId", "ProjectApprovalId"));

            AddRel(rels, ref seq,
                EntityIds.Contract, EntityIds.Customer,
                Ass, N, One, "客户合同", "关联客户",
                joins: DomainEntityRelJoin.Key("CustomerId", "CustomerId"));

            AddRel(rels, ref seq, 
                EntityIds.Contract, EntityIds.Contract,
                Ass, N, One, "子合同", "父合同",
                joins: DomainEntityRelJoin.Key("ParentContractId", "ContractId"));

            AddRel(rels, ref seq, 
                EntityIds.ContractDetail, EntityIds.Contract,
                Comp, N, One, "合同明细", "所属合同",
                cascade: OntologyDeleteBehavior.Cascade,
                joins: DomainEntityRelJoin.Key("ContractId", "ContractId"));

            AddRel(rels, ref seq, 
                EntityIds.ContractDetail, EntityIds.Product,
                Dep, N, One, "引用产品", "被引用产品",
                joins: DomainEntityRelJoin.Key("ProductId", "ProductId"));

            AddRel(rels, ref seq, 
                EntityIds.ContractChange, EntityIds.Contract,
                Comp, One, One, "合同变更记录", "变更的合同",
                cascade: OntologyDeleteBehavior.Cascade,
                joins: DomainEntityRelJoin.Key("ContractId", "ContractId"));

            AddRel(rels, ref seq, 
                EntityIds.ContractHandover, EntityIds.Contract,
                Comp, One, One, "合同交底记录", "交底的合同",
                cascade: OntologyDeleteBehavior.Cascade,
                joins: DomainEntityRelJoin.Key("ContractId", "ContractId"));

            return rels;
        }

        /// <summary>
        /// 辅助方法：统一构建 DomainEntityRel，支持 string EntityId
        /// </summary>
        private static void AddRel(
            List<DomainEntityRel> rels,
            ref int seq,
            string srcEntityId, 
            string tgtEntityId,
            DomainEntityRelType type,
            string srcCard, 
            string tgtCard,
            string srcRole, 
            string tgtRole,
            OntologyDeleteBehavior cascade = OntologyDeleteBehavior.NoAction,
            params DomainEntityRelJoin[] joins)
            {
                rels.Add(new DomainEntityRel
                {
                    Id = $"CRM-REL-{seq++:D3}",
                    Type = type,
                    SourceEntityId = srcEntityId,
                    TargetEntityId = tgtEntityId,
                    SourceCardinality = srcCard,
                    TargetCardinality = tgtCard,
                    SourceRole = srcRole,
                    TargetRole = tgtRole,
                    CascadeDelete = cascade,
                    JoinConditions = joins.ToList(),
                });
            }

        // ─────────────────────────────────────────────────────────────────────
        // 9. 向量入库
        // ─────────────────────────────────────────────────────────────────────

        private async Task UpsertFieldEmbeddingsAsync(
            List<DomainEntity> entities,
            List<BusinessObjective> allDomains)
        {
            var domainMap = allDomains.ToDictionary(d => d.Id);
            var batch = new List<BusinessObjectiveFieldEmbeddingModel>();

            foreach (var entity in entities)
            {
                if (!domainMap.TryGetValue(entity.DomainId, out var domain)) continue;

                foreach (var attr in entity.Attrs)
                {
                    // Label 为空时降级用 ColumnName 作为 Embedding 文本
                    var embeddingText = string.IsNullOrWhiteSpace(attr.Label)
                        ? attr.ColumnName
                        : attr.Label;

                    batch.Add(new BusinessObjectiveFieldEmbeddingModel
                    {
                        Id = (ulong)attr.Id,
                        Embedding = await _vectorService.Embedding(embeddingText),
                        MetaDataId = attr.Id,
                        MetaDataName = attr.Label,
                        SemanticType = (int)(attr.SemanticType ?? SemanticType.Attribute),
                        MetaDataDescription = attr.Description,
                        EntityId = entity.Id,
                        ObjectiveMetaDataId = domain.Id,
                        ObjectiveMetaDataName = domain.BusinessName,
                    });
                }
            }

            await _vectorService.UpdateAsync<ulong, BusinessObjectiveFieldEmbeddingModel>(
                "business_objective_field", batch);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 工具方法
        // ─────────────────────────────────────────────────────────────────────

        private static string? CleanDefaultValue(object? value)
        {
            if (value is null or DBNull) return null;
            var str = value.ToString();
            if (string.IsNullOrWhiteSpace(str)) return null;
            while (str.StartsWith('(') && str.EndsWith(')'))
                str = str[1..^1];
            return str;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SQL Server 类型 → DataType 枚举转换
    // ─────────────────────────────────────────────────────────────────────────

    public static class DataTypeConverter
    {
        private static readonly HashSet<string> NumericTypes = new(StringComparer.OrdinalIgnoreCase)
            { "int","bigint","smallint","tinyint","decimal","numeric","float","real","money","smallmoney" };

        private static readonly HashSet<string> DateTypes = new(StringComparer.OrdinalIgnoreCase)
            { "datetime","datetime2","date","time","smalldatetime","datetimeoffset" };

        private static readonly HashSet<string> StringTypes = new(StringComparer.OrdinalIgnoreCase)
            { "varchar","nvarchar","char","nchar","text","ntext","xml" };

        public static DataType Convert(string sqlType)
        {
            if (string.IsNullOrWhiteSpace(sqlType)) return DataType.String;

            // 去除长度修饰，如 varchar(200) → varchar
            var baseType = sqlType.Split('(')[0].Trim();

            if (NumericTypes.Contains(baseType)) return DataType.Number;
            if (DateTypes.Contains(baseType)) return DataType.Date;
            if (baseType.Equals("bit", StringComparison.OrdinalIgnoreCase)) return DataType.Boolean;
            if (baseType.Equals("json", StringComparison.OrdinalIgnoreCase)) return DataType.Json;
            if (StringTypes.Contains(baseType)) return DataType.String;

            return DataType.String;
        }
    }
}