using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.DomainEntity
{
    /// <summary>
    /// 问数语义类型
    /// </summary>
    public enum SemanticType
    {
        /// <summary>
        /// 维度，可作为分组和过滤依据
        /// </summary>
        [Description("维度")]
        Dimension = 1,

        /// <summary>
        /// 时间，支持时间过滤、粒度聚合、时间智能函数
        /// </summary>
        [Description("时间")]
        Date,

        /// <summary>
        /// 度量，可被聚合计算
        /// </summary>
        [Description("度量")]
        Measure,

        /// <summary>
        /// 属性，描述性字段，不参与分组和聚合
        /// </summary>
        [Description("属性")]
        Attribute,
    }
}
