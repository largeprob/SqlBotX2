using SqlBoTx.Net.Domain.DomainEntities;
using SqlBoTx.Net.Domain.Share.Enums.DomainEntity;
using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.BusinessEntities
{
    /// <summary>
    /// 实体属性
    /// </summary>
    [Description("实体属性")]
    public class DomainEntityAttr
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        [Description("主键自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 实体ID
        /// </summary>
        [Description("实体ID")]
        public string EntityId { get; set; }

        /// <summary>
        /// 实体
        /// </summary>
        [Description("实体")]
        public virtual DomainEntity? Entity { get; set; }

        /// <summary>
        /// 引用库字段ID
        /// </summary>
        [Description("引用库字段ID")]
        public int ColumnId { get; set; }

        /// <summary>
        /// 引用字段
        /// </summary>
        [Description("引用字段")]
        public virtual TableStructureColumn? Column { get; set; }

        /// <summary>
        /// 属性列名
        /// </summary>
        [Description("属性列名")]
        public string? ColumnName { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        [Description("属性名称")]
        public string? Label { get; set; }

        /// <summary>
        /// 是否必要
        /// </summary>
        [Description("是否必要")]
        public bool? IsRequired { get; set; } 

        /// <summary>
        /// 数据类型
        /// <see cref="DataType"/>
        /// </summary>
        [Description("数据类型")]
        public DataType? DataType { get; set; } 

        /// <summary>
        /// 数据类型概要（用来描述取值）
        /// </summary>
        [Description("数据类型概要（用来描述取值）")]
        public string? DataTypeSchema { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        [Description("默认值")]
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 字段说明（字段额外的解释）
        /// </summary>
        [Description("字段说明（字段额外的解释）")]
        public string? Description { get; set; }

        /// <summary>
        /// 结构语义
        /// </summary>
        [Description("结构语义")]
        public StructureRole? StructureRole { get; set; }

        /// <summary>
        /// 外键元数据
        /// </summary>
        [Description("外键元数据")]
        public virtual ForeignKeyMetaData ForeignKeyMetaData { get; set; } = null!;

        /// <summary>
        /// 语义类型
        /// </summary>
        [Description("语义类型")]
        public SemanticType? SemanticType { get; set; }

        /// <summary>
        /// 维度分类（SemanticType = Dimension 时有效）
        /// </summary>
        [Description("维度分类（SemanticType = Dimension 时有效）")]
        public DimensionCategory? DimensionCategory { get; set; }

        /// <summary>
        /// 时间粒度（SemanticType = DateTime 时有效）
        /// </summary>
        [Description("时间粒度（SemanticType = DateTime 时有效）")]
        public TimeDimensionGranularity? TimeGranularity { get; set; }

        /// <summary>
        /// 仅当 SemanticType = Measure 时有效。可使用的度量函数
        /// </summary>
        [Description("仅当 SemanticType = Measure 时有效。可使用的度量函数")]
        public MeasureAggregation? SupportedAggregations { get; set; }
    }

   
}
