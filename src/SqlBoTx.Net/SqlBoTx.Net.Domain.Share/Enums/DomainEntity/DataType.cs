using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.DomainEntity
{
    /// <summary>
    /// 实体属性类型
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// 数值
        /// </summary>
        [Description("数值")]
        Number = 1,
        /// <summary>
        /// 字符串
        /// </summary>
        [Description("字符串")]
        String,
        /// <summary>
        /// 日期
        /// </summary>
        [Description("日期")]
        Date,
        /// <summary>
        /// Json格式
        /// </summary>
        [Description("Json格式")]
        Json,
        /// <summary>
        /// 枚举
        /// </summary>
        [Description("枚举")]
        Enum,
        /// <summary>
        /// Bool
        /// </summary>
        [Description("Bool")]
        Boolean,
    }
}
