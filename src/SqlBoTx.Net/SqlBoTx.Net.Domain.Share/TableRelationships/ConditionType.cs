using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.TableRelationships
{
    public enum ConditionType
    {
        /// <summary>
        /// 键关联
        /// </summary>
        [Description("键关联")]
        Key = 1,

        /// <summary>
        /// 多态关联
        /// </summary>
        [Description("多态关联")]
        SourceConstant,

        /// <summary>
        /// 多态关联
        /// </summary>
        [Description("多态关联")]
        TargetConstant
    }
}
