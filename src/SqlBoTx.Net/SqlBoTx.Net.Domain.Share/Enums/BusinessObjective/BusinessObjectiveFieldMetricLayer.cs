using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.BusinessObjective
{
    /// <summary>
    /// 业务指标字段标签
    /// </summary>
    [Flags]
    public enum BusinessObjectiveFieldMetricLayer
    {
        /// <summary>
        /// 返回指定数值列的总和
        /// </summary>
        [Description("求和")]
        Sum = 1,

        /// <summary>
        /// 返回指定数值列的平均值
        /// </summary>
        [Description("平均值")]
        Avg,

        /// <summary>
        /// 返回组中的项数（int 类型）
        /// </summary>
        [Description("计数")]
        Count,

        /// <summary>
        /// 返回组中的项数（bigint 类型）
        /// </summary>
        [Description("计数(大整数)")]
        CountBig,

        /// <summary>
        /// 返回表达式中的最大值
        /// </summary>
        [Description("最大值")]
        Max,

        /// <summary>
        /// 返回表达式中的最小值
        /// </summary>
        [Description("最小值")]
        Min,

        /// <summary>
        /// 返回给定表达式中所有值的统计标准偏差
        /// </summary>
        [Description("标准偏差")]
        Stdev,

        /// <summary>
        /// 返回给定表达式中所有值的总体标准偏差
        /// </summary>
        [Description("总体标准偏差")]
        Stdevp,

        /// <summary>
        /// 返回给定表达式中所有值的统计方差
        /// </summary>
        [Description("方差")]
        Var,

        /// <summary>
        /// 返回给定表达式中所有值的总体统计方差
        /// </summary>
        [Description("总体方差")]
        Varp,

        /// <summary>
        /// 返回组中各值的校验和，用于检测表中的更改
        /// </summary>
        [Description("校验和聚合")]
        ChecksumAgg,

        /// <summary>
        /// 返回对表中的行或表达式列表计算的二进制校验值
        /// </summary>
        [Description("二进制校验和")]
        BinaryChecksum,

        /// <summary>
        /// 当行由 CUBE 或 ROLLUP 添加时返回 1，否则返回 0
        /// </summary>
        [Description("分组标识")]
        Grouping,

        /// <summary>
        /// 计算分组级别，标识聚合层次
        /// </summary>
        [Description("分组ID")]
        GroupingId,

        /// <summary>
        /// 返回组中非重复值的近似数目，适用于大数据场景
        /// </summary>
        [Description("近似计数")]
        ApproxCountDistinct
    }
}
