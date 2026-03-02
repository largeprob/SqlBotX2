using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.BusinessObjective
{
    /// <summary>
    /// 分析维度标签
    /// </summary>
    [Flags]
    public enum BusinessObjectiveFieldDimensionTag
    {
        /// <summary>
        /// 地区维度
        /// </summary>
        [Description("地区")]
        地区 = 1,

        /// <summary>
        /// 部门维度
        /// </summary>
        [Description("部门")]
        部门 = 2,

        /// <summary>
        /// 组织维度
        /// </summary>
        [Description("组织")]
        组织 = 3,

        /// <summary>
        /// 角色维度
        /// </summary>
        [Description("角色")]
        角色 = 4,

        /// <summary>
        /// 年份维度
        /// </summary>
        [Description("年")]
        年 = 5,

        /// <summary>
        /// 月份维度
        /// </summary>
        [Description("月")]
        月 = 6,

        /// <summary>
        /// 日期维度（具体到日）
        /// </summary>
        [Description("日")]
        日 = 7,

        /// <summary>
        /// 天维度（可能与日类似，或指星期几）
        /// </summary>
        [Description("天")]
        天 = 8,

        /// <summary>
        /// 小时维度
        /// </summary>
        [Description("时")]
        时 = 9,

        /// <summary>
        /// 分钟维度
        /// </summary>
        [Description("分")]
        分 = 10,

        /// <summary>
        /// 秒钟维度
        /// </summary>
        [Description("秒")]
        秒 = 11,

        /// <summary>
        /// 实体维度
        /// </summary>
        [Description("实体")]
        实体 = 12,

        /// <summary>
        /// 分类维度
        /// </summary>
        [Description("分类")]
        分类 = 13,

        /// <summary>
        /// 其他未指定的维度
        /// </summary>
        [Description("其他")]
        其他 = 14
    }
}
