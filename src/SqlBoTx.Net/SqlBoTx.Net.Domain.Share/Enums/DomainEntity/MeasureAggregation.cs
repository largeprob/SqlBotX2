using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.DomainEntity
{
    /// <summary>
    /// 业务指标字段标签
    /// </summary>
    [Flags]
    public enum MeasureAggregation
    {
        None = 0,

        // 基础聚合（通用）
        [Description("求和")]
        Sum = 1 << 0,   // 1

        [Description("平均值")]
        Avg = 1 << 1,   // 2

        [Description("计数")]
        Count = 1 << 2,   // 4

        [Description("去重计数")]
        CountDistinct = 1 << 3,   // 8

        [Description("最大值")]
        Max = 1 << 4,   // 16

        [Description("最小值")]
        Min = 1 << 5,   // 32

        // 统计聚合（专业场景）
        [Description("标准偏差")]
        Stdev = 1 << 6,   // 64

        [Description("总体标准偏差")]
        Stdevp = 1 << 7,   // 128

        [Description("方差")]
        Var = 1 << 8,   // 256

        [Description("总体方差")]
        Varp = 1 << 9,   // 512

        [Description("近似计数")]
        ApproxCountDistinct = 1 << 10, // 1024

        /// <summary>基础统计，大多数度量字段的默认值</summary>
        Basic = Sum | Avg | Count | Max | Min,

        /// <summary>完整统计，包含统计学函数</summary>
        Full = Basic | Stdev | Var | Varp | ApproxCountDistinct,
    }
}
