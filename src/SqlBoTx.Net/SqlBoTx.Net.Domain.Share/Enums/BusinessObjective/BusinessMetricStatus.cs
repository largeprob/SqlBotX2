using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.BusinessObjective
{
    public enum BusinessMetricStatus
    {

        /// <summary>
        /// 自动反馈机制生成
        /// </summary>
        [Description("自动反馈机制生成")]
        AI = 1,
        /// <summary>
        /// 核验发布
        /// </summary>
        [Description("核验发布")]
        VERIFIED = 2,
    }
}
