using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.BusinessObjective
{
    public enum MetricScope
    {
        [Description("实体级")]
        Entity,

        [Description("域级")]
        Domain
    }
}
