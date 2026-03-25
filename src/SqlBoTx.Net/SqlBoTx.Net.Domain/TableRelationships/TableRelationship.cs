using SqlBoTx.Net.Domain.Share.Enums;
using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using SqlBoTx.Net.Domain.Share.TableRelationships;

namespace SqlBoTx.Net.Domain.TableRelationships
{
    /// <summary>
    /// 表关系维护实体
    /// </summary>
    [Description("表关系维护")]
    public class TableRelationship
    {
        /// <summary>
        /// 关系ID（主键）
        /// </summary>
        [Description("主键自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 源表ID（外键，指向TableStructure）
        /// </summary>
        [Description("源表ID")]
        public int SourceTableId { get; set; }

        /// <summary>
        /// 导航属性 - 源表
        /// </summary>
        [ForeignKey("SourceTableId")]
        public virtual TableStructure? SourceTable { get; set; }

        /// <summary>
        /// 目标表ID（外键，指向TableStructure）
        /// </summary>
        [Description("目标表ID")]
        public int TargetTableId { get; set; }

        /// <summary>
        /// 导航属性 - 目标表
        /// </summary>
        [ForeignKey("TargetTableId")]
        public virtual TableStructure? TargetTable { get; set; }

        /// <summary>
        /// 源端基数（One/Many）
        /// </summary>
        [Description("源端基数（One/Many）")]
        public TableRelationshipType SourceCardinality { get; set; }

        /// <summary>
        /// 目标端基数（One/Many）
        /// </summary>
        [Description("目标端基数（One/Many）")]
        public TableRelationshipType TargetCardinality { get; set; }

        /// <summary>
        /// 关联条件（JSON格式存储）
        /// 示例：[{"SourceColumn":"Id","TargetColumn":"UserId"},{"SourceColumn":"TenantId","TargetColumn":"TenantId"}]
        /// </summary>
        [Description("关联条件")]
        public List<TableRelationshipCondition>? Conditions { get; set; }

        /// <summary>
        /// 说明这段关系的作用
        /// </summary>
        [Description("关系类型")]
        public string? RelationshipDescription { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        [Description("更新时间")]
        public DateTime? UpdatedAt { get; set; }

    }
}
