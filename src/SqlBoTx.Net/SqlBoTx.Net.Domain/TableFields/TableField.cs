
using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Text;

namespace SqlBoTx.Net.Domain.TableFields
{
    /// <summary>
    /// 表字段明细信息
    /// </summary>
    [Description("表字段明细信息")]
    public class TableField
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Description("主键自增ID")]
        public int FieldId { get; set; }

        /// <summary>
        /// 外键
        /// </summary>
        [Description("外键")]
        public int TableId { get; set; }

        /// <summary>
        /// 字段名称（数据库中实际字段名）
        /// </summary>
        [Description("字段名称")]
        public string? ColumnName { get; set; }

        /// <summary>
        /// 字段数据类型（如：int, varchar(200),Decimal(18,2), datetime等）
        /// </summary>
        [Description("字段数据类型")]
        public string? DataType { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        [Description("是否为主键")]
        public bool IsPrimaryKey { get; set; } = false;

        /// <summary>
        /// 是否允许为空
        /// </summary>
        [Description("是否允许为空")]
        public bool IsNullable { get; set; } = true;

        /// <summary>
        /// 是否为自增字段
        /// </summary>
        [Description("是否为自增字段")]
        public bool IsIdentity { get; set; } = false;

        /// <summary>
        /// 默认值
        /// </summary>
        [Description("默认值")]
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 字段中文名称
        /// </summary>
        [Description("字段中文名称")]
        public string? FieldName { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        [Description("字段说明")]
        public string? FieldDescription { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [Description("是否可用")]
        public bool? IsAvailable { get; set; }
    }
}
