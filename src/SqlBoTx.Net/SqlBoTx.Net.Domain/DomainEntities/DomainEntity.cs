using JasperFx.CommandLine;
using SqlBoTx.Net.Domain.BusinessObjectives;
using SqlBoTx.Net.Domain.DatabaseConnections;
using SqlBoTx.Net.Domain.Share.Enums;
using SqlBoTx.Net.Domain.TableRelationships;
using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.Domain.BusinessEntities
{
    /// <summary>
    /// 实体管理
    /// </summary>
    [Description("实体管理")]
    public class DomainEntity
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        [Description("主键自增ID")]
        public string Id { get; set; }

        /// <summary>
        /// 所属业务域ID
        /// </summary>
        [Description("所属业务域ID")]
        public int DomainId { get; set; }

        /// <summary>
        /// 业务域
        /// </summary>
        [Description("业务域")]
        public virtual BusinessObjective? Domain { get; set; }

        /// <summary>
        /// 引用数据库
        /// </summary>
        [Description("引用数据库")]
        public int ReferenceConnectId { get; set; }

        /// <summary>
        /// 引用数据库
        /// </summary>
        [Description("引用数据库")]
        public virtual DatabaseConnection? Database { get; set; }

        /// <summary>
        /// 引用表
        /// </summary>
        [Description("引用表")]
        public int ReferenceTableId { get; set; }

        /// <summary>
        /// 引用数据库
        /// </summary>
        [Description("引用数据库")]
        public virtual TableStructure? Table { get; set; }

        /// <summary>
        /// 实体名
        /// </summary>
        [Description("实体名")]
        public string? Name { get; set; }

        /// <summary>
        /// 实体别名
        /// </summary>
        [Description("实体别名")]
        public string? Alias { get; set; }

        /// <summary>
        /// 解释说明
        /// </summary>
        [Description("解释说明")]
        public string? Description { get; set; }

        /// <summary>
        /// 实体标签，在业务域中：主实体、明细实体、副作用实体、关联实体
        /// </summary>
        [Description("实体标签，在业务域中：核心实体、明细实体、副作用实体、关联实体")]
        public string[]? Tags { get; set; }


        /// <summary>
        /// 导航属性 - 属性明细
        /// </summary>
        public virtual ICollection<DomainEntityAttr> Attrs { get; set; } = null!;

        /// <summary>
        /// 导航属性 - 作为源表的关系列表
        /// </summary>
        public virtual ICollection<DomainEntityRel> SourceRels { get; set; } = null!;

        /// <summary>
        /// 导航属性 - 作为目标表的关系列表
        /// </summary>
        public virtual ICollection<DomainEntityRel> TargetRels { get; set; } = null!;
    }
}
