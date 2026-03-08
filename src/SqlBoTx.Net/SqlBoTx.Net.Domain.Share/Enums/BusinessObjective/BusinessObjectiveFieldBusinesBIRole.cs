using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.BusinessObjective
{
    public  enum BusinessObjectiveFieldBusinesBIRole
    {
        /// <summary>
        /// 属性
        /// </summary>
        [Description("属性")]
        Attribute = 0,

        /// <summary>
        /// 维度
        /// </summary>
        [Description("维度")]
        Demension = 1,

        /// <summary>
        /// 度量
        /// </summary>
        [Description("度量")]
        Metric = 2,
    }
}
