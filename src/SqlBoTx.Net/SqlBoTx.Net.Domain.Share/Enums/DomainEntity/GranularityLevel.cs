using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.DomainEntity
{
    public enum GranularityLevel
    {
        /// <summary>
        /// 事实表
        /// </summary>
        [Description("事实实体")]
        Fact = 1,
        /// <summary>
        /// 周期表
        /// </summary>
        [Description("周期实体")]
        Period,
        /// <summary>
        /// 维度表
        /// </summary>
        [Description("维度实体")]
        Dimension,
        /// <summary>
        /// 关联实体
        /// </summary>
        [Description("关联实体")]
        Bridge,
    }
}
