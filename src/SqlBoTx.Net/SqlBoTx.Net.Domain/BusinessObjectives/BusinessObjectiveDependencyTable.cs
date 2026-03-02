using SqlBoTx.Net.Domain.Share;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.BusinessObjectives
{
    /// <summary>
    /// 业务目标依赖表
    /// </summary>
    [Description("业务目标依赖表")]
    public class BusinessObjectiveDependencyTable: ValueObject
    {
        [Description("表ID")]
        public int TableId{ get; set; }
    }
}
