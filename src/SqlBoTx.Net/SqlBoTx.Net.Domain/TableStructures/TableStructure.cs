using SqlBoTx.Net.Domain.Share.Enums;
using SqlBoTx.Net.Domain.TableFields;
using SqlBoTx.Net.Domain.TableRelationships;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Tracing;
using System.Text;

namespace SqlBoTx.Net.Domain.TableStructures
{
    /// <summary>
    /// 数据库表结构
    /// </summary>
    [Description("数据库表结构")]
    public class TableStructure
    {
        /// <summary>
        /// 表ID（主键）
        /// </summary>
        [Description("主键自增ID")]
        public int TableId { get; set; }

        /// <summary>
        /// 数据库连接ID（外键）
        /// </summary>
        [Description("外键")]
        public int ConnectionId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [Description("表名")]
        public string? TableName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [Description("显示名称")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// 字段数量
        /// </summary>
        [Description("字段数量")]
        public int FieldCount { get; set; }

        /// <summary>
        /// 表描述
        /// </summary>
        [Description("表描述")]
        public string? Description { get; set; }

        /// <summary>
        /// 颗粒度描述
        /// </summary>
        [Description("颗粒度描述")]
        public string? Granularity { get; set; }

        /// <summary>
        /// 颗粒度级别
        /// </summary>
        [Description("颗粒度级别")]
        public TableStructureGranularityLevel? GranularityLevel { get; set; }

        /// <summary>
        /// 导航属性 - 表字段明细
        /// </summary>
        public virtual ICollection<TableField> TableFields { get; set; } = new List<TableField>();

        /// <summary>
        /// 导航属性 - 作为源表的关系列表
        /// </summary>
        public virtual ICollection<TableRelationship> SourceTableRelationships { get; set; } = new List<TableRelationship>();

        /// <summary>
        /// 导航属性 - 作为目标表的关系列表
        /// </summary>
        public virtual ICollection<TableRelationship> TargetTableRelationships { get; set; } = new List<TableRelationship>();
    }
}
