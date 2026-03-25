using Microsoft.EntityFrameworkCore;
using SqlBoTx.Net.Domain.Share.Enums.DomainEntity;
using SqlBoTx.Net.Domain.Share.TableRelationships;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.BusinessEntities
{
    /// <summary>
    /// 实体关系
    /// </summary>
    [Description("实体关系")]
    public class DomainEntityRel
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        [Description("主键自增ID")]
        public string? Id { get; set; }

        /// <summary>
        /// 关联类型
        /// </summary>
        [Description("关联类型")]
        public DomainEntityRelType Type { get; set; }

        /// <summary>
        /// 源实体标识符
        /// </summary>
        [Description("源实体标识符")]
        public string SourceEntityId { get; set; }

        /// <summary>
        /// 源实体
        /// </summary>
        [Description("源实体")]
        public virtual DomainEntity? SourceEntity { get; set; }

        /// <summary>
        /// 目标实体标识符
        /// </summary>
        [Description("目标实体标识符")]
        public string TargetEntityId { get; set; }

        /// <summary>
        /// 目标实体
        /// </summary>
        [Description("目标实体")]
        public virtual DomainEntity? TargetEntity { get; set; }

        /// <summary>
        /// 源角色名称
        /// </summary>
        [Description("源角色名称")]
        public string? SourceRole { get; set; }

        /// <summary>
        /// 目标角色名称
        /// </summary>
        [Description("目标角色名称")]
        public string? TargetRole { get; set; }

        /// <summary>
        /// 源端基数（One/Many）
        /// </summary>
        [Description("源端基数（One/Many）")]
        public string? SourceCardinality { get; set; }

        /// <summary>
        /// 目标端基数（One/Many）
        /// </summary>
        [Description("目标端基数（One/Many）")]
        public string? TargetCardinality { get; set; }

        /// <summary>
        /// 反向关系Id
        /// </summary>
        [Description("反向关系Id")]
        public string? InverseOf { get; set; }

        // "删除 Target 时，Source 上的外键列怎么处理"。
        /// <summary>
        /// 是否级联删除
        /// </summary>
        [Description("是否级联删除")]
        public OntologyDeleteBehavior CascadeDelete { get; set; }

        /// <summary>
        /// 关联条件（JSON格式存储）
        /// 示例：[{"SourceColumn":"Id","TargetColumn":"UserId"},{"SourceColumn":"TenantId","TargetColumn":"TenantId"}]
        /// </summary>
        [Description("关联条件")]
        public List<DomainEntityRelJoin>? JoinConditions { get; set; }
    }
}
