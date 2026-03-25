using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.DomainEntity
{
    public enum TimeDimensionGranularity
    {
        /// <summary>
        /// None
        /// </summary>
        [Description("None")]
        None = 0,
        /// <summary>
        /// 年
        /// </summary>
        [Description("年")]
        年,    // 这个字段只存到年，不能按月聚合
        /// <summary>
        /// 季
        /// </summary>
        [Description("季")]
        季,
        /// <summary>
        /// 月
        /// </summary>
        [Description("月")]
        月,    // 这个字段存到月，可以按年/季/月聚合
        /// <summary>
        /// 周
        /// </summary>
        [Description("周")]
        周,
        /// <summary>
        /// 日
        /// </summary>
        [Description("日")]
        日,    // 这个字段存到日，可以按任意粒度聚合
        /// <summary>
        /// 时
        /// </summary>
        [Description("时")]
        时,
        /// <summary>
        /// 分
        /// </summary>
        [Description("分")]
        分,
        /// <summary>
        /// 秒
        /// </summary>
        [Description("秒")]
        秒
    }
}
