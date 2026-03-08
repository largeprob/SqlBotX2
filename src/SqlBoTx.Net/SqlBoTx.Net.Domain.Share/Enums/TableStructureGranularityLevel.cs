using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums
{
    public enum TableStructureGranularityLevel
    {
        /// <summary>
        /// 事实表
        /// </summary>
        [Description("事实表")]
        Fact = 1,
        /// <summary>
        /// 周期表
        /// </summary>
        [Description("周期表")]
        Period = 2,
        /// <summary>
        /// 维度表
        /// </summary>
        [Description("维度表")]
        Dimension = 3,
    }
}
