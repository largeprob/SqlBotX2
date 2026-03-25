using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Text;

namespace SqlBoTx.Net.Domain.TableStructures
{
    /// <summary>
    /// 表字段明细信息
    /// </summary>
    [Description("表字段明细信息")]
    public class TableStructureColumn
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
        /// 外键表
        /// </summary>
        [Description("外键表")]
        public virtual TableStructure Table { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Description("标题")]
        public string? Label { get; set; }

        /// <summary>
        /// 字段名称（数据库中实际字段名）
        /// </summary>
        [Description("字段名称")]
        public string? ColumnName { get; set; }

        /// <summary>
        /// 字段数据类型（如：int, varchar,Decimal, datetime等）
        /// </summary>
        [Description("字段数据类型")]
        public string? DataType { get; set; }

        /// <summary>
        /// 类型描述
        /// </summary>
        [Description("类型描述")]
        public string? DataTypeSchema { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        [Description("默认值")]
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        [Description("字段说明")]
        public string? Description { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        [Description("是否为主键")]
        public bool IsPrimaryKey { get; set; } = false;

        /// <summary>
        /// 是否为自增字段
        /// </summary>
        [Description("是否为自增字段")]
        public bool IsIdentity { get; set; } = false;

        /// <summary>
        /// 是否必填
        /// </summary>
        [Description("是否必填")]
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// 是否唯一
        /// </summary>
        [Description("是否唯一")]
        public bool IsUnique { get; set; } = false;

        /// <summary>
        /// 外键引用
        /// </summary>
        [Description("外键引用")]
        public bool IsReference { get; set; } = false;

        /// <summary>
        /// 引用表名
        /// </summary>
        [Description("引用表名")]
        public string? ReferenceTableName { get; set; }

        /// <summary>
        /// 引用字段
        /// </summary>
        [Description("引用字段")]
        public string? ReferenceColumn { get; set; }

        /// <summary>
        /// 是否计算列
        /// </summary>
        [Description("是否计算列")]
        public bool IsComputed { get; set; } = false;

        /// <summary>
        /// 表达式
        /// </summary>
        [Description("表达式")]
        public string? Expression { get; set; }

        /// <summary>
        /// 是否索引
        /// </summary>
        [Description("是否索引")]
        public bool IsIndex { get; set; } = false;

        /// <summary>
        /// 索引
        /// </summary>
        [Description("索引")]
        public string[]? Indexs { get; set; } 
    }
}
