using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.TableRelationships
{
    public enum ConditionType
    {
        /// <summary>
        /// 外键关联
        /// </summary>
        [Description("外键关联")]
        Key = 1,
        /// <summary>
        /// 多态关联
        /// </summary>
        [Description("多态关联")]
        SourceConstant =2 ,
        /// <summary>
        /// 多态关联
        /// </summary>
        [Description("多态关联")]
        TargetConstant = 3
    }
}
