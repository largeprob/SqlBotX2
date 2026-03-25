using SqlBoTx.Net.Domain.Share.Enums;
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
        [Description("数据库连接ID")]
        public int ConnectionId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [Description("表名")]
        public string? TableName { get; set; }

        /// <summary>
        /// 模式
        /// </summary>
        [Description("模式")]
        public string? SchemaName { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [Description("别名")]
        public string? Alias { get; set; }

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
        /// 导航属性 - 表字段明细
        /// </summary>
        public virtual ICollection<TableStructureColumn> Columns { get; set; } = new List<TableStructureColumn>();

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
