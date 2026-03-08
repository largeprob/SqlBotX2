using JasperFx.Events.Daemon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives.Embeddings;
using SqlBoTx.Net.Application.Vectors;
using SqlBoTx.Net.Domain.BusinessObjectives;
using SqlBoTx.Net.Domain.DatabaseConnections;
using SqlBoTx.Net.Domain.Share.Enums;
using SqlBoTx.Net.Domain.Share.Enums.BusinessObjective;
using SqlBoTx.Net.Domain.Share.TableRelationships;
using SqlBoTx.Net.Domain.TableFields;
using SqlBoTx.Net.Domain.TableRelationships;
using SqlBoTx.Net.Domain.TableStructures;
using SqlBoTx.Net.EFCore;

namespace SqlBoTx.Net.DbManager.SeedDatas
{
    public class LocalDb_CRM
    {
        private readonly SqlBotxDBContext _sqlBotxDBContext;

        private readonly QdrantVectorService _vectorService;

        public LocalDb_CRM(SqlBotxDBContext sqlBotxDBContext, QdrantVectorService vectorService)
        {
            _sqlBotxDBContext = sqlBotxDBContext;
            _vectorService = vectorService;
        }

        private const int ConnectionId = 1;

        public async Task RunAsync()
        {
            await InitTableAsync();
        }

        public async Task InitTableAsync()
        {
            if (await _sqlBotxDBContext.Set<TableStructure>().CountAsync() > 0)
            {
                return;
            }

            await _sqlBotxDBContext.AddRangeAsync(new DatabaseConnection
            {
                ConnectionName = "本地SQLServer",
                ConnectionType =  ConnectionType.SQLServer,
                ConnectionString = ".",
                UserName = "sa",
                UserPassword = "123456",
            });
            await _sqlBotxDBContext.SaveChangesAsync();

            //表结构
            var tables = new List<TableStructure>() {
                GetOrganization(), Department(),GetUserInfoTable(), Product(), Customer(),
                Opportunity(), Quotation(), Bid(), ProjectApproval(),
                Contract(), ContractDetail(), ContractChange(), ContractHandover()
            };
            await _sqlBotxDBContext.AddRangeAsync(tables);
            await _sqlBotxDBContext.SaveChangesAsync();

            var tableNameToIdMap = new Dictionary<string, int>();
            for (int i = 0; i < tables.Count; i++)
            {
                tableNameToIdMap[tables[i].TableName] = i + 1;
            }

            //表关系
            var relationships = GenerateRelationships(tableNameToIdMap);
            await _sqlBotxDBContext.AddRangeAsync(relationships);
            await _sqlBotxDBContext.SaveChangesAsync();

            //业务分层
            var businessObjectives = GenerateBusinessObjective(tableNameToIdMap, tables);
            await _sqlBotxDBContext.AddRangeAsync(businessObjectives);
            await _sqlBotxDBContext.SaveChangesAsync();

            //将数据库字段存入向量库
            var insertVectorBusinessObjectives = new List<BusinessObjectiveEmbeddingModel>();
            var insertVectorBusinessObjectivesSynonyms = new List<BusinessObjectiveSynonymEmbeddingModel>();
            var insertVectorBusinessObjectivesField = new List<BusinessObjectiveFieldEmbeddingModel>();
            foreach (var objective in businessObjectives)
            {
                //业务目标向量
                insertVectorBusinessObjectives.Add(new BusinessObjectiveEmbeddingModel
                {
                    Id = (ulong)objective.Id,
                    Embedding = await _vectorService.Embedding(objective.BusinessName),
                    MetaDataId = objective.Id,
                    MetaDataName = objective.BusinessName,
                    MetaDataDescription = objective.Description
                });

                //近义词向量
                if (!string.IsNullOrWhiteSpace(objective.Synonyms))
                {
                    var synonymsArr = objective.Synonyms!.Split(",");
                    for (int i = 0; i < synonymsArr.Count(); i++)
                    {
                        insertVectorBusinessObjectivesSynonyms.Add(new BusinessObjectiveSynonymEmbeddingModel
                        {
                            Id = Guid.CreateVersion7(),
                            Embedding = await _vectorService.Embedding(synonymsArr[i]),
                            MataData = synonymsArr[i],
                            ObjectiveMetaDataId = objective.Id,
                            ObjectiveMetaDataName = objective.BusinessName,
                            ObjectiveMetaDataDescription = objective.Description,
                        });
                    }
                }
              
                //字段向量
                foreach (var field in objective.Fields)
                {
                    insertVectorBusinessObjectivesField.Add(new BusinessObjectiveFieldEmbeddingModel
                    {
                        Id = (ulong)field.Id,
                        Embedding = await _vectorService.Embedding(field.Name),

                        MetaDataId = field.Id,
                        MetaDataType = "field",
                        MetaDataBusinesBIRole = (int)(field.BusinesBIRole ?? 0),
                        MetaDataName = field.Name,
                        MetaDataDescription = field.Description,

                        ObjectiveMetaDataId = objective.Id,
                        ObjectiveMetaDataName = objective.BusinessName,
                        ObjectiveMetaDataDescription = objective.Description,
                    });
                }
            }

            await _vectorService.UpdateAsync<ulong, BusinessObjectiveEmbeddingModel>("business_objective", insertVectorBusinessObjectives);
            await _vectorService.UpdateAsync<Guid, BusinessObjectiveSynonymEmbeddingModel>("business_objective_synonyms", insertVectorBusinessObjectivesSynonyms);
            await _vectorService.UpdateAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", insertVectorBusinessObjectivesField);
        }

        #region 表结构
        public TableStructure GetOrganization()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "Organization",
                DisplayName = "组织架构表",
                Description = "组织架构表，存储公司组织架构信息，支持多级组织管理",
                Granularity = "每个组织单元(公司/事业部)",
                GranularityLevel =  TableStructureGranularityLevel.Dimension,
            };

            var fields = new List<TableField>
            {
                new TableField { ColumnName = "OrganizationId", FieldName = "组织ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
                new TableField { ColumnName = "OrganizationCode", FieldName = "组织编码", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "唯一标识组织的代码" },
                new TableField { ColumnName = "OrganizationName", FieldName = "组织名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "ParentOrganizationId", FieldName = "上级组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "自关联外键，指向父级组织" },
                new TableField { ColumnName = "ParentOrganizationCode", FieldName = "上级组织编码", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "ParentOrganizationName", FieldName = "上级组织名称", DataType = "NVARCHAR(200)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "OrganizationType", FieldName = "组织类型", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-公司，2-事业部，3-部门，4-其他" },
                new TableField { ColumnName = "OrganizationLevel", FieldName = "组织层级", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "从1开始计数" },
                new TableField { ColumnName = "OrganizationPath", FieldName = "组织路径", DataType = "NVARCHAR(500)", IsNullable = false, IsAvailable = true, FieldDescription = "显示组织的完整层级路径" },
                new TableField { ColumnName = "ContactPerson", FieldName = "组织联系人", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "ContactPhone", FieldName = "联系电话", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "Address", FieldName = "组织地址", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "Description", FieldName = "组织描述", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "详细说明组织职能和职责" },
                new TableField { ColumnName = "Status", FieldName = "状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-启用，0-停用" },
                new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "默认当前时间" },
                new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "ModifyTime", FieldName = "最后修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
                new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
            };

            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure GetUserInfoTable()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "UserInfo",
                DisplayName = "用户信息表",
                Description = "用户信息表，存储系统用户数据",
                Granularity = "每个系统用户",
                GranularityLevel = TableStructureGranularityLevel.Dimension,
            };

            var fields = new List<TableField>
{
    new TableField { ColumnName = "UserId", FieldName = "用户ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, FieldDescription = "主键" },
    new TableField { ColumnName = "UserCode", FieldName = "用户编码", DataType = "NVARCHAR(50)", IsNullable = false, FieldDescription = "唯一标识" },
    new TableField { ColumnName = "UserName", FieldName = "用户姓名", DataType = "NVARCHAR(100)", IsNullable = false, FieldDescription = "" },
    new TableField { ColumnName = "LoginName", FieldName = "登录名", DataType = "NVARCHAR(50)", IsNullable = false, FieldDescription = "" },
    new TableField { ColumnName = "PasswordHash", FieldName = "密码哈希值", DataType = "NVARCHAR(200)", IsNullable = false, FieldDescription = "" },
    new TableField { ColumnName = "Email", FieldName = "邮箱", DataType = "NVARCHAR(100)", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "Phone", FieldName = "电话", DataType = "NVARCHAR(20)", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "OrganizationId", FieldName = "组织ID", DataType = "INT", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "DepartmentId", FieldName = "部门ID", DataType = "INT", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "DepartmentName", FieldName = "部门名称", DataType = "NVARCHAR(100)", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "Position", FieldName = "职位", DataType = "NVARCHAR(50)", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "UserType", FieldName = "用户类型", DataType = "INT", IsNullable = false, DefaultValue = "1", FieldDescription = "1-销售，2-管理员，3-审核人，4-交付人员" },
    new TableField { ColumnName = "Status", FieldName = "状态", DataType = "INT", IsNullable = false, DefaultValue = "1", FieldDescription = "1-启用，0-停用" },
    new TableField { ColumnName = "LastLoginTime", FieldName = "最后登录时间", DataType = "DATETIME", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "Remark", FieldName = "备注", DataType = "NVARCHAR(500)", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", FieldDescription = "" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "修改时间", DataType = "DATETIME", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, FieldDescription = "" }
};

            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure Department()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "Department",
                DisplayName = "部门表",
                Description = "部门表，存储公司部门组织架构信息，支持多级部门管理",
                Granularity = "每个部门",
                GranularityLevel = TableStructureGranularityLevel.Dimension,
            };

            var fields = new List<TableField>
{
    new TableField { ColumnName = "DepartmentId", FieldName = "部门ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
    new TableField { ColumnName = "DepartmentCode", FieldName = "部门编码", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "唯一标识部门的代码" },
    new TableField { ColumnName = "DepartmentName", FieldName = "部门名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ParentDepartmentId", FieldName = "上级部门ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "自关联外键，指向父级部门" },
    new TableField { ColumnName = "ParentDepartmentCode", FieldName = "上级部门编码", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ParentDepartmentName", FieldName = "上级部门名称", DataType = "NVARCHAR(200)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OrganizationId", FieldName = "所属组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联到组织表" },
    new TableField { ColumnName = "OrganizationCode", FieldName = "所属组织编码", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OrganizationName", FieldName = "所属组织名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "DepartmentType", FieldName = "部门类型", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-销售部，2-技术部，3-财务部，4-人事部，5-行政部，6-其他" },
    new TableField { ColumnName = "DepartmentLevel", FieldName = "部门层级", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "从1开始计数，1为一级部门" },
    new TableField { ColumnName = "DepartmentPath", FieldName = "部门路径", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "显示部门的完整层级路径" },
    new TableField { ColumnName = "ManagerUserId", FieldName = "部门负责人用户ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "关联用户表" },
    new TableField { ColumnName = "ManagerUserName", FieldName = "部门负责人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContactPhone", FieldName = "部门联系电话", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Description", FieldName = "部门描述", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "详细说明部门职能和职责" },
    new TableField { ColumnName = "Status", FieldName = "状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-启用，0-停用" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "默认当前时间" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "最后修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
};
            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure Product()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "Product",
                DisplayName = "产品信息表",
                Description = "产品信息表，存储所有可销售的产品数据",
                Granularity = "每个产品",
                GranularityLevel = TableStructureGranularityLevel.Dimension,
            };

            var fields = new List<TableField>
{
    new TableField { ColumnName = "ProductId", FieldName = "产品ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
    new TableField { ColumnName = "ProductCode", FieldName = "产品编码", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "唯一标识" },
    new TableField { ColumnName = "ProductName", FieldName = "产品名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ProductType", FieldName = "产品类型", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ProductCategory", FieldName = "产品分类", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ProductSpecification", FieldName = "产品规格描述", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Unit", FieldName = "计量单位", DataType = "NVARCHAR(20)", IsNullable = false, IsAvailable = true, FieldDescription = "如：个、台、套" },
    new TableField { ColumnName = "UnitPrice", FieldName = "标准单价", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CostPrice", FieldName = "成本价", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "MinimumStock", FieldName = "最低库存", DataType = "INT", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CurrentStock", FieldName = "当前库存", DataType = "INT", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Status", FieldName = "状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-启用，0-停用" },
    new TableField { ColumnName = "Remark", FieldName = "备注", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
};

            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure Customer()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "Customer",
                DisplayName = "客户信息表",
                Description = "客户信息表，统一管理公海客户和正式客户",
                Granularity = "每个客户",
                GranularityLevel = TableStructureGranularityLevel.Dimension,
            };

            var fields = new List<TableField>
{
    new TableField { ColumnName = "CustomerId", FieldName = "客户ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
    new TableField { ColumnName = "CustomerCode", FieldName = "客户编码", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "唯一标识" },
    new TableField { ColumnName = "CustomerName", FieldName = "客户名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerType", FieldName = "客户类型", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-企业，2-个人，3-政府，4-其他" },
    new TableField { ColumnName = "CustomerSource", FieldName = "客户来源", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "如：展会、网络搜索、老客户介绍等" },
    new TableField { ColumnName = "Industry", FieldName = "所属行业", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContactPerson", FieldName = "联系人", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContactPhone", FieldName = "联系电话", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContactEmail", FieldName = "联系邮箱", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Address", FieldName = "地址", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerLevel", FieldName = "客户等级", DataType = "INT", IsNullable = true, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-A级（重要），2-B级（一般），3-C级（潜在）" },
    new TableField { ColumnName = "CreditStatus", FieldName = "信用状态", DataType = "INT", IsNullable = true, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-良好，2-一般，3-受限" },
    new TableField { ColumnName = "CustomerStatus", FieldName = "客户状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-公海客户，2-已分配待跟进，3-正式客户，4-流失客户" },
    new TableField { ColumnName = "CustomerTransactionType", FieldName = "客户交易类型", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "IsPublic", FieldName = "是否公海客户", DataType = "BIT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-是，0-否" },
    new TableField { ColumnName = "OwnerUserId", FieldName = "负责人用户ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "如果为空则表示在公海中" },
    new TableField { ColumnName = "OwnerUserName", FieldName = "负责人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OwnerOrganizationId", FieldName = "负责人组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OwnerOrganizationName", FieldName = "负责人组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "RecentContactTime", FieldName = "最近联系时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "用于判断客户是否活跃" },
    new TableField { ColumnName = "TotalOrderAmount", FieldName = "累计订单金额", DataType = "DECIMAL(18,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TotalOrderCount", FieldName = "累计订单数量", DataType = "INT", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "LastOrderTime", FieldName = "最后下单时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Remark", FieldName = "备注", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
};

            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure Opportunity()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "Opportunity",
                DisplayName = "商机信息表",
                Description = "商机信息表，记录销售机会的全过程",
                Granularity = "每个商机",
                GranularityLevel = TableStructureGranularityLevel.Fact,
            };

            var fields = new List<TableField>
{
    new TableField { ColumnName = "OpportunityId", FieldName = "商机ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
    new TableField { ColumnName = "OpportunityCode", FieldName = "商机编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "唯一标识" },
    new TableField { ColumnName = "OpportunityName", FieldName = "商机名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerId", FieldName = "客户ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Customer表" },
    new TableField { ColumnName = "CustomerName", FieldName = "客户名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerType", FieldName = "客户类型", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OpportunityType", FieldName = "商机类型", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-新客户商机，2-老客户商机" },
    new TableField { ColumnName = "ProjectBackground", FieldName = "项目背景", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerNeed", FieldName = "客户需求", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ExpectedAmount", FieldName = "预计金额", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ExpectedClosingDate", FieldName = "预计成交日期", DataType = "DATE", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "WinProbability", FieldName = "赢单概率", DataType = "INT", IsNullable = false, DefaultValue = "30", IsAvailable = true, FieldDescription = "取值范围0-100" },
    new TableField { ColumnName = "CompetitorInfo", FieldName = "竞争对手信息", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OpportunityStage", FieldName = "商机阶段", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-初步接触，2-需求分析，3-方案制定，4-报价中，5-谈判中，6-赢单，7-丢单" },
    new TableField { ColumnName = "NeedBid", FieldName = "是否需要投标", DataType = "BIT", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "0-否（走报价流程），1-是（走投标流程）" },
    new TableField { ColumnName = "BidProjectCode", FieldName = "招标项目编号", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "SalesOwnerUserId", FieldName = "销售负责人用户ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "SalesOwnerUserName", FieldName = "销售负责人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "SalesOwnerOrganizationId", FieldName = "销售负责人组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "SalesOwnerOrganizationName", FieldName = "销售负责人组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CoSalesUserIds", FieldName = "协同销售人员ID列表", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "多个ID用逗号分隔" },
    new TableField { ColumnName = "CoSalesUserNames", FieldName = "协同销售人员姓名列表", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "多个姓名用逗号分隔" },
    new TableField { ColumnName = "CurrentTask", FieldName = "当前任务", DataType = "NVARCHAR(200)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "NextStep", FieldName = "下一步计划", DataType = "NVARCHAR(200)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Remark", FieldName = "备注", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OpportunityStatus", FieldName = "商机状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-进行中，2-已关闭，3-已取消" },
    new TableField { ColumnName = "CloseReason", FieldName = "关闭原因", DataType = "NVARCHAR(200)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CloseTime", FieldName = "关闭时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
};

            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure Quotation()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "Quotation",
                DisplayName = "销售报价表",
                Description = "销售报价表，记录普通项目的报价信息",
                Granularity = "每个报价单",
                GranularityLevel = TableStructureGranularityLevel.Fact,
            };

            var fields = new List<TableField>
{
    new TableField { ColumnName = "QuotationId", FieldName = "报价ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
    new TableField { ColumnName = "QuotationCode", FieldName = "报价单号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "唯一标识" },
    new TableField { ColumnName = "OpportunityId", FieldName = "商机ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Opportunity表" },
    new TableField { ColumnName = "OpportunityCode", FieldName = "商机编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerId", FieldName = "客户ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Customer表" },
    new TableField { ColumnName = "CustomerName", FieldName = "客户名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "QuotationDate", FieldName = "报价日期", DataType = "DATE", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ValidUntilDate", FieldName = "有效期至", DataType = "DATE", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TotalAmount", FieldName = "报价总金额", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TaxRate", FieldName = "税率", DataType = "DECIMAL(5,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TaxAmount", FieldName = "税额", DataType = "DECIMAL(18,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TotalAmountWithTax", FieldName = "含税总金额", DataType = "DECIMAL(18,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Currency", FieldName = "币种", DataType = "NVARCHAR(10)", IsNullable = true, DefaultValue = "'CNY'", IsAvailable = true, FieldDescription = "默认CNY（人民币）" },
    new TableField { ColumnName = "PaymentTerms", FieldName = "付款条款", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "DeliveryTerms", FieldName = "交货条款", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "WarrantyTerms", FieldName = "保修条款", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "SpecialTerms", FieldName = "特殊条款", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "QuotationStatus", FieldName = "报价状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-草稿，2-已提交，3-客户确认中，4-客户已确认，5-客户拒绝，6-已过期" },
    new TableField { ColumnName = "ApprovalStatus", FieldName = "审批状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-未提交，2-审批中，3-已通过，4-已驳回" },
    new TableField { ColumnName = "ApprovalFlowId", FieldName = "审批流程ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalUserId", FieldName = "审批人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalUserName", FieldName = "审批人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalTime", FieldName = "审批时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalComment", FieldName = "审批意见", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerFeedback", FieldName = "客户反馈", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerConfirmTime", FieldName = "客户确认时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "NextFollowUpTime", FieldName = "下次跟进时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Remark", FieldName = "备注", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
};
            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure Bid()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "Bid",
                DisplayName = "销售投标表",
                Description = "销售投标表，记录招投标项目的投标信息",
                Granularity = "每个投标项目",
                GranularityLevel = TableStructureGranularityLevel.Fact,
            };

            var fields = new List<TableField>
{
    new TableField { ColumnName = "BidId", FieldName = "投标ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
    new TableField { ColumnName = "BidCode", FieldName = "投标编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "唯一标识" },
    new TableField { ColumnName = "OpportunityId", FieldName = "商机ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Opportunity表" },
    new TableField { ColumnName = "OpportunityCode", FieldName = "商机编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerId", FieldName = "客户ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Customer表" },
    new TableField { ColumnName = "CustomerName", FieldName = "客户名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "BidProjectName", FieldName = "招标项目名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "BidProjectCode", FieldName = "招标项目编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TenderOrganization", FieldName = "招标单位", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TenderAddress", FieldName = "招标地址", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "BidOpeningDate", FieldName = "开标日期", DataType = "DATETIME", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "BidSubmissionDeadline", FieldName = "投标截止日期", DataType = "DATETIME", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "BidBondAmount", FieldName = "投标保证金金额", DataType = "DECIMAL(18,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "BidBondPaid", FieldName = "投标保证金是否已支付", DataType = "BIT", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "BidBondPaidTime", FieldName = "保证金支付时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "PerformanceBondRequired", FieldName = "是否需要履约保证金", DataType = "BIT", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "PerformanceBondRate", FieldName = "履约保证金比例", DataType = "DECIMAL(5,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TechnicalScore", FieldName = "技术得分", DataType = "DECIMAL(5,2)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CommercialScore", FieldName = "商务得分", DataType = "DECIMAL(5,2)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TotalScore", FieldName = "总分", DataType = "DECIMAL(5,2)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Ranking", FieldName = "排名", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Competitors", FieldName = "竞争对手信息", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OurAdvantages", FieldName = "我方优势", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OurDisadvantages", FieldName = "我方劣势", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "BidStatus", FieldName = "投标状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-准备中，2-已提交，3-已开标，4-中标，5-未中标，6-废标" },
    new TableField { ColumnName = "ApprovalStatus", FieldName = "审批状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-未提交，2-审批中，3-已通过，4-已驳回" },
    new TableField { ColumnName = "ApprovalFlowId", FieldName = "审批流程ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalUserId", FieldName = "审批人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalUserName", FieldName = "审批人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalTime", FieldName = "审批时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalComment", FieldName = "审批意见", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "WinNoticeTime", FieldName = "中标通知书时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "WinNoticeNumber", FieldName = "中标通知书编号", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "BidDocumentPath", FieldName = "投标文件路径", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OtherAttachments", FieldName = "其他附件路径", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Remark", FieldName = "备注", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
};

            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure ProjectApproval()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "ProjectApproval",
                DisplayName = "销售立项表",
                Description = "销售立项表，记录内部评审和立项信息",
                Granularity = "每个立项申请",
                GranularityLevel = TableStructureGranularityLevel.Fact,
            };

            var fields = new List<TableField>
{
    new TableField { ColumnName = "ProjectApprovalId", FieldName = "立项ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
    new TableField { ColumnName = "ProjectApprovalCode", FieldName = "立项编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "唯一标识" },
    new TableField { ColumnName = "ReferenceType", FieldName = "引用类型", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "1-报价，2-投标" },
    new TableField { ColumnName = "ReferenceId", FieldName = "引用ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "根据ReferenceType对应QuotationId或BidId" },
    new TableField { ColumnName = "ReferenceCode", FieldName = "引用编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "根据ReferenceType对应QuotationCode或BidCode" },
    new TableField { ColumnName = "OpportunityId", FieldName = "商机ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Opportunity表" },
    new TableField { ColumnName = "OpportunityCode", FieldName = "商机编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerId", FieldName = "客户ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Customer表" },
    new TableField { ColumnName = "CustomerName", FieldName = "客户名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ProjectName", FieldName = "项目名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ProjectBackground", FieldName = "项目背景", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerDemand", FieldName = "客户需求", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "SolutionOverview", FieldName = "解决方案概述", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "EstimatedAmount", FieldName = "预估金额", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "EstimatedCost", FieldName = "预估成本", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "EstimatedProfit", FieldName = "预估利润", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ProfitMargin", FieldName = "毛利率", DataType = "DECIMAL(5,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ResourceRequirements", FieldName = "资源需求", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ImplementationPeriod", FieldName = "实施周期", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "RiskAnalysis", FieldName = "风险分析", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "RiskMitigationMeasures", FieldName = "风险应对措施", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "SalesDepartmentOpinion", FieldName = "销售部门意见", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TechnicalDepartmentOpinion", FieldName = "技术部门意见", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "FinanceDepartmentOpinion", FieldName = "财务部门意见", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ManagementOpinion", FieldName = "管理层意见", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalStatus", FieldName = "审批状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-草稿，2-销售审批中，3-技术审批中，4-财务审批中，5-管理层审批中，6-已通过，7-已驳回" },
    new TableField { ColumnName = "CurrentApprovalStage", FieldName = "当前审批阶段", DataType = "INT", IsNullable = true, DefaultValue = "1", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalFlowId", FieldName = "审批流程ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "FinalApproverUserId", FieldName = "最终审批人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "FinalApproverUserName", FieldName = "最终审批人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "FinalApprovalTime", FieldName = "最终审批时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalComment", FieldName = "审批意见", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "IsUrgent", FieldName = "是否加急", DataType = "BIT", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "UrgentReason", FieldName = "加急原因", DataType = "NVARCHAR(200)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Remark", FieldName = "备注", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
};

            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure Contract()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "Contract",
                DisplayName = "销售合同表",
                Description = "销售合同表，记录正式签订的合同信息",
                Granularity = "每个合同",
                GranularityLevel = TableStructureGranularityLevel.Fact,
            };
            var fields = new List<TableField>
{
    new TableField { ColumnName = "ContractId", FieldName = "合同ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
    new TableField { ColumnName = "ContractCode", FieldName = "合同编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "唯一标识" },
    new TableField { ColumnName = "ProjectApprovalId", FieldName = "立项ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联ProjectApproval表" },
    new TableField { ColumnName = "ProjectApprovalCode", FieldName = "立项编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OpportunityId", FieldName = "商机ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Opportunity表" },
    new TableField { ColumnName = "OpportunityCode", FieldName = "商机编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerId", FieldName = "客户ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Customer表" },
    new TableField { ColumnName = "CustomerName", FieldName = "客户名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContractName", FieldName = "合同名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContractType", FieldName = "合同类型", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-销售合同，2-服务合同，3-框架协议，4-补充协议" },
    new TableField { ColumnName = "ContractAmount", FieldName = "合同金额", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TaxRate", FieldName = "税率", DataType = "DECIMAL(5,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TaxAmount", FieldName = "税额", DataType = "DECIMAL(18,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContractAmountWithTax", FieldName = "含税合同金额", DataType = "DECIMAL(18,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Currency", FieldName = "币种", DataType = "NVARCHAR(10)", IsNullable = true, DefaultValue = "'CNY'", IsAvailable = true, FieldDescription = "默认CNY（人民币）" },
    new TableField { ColumnName = "SignDate", FieldName = "签订日期", DataType = "DATE", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "EffectiveDate", FieldName = "生效日期", DataType = "DATE", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ExpiryDate", FieldName = "到期日期", DataType = "DATE", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContractPeriod", FieldName = "合同期限", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "PaymentMethod", FieldName = "付款方式", DataType = "NVARCHAR(200)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "DeliveryRequirements", FieldName = "交付要求", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "QualityRequirements", FieldName = "质量要求", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "WarrantyPeriod", FieldName = "质保期", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContractTerms", FieldName = "合同条款", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "SpecialAgreements", FieldName = "特殊约定", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContractStatus", FieldName = "合同状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-草稿，2-审批中，3-已生效，4-履行中，5-已完成，6-已终止，7-已作废" },
    new TableField { ColumnName = "ApprovalStatus", FieldName = "审批状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-未提交，2-审批中，3-已通过，4-已驳回" },
    new TableField { ColumnName = "ApprovalFlowId", FieldName = "审批流程ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalUserId", FieldName = "审批人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalUserName", FieldName = "审批人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalTime", FieldName = "审批时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalComment", FieldName = "审批意见", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContractFilePath", FieldName = "合同文件路径", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ContractScanPath", FieldName = "合同扫描件路径", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "IsTemplate", FieldName = "是否模板合同", DataType = "BIT", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ParentContractId", FieldName = "父合同ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "用于补充协议" },
    new TableField { ColumnName = "Remark", FieldName = "备注", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
};

            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure ContractDetail()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "ContractDetail",
                DisplayName = "销售合同明细表",
                Description = "合同明细表，记录合同的产品明细信息",
                Granularity = "每个合同明细行",
                GranularityLevel = TableStructureGranularityLevel.Fact,
            };
            var fields = new List<TableField>
{
    new TableField { ColumnName = "ContractDetailId", FieldName = "合同明细ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
    new TableField { ColumnName = "ContractId", FieldName = "合同ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Contract表" },
    new TableField { ColumnName = "ContractCode", FieldName = "合同编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "LineNumber", FieldName = "行号", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "同一合同内唯一" },
    new TableField { ColumnName = "ProductId", FieldName = "产品ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Product表" },
    new TableField { ColumnName = "ProductCode", FieldName = "产品编码", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ProductName", FieldName = "产品名称", DataType = "NVARCHAR(200)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ProductSpecification", FieldName = "产品规格", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Unit", FieldName = "计量单位", DataType = "NVARCHAR(20)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Quantity", FieldName = "数量", DataType = "DECIMAL(18,4)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "UnitPrice", FieldName = "单价", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Amount", FieldName = "金额", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "数量×单价" },
    new TableField { ColumnName = "TaxRate", FieldName = "税率", DataType = "DECIMAL(5,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TaxAmount", FieldName = "税额", DataType = "DECIMAL(18,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "AmountWithTax", FieldName = "含税金额", DataType = "DECIMAL(18,2)", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "DeliveryDate", FieldName = "要求交货日期", DataType = "DATE", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "DeliveryAddress", FieldName = "交货地址", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TechnicalParameters", FieldName = "技术参数", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "QualityStandard", FieldName = "质量标准", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Remark", FieldName = "备注", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
};

            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure ContractChange()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "ContractChange",
                DisplayName = "销售合同变更表",
                Description = "合同变更表，记录合同的变更申请和审批信息",
                Granularity = "每个合同变更申请",
                GranularityLevel = TableStructureGranularityLevel.Fact,
            };

            var fields = new List<TableField>
{
    new TableField { ColumnName = "ContractChangeId", FieldName = "合同变更ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
    new TableField { ColumnName = "ContractChangeCode", FieldName = "变更单号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "唯一标识" },
    new TableField { ColumnName = "ContractId", FieldName = "合同ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Contract表" },
    new TableField { ColumnName = "ContractCode", FieldName = "合同编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ChangeType", FieldName = "变更类型", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-金额变更，2-范围变更，3-时间变更，4-条款变更，5-综合变更" },
    new TableField { ColumnName = "ChangeReason", FieldName = "变更原因", DataType = "NVARCHAR(MAX)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ChangeContent", FieldName = "变更内容", DataType = "NVARCHAR(MAX)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "OriginalAmount", FieldName = "原合同金额", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ChangeAmount", FieldName = "变更金额", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "正数为增加，负数为减少" },
    new TableField { ColumnName = "NewAmount", FieldName = "新合同金额", DataType = "DECIMAL(18,2)", IsNullable = false, DefaultValue = "0", IsAvailable = true, FieldDescription = "原合同金额+变更金额" },
    new TableField { ColumnName = "OriginalExpiryDate", FieldName = "原到期日期", DataType = "DATE", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "NewExpiryDate", FieldName = "新到期日期", DataType = "DATE", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ChangeAgreementPath", FieldName = "变更协议文件路径", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerConfirm", FieldName = "客户是否确认", DataType = "BIT", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerConfirmTime", FieldName = "客户确认时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerConfirmPerson", FieldName = "客户确认人", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerConfirmDocumentPath", FieldName = "客户确认文件路径", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalStatus", FieldName = "审批状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-草稿，2-审批中，3-已通过，4-已驳回" },
    new TableField { ColumnName = "ApprovalFlowId", FieldName = "审批流程ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalUserId", FieldName = "审批人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalUserName", FieldName = "审批人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalTime", FieldName = "审批时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ApprovalComment", FieldName = "审批意见", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ChangeStatus", FieldName = "变更状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-待处理，2-处理中，3-已完成，4-已取消" },
    new TableField { ColumnName = "ImplementTime", FieldName = "实施时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ImplementUserId", FieldName = "实施人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ImplementUserName", FieldName = "实施人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ImplementResult", FieldName = "实施结果", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Remark", FieldName = "备注", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
};

            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }

        public TableStructure ContractHandover()
        {
            var table = new TableStructure
            {
                ConnectionId = ConnectionId,
                TableName = "ContractHandover",
                DisplayName = "合同交底表",
                Description = "合同交底表，记录合同签署后的交底信息",
                Granularity = "每个合同交底记录",
                GranularityLevel = TableStructureGranularityLevel.Dimension,
            };

            var fields = new List<TableField>
{
    new TableField { ColumnName = "ContractHandoverId", FieldName = "合同交底ID", DataType = "INT", IsPrimaryKey = true, IsNullable = false, IsIdentity = true, IsAvailable = true, FieldDescription = "主键" },
    new TableField { ColumnName = "ContractId", FieldName = "合同ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联Contract表" },
    new TableField { ColumnName = "ContractCode", FieldName = "合同编号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "HandoverCode", FieldName = "交底单号", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "唯一标识" },
    new TableField { ColumnName = "HandoverType", FieldName = "交底类型", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-首次交底，2-变更交底" },
    new TableField { ColumnName = "HandoverDate", FieldName = "交底日期", DataType = "DATETIME", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "HandoverLocation", FieldName = "交底地点", DataType = "NVARCHAR(200)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "SalesRepresentativeId", FieldName = "销售代表ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联User表" },
    new TableField { ColumnName = "SalesRepresentativeName", FieldName = "销售代表姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "DeliveryTeamLeaderId", FieldName = "交付团队负责人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "外键关联User表" },
    new TableField { ColumnName = "DeliveryTeamLeaderName", FieldName = "交付团队负责人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Participants", FieldName = "参与人员", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "可存储多个人员信息" },
    new TableField { ColumnName = "KeyContractTerms", FieldName = "关键合同条款", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CustomerSpecialRequirements", FieldName = "客户特殊要求", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "TechnicalRequirements", FieldName = "技术要求", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "DeliveryRequirements", FieldName = "交付要求", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "QualityRequirements", FieldName = "质量要求", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "PaymentTerms", FieldName = "付款条款", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "RiskPoints", FieldName = "风险点", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "RiskMitigationMeasures", FieldName = "风险应对措施", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ImportantNotes", FieldName = "重要注意事项", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Attachments", FieldName = "附件路径", DataType = "NVARCHAR(MAX)", IsNullable = true, IsAvailable = true, FieldDescription = "JSON格式存储多个附件信息" },
    new TableField { ColumnName = "HandoverStatus", FieldName = "交底状态", DataType = "INT", IsNullable = false, DefaultValue = "1", IsAvailable = true, FieldDescription = "1-已安排，2-进行中，3-已完成，4-已取消" },
    new TableField { ColumnName = "CompletionTime", FieldName = "完成时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "SalesSignOff", FieldName = "销售签字确认", DataType = "BIT", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "SalesSignOffTime", FieldName = "销售签字时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "DeliverySignOff", FieldName = "交付签字确认", DataType = "BIT", IsNullable = true, DefaultValue = "0", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "DeliverySignOffTime", FieldName = "交付签字时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "HandoverEvaluation", FieldName = "交底评价", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "FollowUpActions", FieldName = "后续行动", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "Remark", FieldName = "备注", DataType = "NVARCHAR(500)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateTime", FieldName = "创建时间", DataType = "DATETIME", IsNullable = false, DefaultValue = "GETDATE()", IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserId", FieldName = "创建人ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateUserName", FieldName = "创建人姓名", DataType = "NVARCHAR(50)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationId", FieldName = "创建人所在组织ID", DataType = "INT", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "CreateOrganizationName", FieldName = "创建人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = false, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyTime", FieldName = "修改时间", DataType = "DATETIME", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserId", FieldName = "修改人ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyUserName", FieldName = "修改人姓名", DataType = "NVARCHAR(50)", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationId", FieldName = "修改人所在组织ID", DataType = "INT", IsNullable = true, IsAvailable = true, FieldDescription = "" },
    new TableField { ColumnName = "ModifyOrganizationName", FieldName = "修改人所在组织名称", DataType = "NVARCHAR(100)", IsNullable = true, IsAvailable = true, FieldDescription = "" }
};
            table.TableFields = fields;
            table.FieldCount = fields.Count;

            return table;
        }
        #endregion

        public List<TableRelationship> GenerateRelationships(Dictionary<string, int> tableNameToIdMap)
        {
            var relationships = new List<TableRelationship>();

            //按照业务流程维护关系，到谁谁维护

            // 1. Department -> Organization
            if (tableNameToIdMap.ContainsKey("Department") && tableNameToIdMap.ContainsKey("Organization"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["Department"],
                    TargetTableId = tableNameToIdMap["Organization"],
                    RelationshipType = TableRelationshipType.ManyToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("OrganizationId","OrganizationId")
                    }
                });
            }

            //  UserInfo -> Department
            if (tableNameToIdMap.ContainsKey("UserInfo") && tableNameToIdMap.ContainsKey("Department"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["UserInfo"],
                    TargetTableId = tableNameToIdMap["Department"],
                    RelationshipType = TableRelationshipType.ManyToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("DepartmentId","DepartmentId")
                    }
                });
            }

            //  UserInfo -> Organization
            if (tableNameToIdMap.ContainsKey("UserInfo") && tableNameToIdMap.ContainsKey("Organization"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["UserInfo"],
                    TargetTableId = tableNameToIdMap["Organization"],
                    RelationshipType = TableRelationshipType.ManyToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("OrganizationId","OrganizationId")
                    }
                });
            }


            #region Customer
            // Customer -> UserInfo
            if (tableNameToIdMap.ContainsKey("Customer") && tableNameToIdMap.ContainsKey("UserInfo"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["Customer"],
                    TargetTableId = tableNameToIdMap["UserInfo"],
                    RelationshipType = TableRelationshipType.ManyToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("OwnerUserId","CustomerId")
                    }
                });
            }
           
            // Customer -> Organization
            if (tableNameToIdMap.ContainsKey("Customer") && tableNameToIdMap.ContainsKey("Organization"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["Customer"],
                    TargetTableId = tableNameToIdMap["Organization"],
                    RelationshipType = TableRelationshipType.ManyToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("CreateOrganizationId","OrganizationId")
                    }
                });
            }
            #endregion

            //  Opportunity -> Customer
            if (tableNameToIdMap.ContainsKey("Opportunity") && tableNameToIdMap.ContainsKey("Customer"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["Opportunity"],
                    TargetTableId = tableNameToIdMap["Customer"],
                    RelationshipType = TableRelationshipType.ManyToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("CustomerId","CustomerId")
                    }
                });
            }

            // Quotation -> Opportunity
            if (tableNameToIdMap.ContainsKey("Quotation") && tableNameToIdMap.ContainsKey("Opportunity"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["Quotation"],
                    TargetTableId = tableNameToIdMap["Opportunity"],
                    RelationshipType = TableRelationshipType.ManyToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("OpportunityId","OpportunityId")
                    }
                });
            }
 
            // Bid -> Opportunity
            if (tableNameToIdMap.ContainsKey("Bid") && tableNameToIdMap.ContainsKey("Opportunity"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["Bid"],
                    TargetTableId = tableNameToIdMap["Opportunity"],
                    RelationshipType = TableRelationshipType.ManyToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("OpportunityId","OpportunityId")
                    }
                });
            }

            #region 销售立项表
            // ProjectApproval -> Quotation
            if (tableNameToIdMap.ContainsKey("ProjectApproval") && tableNameToIdMap.ContainsKey("Quotation"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["ProjectApproval"],
                    TargetTableId = tableNameToIdMap["Opportunity"],
                    RelationshipType = TableRelationshipType.OneToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("ReferenceId","ReferenceId"),
                        TableRelationshipCondition.SourceConstant("ReferenceType","1"),
                    }
                });
            }
            // ProjectApproval -> Bid
            if (tableNameToIdMap.ContainsKey("ProjectApproval") && tableNameToIdMap.ContainsKey("Bid"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["ProjectApproval"],
                    TargetTableId = tableNameToIdMap["Bid"],
                    RelationshipType = TableRelationshipType.OneToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("ReferenceId","ReferenceId"),
                        TableRelationshipCondition.SourceConstant("ReferenceType","2"),
                    }
                });
            }
            // ProjectApproval -> CustomerId
            if (tableNameToIdMap.ContainsKey("ProjectApproval") && tableNameToIdMap.ContainsKey("Customer"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["ProjectApproval"],
                    TargetTableId = tableNameToIdMap["Customer"],
                    RelationshipType = TableRelationshipType.ManyToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("CustomerId","CustomerId"),
                    }
                });
            }
            #endregion

            if (tableNameToIdMap.ContainsKey("Contract") && tableNameToIdMap.ContainsKey("ProjectApproval"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["Contract"],
                    TargetTableId = tableNameToIdMap["ProjectApproval"],
                    RelationshipType = TableRelationshipType.OneToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("ProjectApprovalId","ProjectApprovalId"),
                    }
                });
            }
 
            // ContractDetail -> Contract
            if (tableNameToIdMap.ContainsKey("ContractDetail") && tableNameToIdMap.ContainsKey("Contract"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["ContractDetail"],
                    TargetTableId = tableNameToIdMap["Contract"],
                    RelationshipType = TableRelationshipType.ManyToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("ContractId","ContractId"),
                    }
                });
            }

            // ContractDetail -> Product
            if (tableNameToIdMap.ContainsKey("ContractDetail") && tableNameToIdMap.ContainsKey("Product"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["ContractDetail"],
                    TargetTableId = tableNameToIdMap["Product"],
                    RelationshipType = TableRelationshipType.ManyToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("ProductId","ProductId"),
                    }
                });
            }

            // ContractChange -> Contract
            if (tableNameToIdMap.ContainsKey("ContractChange") && tableNameToIdMap.ContainsKey("Contract"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["ContractChange"],
                    TargetTableId = tableNameToIdMap["Contract"],
                    RelationshipType = TableRelationshipType.OneToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("ContractId","ContractId"),
                    }
                });
            }

            // ContractHandover -> Contract
            if (tableNameToIdMap.ContainsKey("ContractHandover") && tableNameToIdMap.ContainsKey("Contract"))
            {
                relationships.Add(new TableRelationship
                {
                    SourceTableId = tableNameToIdMap["ContractHandover"],
                    TargetTableId = tableNameToIdMap["Contract"],
                    RelationshipType = TableRelationshipType.OneToOne,
                    Conditions = new List<TableRelationshipCondition>() {
                        TableRelationshipCondition.Key("ContractId","ContractId"),
                    }
                });
            }
            return relationships;
        }

        public List<BusinessObjective> GenerateBusinessObjective(Dictionary<string, int> tableNameToIdMap, List<TableStructure> tableStructures)
        {
            var list = new List<BusinessObjective>();

            if (tableNameToIdMap.ContainsKey("Organization"))
            {
                var table = tableStructures.Find(x => x.TableName == "Organization")!;
                var dimension1 = new string[] { "OrganizationCode", "OrganizationName" };
                var dimension2 = new string[] { "CreateTime" };
                list.Add(new BusinessObjective
                {
                    BusinessName = "组织架构",
                    DependencyTables = new List<BusinessObjectiveDependencyTable> {
                        new () { TableId = table.TableId }
                    },
                    Fields = table.TableFields.Select(x =>
                    {
                        var result = new BusinessObjectiveField
                        {
                            FieldId = x.FieldId,
                            Name = x.FieldName,
                            Description = x.FieldDescription,
                            BusinesBIRole = (x.IsPrimaryKey || dimension1.Contains(x.ColumnName)) ? BusinessObjectiveFieldBusinesBIRole.Demension : BusinessObjectiveFieldBusinesBIRole.Attribute,
                        };

                        if (x.IsPrimaryKey)
                        {
                            result.DimensionLayer = BusinessObjectiveFieldDimensionLayer.实体;
                        }

                        if (dimension1.Contains(x.ColumnName))
                        {
                            result.DimensionLayer = BusinessObjectiveFieldDimensionLayer.组织;
                        }

                        if (dimension2.Contains(x.ColumnName))
                        {
                            result.DimensionLayer = BusinessObjectiveFieldDimensionLayer.年 | BusinessObjectiveFieldDimensionLayer.月 | BusinessObjectiveFieldDimensionLayer.日;
                        }

                        return result;
                    }).ToList()
                });
            }

            if (tableNameToIdMap.ContainsKey("Department"))
            {
                var table = tableStructures.Find(x => x.TableName == "Department")!;
                list.Add(new BusinessObjective
                {
                    BusinessName = "部门管理",
                    DependencyTables = new List<BusinessObjectiveDependencyTable> {
                         new ()
                         {
                           TableId = table.TableId
                         }
                    },
                    Fields = table.TableFields.Select(x => new BusinessObjectiveField
                    {
                        FieldId = x.FieldId,
                        Name = x.FieldName,
                        Description = x.FieldDescription,
                    }).ToList()
                });
            }

            if (tableNameToIdMap.ContainsKey("UserInfo"))
            {
                var table = tableStructures.Find(x => x.TableName == "UserInfo")!;
                list.Add(new BusinessObjective
                {
                    BusinessName = "用户管理",
                    DependencyTables = new List<BusinessObjectiveDependencyTable> {
                         new ()
                         {
                           TableId = table.TableId
                         }
                    },
                    Fields = table.TableFields.Select(x => new BusinessObjectiveField
                    {
                        FieldId = x.FieldId,
                        Name = x.FieldName,
                        Description = x.FieldDescription,
                    }).ToList()
                });
            }

            if (tableNameToIdMap.ContainsKey("Customer"))
            {
                var table = tableStructures.Find(x => x.TableName == "Customer")!;
                list.Add(new BusinessObjective
                {
                    BusinessName = "客户管理",
                    Description = "维护客户基础资料",
                    DependencyTables = new List<BusinessObjectiveDependencyTable> {
                         new ()
                         {
                           TableId = table.TableId
                         }
                    },
                    Fields = table.TableFields.Select(x => new BusinessObjectiveField
                    {
                        FieldId = x.FieldId,
                        Name = x.FieldName,
                        Description = x.FieldDescription,
                    }).ToList()
                });
            }

            if (tableNameToIdMap.ContainsKey("Opportunity"))
            {
                var table = tableStructures.Find(x => x.TableName == "Opportunity")!;
                list.Add(new BusinessObjective
                {
                    BusinessName = "商机登记",
                    Description = "管理客户商机信息，包括增删改查等操作",
                    DependencyTables = new List<BusinessObjectiveDependencyTable> {
                         new ()
                         {
                           TableId = table.TableId
                         }
                    },
                    Fields = table.TableFields.Select(x => new BusinessObjectiveField
                    {
                        FieldId = x.FieldId,
                        Name = x.FieldName,
                        Description = x.FieldDescription,
                    }).ToList()
                });
            }

            if (tableNameToIdMap.ContainsKey("Quotation"))
            {
                var table = tableStructures.Find(x => x.TableName == "Quotation")!;
                list.Add(new BusinessObjective
                {
                    BusinessName = "销售报价",
                    Description = "对商机的报价信息",
                    DependencyTables = new List<BusinessObjectiveDependencyTable> {
                         new ()
                         {
                           TableId = table.TableId
                         }
                    },
                    Fields = table.TableFields.Select(x => new BusinessObjectiveField
                    {
                        FieldId = x.FieldId,
                        Name = x.FieldName,
                        Description = x.FieldDescription,
                    }).ToList()
                });
            }

            if (tableNameToIdMap.ContainsKey("Bid"))
            {
                var table = tableStructures.Find(x => x.TableName == "Bid")!;
                list.Add(new BusinessObjective
                {
                    BusinessName = "销售投标",
                    Description = "对商机的投标信息维护",
                    DependencyTables = new List<BusinessObjectiveDependencyTable> {
                         new ()
                         {
                           TableId = table.TableId
                         }
                    },
                    Fields = table.TableFields.Select(x => new BusinessObjectiveField
                    {
                        FieldId = x.FieldId,
                        Name = x.FieldName,
                        Description = x.FieldDescription,
                    }).ToList()
                });
            }

            if (tableNameToIdMap.ContainsKey("ProjectApproval"))
            {
                var table = tableStructures.Find(x => x.TableName == "ProjectApproval")!;
                list.Add(new BusinessObjective
                {
                    BusinessName = "销售立项",
                    Description = "报价或投标成功后可以进行立项",
                    DependencyTables = new List<BusinessObjectiveDependencyTable> {
                         new ()
                         {
                           TableId = table.TableId
                         }
                    },
                    Fields = table.TableFields.Select(x => new BusinessObjectiveField
                    {
                        FieldId = x.FieldId,
                        Name = x.FieldName,
                        Description = x.FieldDescription,
                    }).ToList()
                });
            }

            if (tableNameToIdMap.ContainsKey("Contract"))
            {
                var table = tableStructures.Find(x => x.TableName == "Contract")!;
                var table2 = tableStructures.Find(x => x.TableName == "ContractDetail")!;
                list.Add(new BusinessObjective
                {
                    BusinessName = "销售合同",
                    Description = "与客户签订的合同信息，包含合同基础信息、合同附件、合同明细",
                    DependencyTables = new List<BusinessObjectiveDependencyTable> {
                         new ()
                         {
                           TableId = table.TableId
                         },
                          new ()
                         {
                           TableId = table2.TableId
                         },
                    },
                    Fields = table.TableFields.Select(x => new BusinessObjectiveField
                    {
                        FieldId = x.FieldId,
                        Name = x.FieldName,
                        Description = x.FieldDescription,
                    }).ToList()
                });
            }

            if (tableNameToIdMap.ContainsKey("ContractChange"))
            {
                var table = tableStructures.Find(x => x.TableName == "ContractChange")!;
                list.Add(new BusinessObjective
                {
                    BusinessName = "合同变更",
                    Description = "管理合同签订后合同发生的变更",
                    DependencyTables = new List<BusinessObjectiveDependencyTable> {
                         new ()
                         {
                           TableId = table.TableId
                         }
                    },
                    Fields = table.TableFields.Select(x => new BusinessObjectiveField
                    {
                        FieldId = x.FieldId,
                        Name = x.FieldName,
                        Description = x.FieldDescription,
                    }).ToList()
                });
            }

            if (tableNameToIdMap.ContainsKey("ContractHandover"))
            {
                var table = tableStructures.Find(x => x.TableName == "ContractHandover")!;
                list.Add(new BusinessObjective
                {
                    BusinessName = "合同交底",
                    Description = "合同再无任何变动后双方达成的执行共识",
                    DependencyTables = new List<BusinessObjectiveDependencyTable> {
                         new ()
                         {
                           TableId = table.TableId
                         }
                    },
                    Fields = table.TableFields.Select(x => new BusinessObjectiveField
                    {
                        FieldId = x.FieldId,
                        Name = x.FieldName,
                        Description = x.FieldDescription,
                    }).ToList()
                });
            }

            //list.Add(new BusinessObjective
            //{
            //    BusinessName = "销售管理",
            //    Description = "销售业务负责的工作领域",
            //    DependencyTables = new List<BusinessObjectiveDependencyTable> {
            //          new () { TableId = tableNameToIdMap["Organization"]  },
            //          new () { TableId = tableNameToIdMap["Department"]  },
            //          new () { TableId = tableNameToIdMap["UserInfo"]  },
            //          new () { TableId = tableNameToIdMap["Product"]  },
            //          new () { TableId = tableNameToIdMap["Customer"]  },
            //          new () { TableId = tableNameToIdMap["Opportunity"]  },
            //          new () { TableId = tableNameToIdMap["Quotation"]  },
            //          new () { TableId = tableNameToIdMap["Bid"]  },
            //          new () { TableId = tableNameToIdMap["ProjectApproval"]  },
            //          new () { TableId = tableNameToIdMap["Contract"]  },
            //          new () { TableId = tableNameToIdMap["ContractDetail"]  },
            //          new () { TableId = tableNameToIdMap["ContractChange"]  },
            //          new () { TableId = tableNameToIdMap["ContractHandover"]  },
            //    }
            //});

            return list;
        }
    }
}