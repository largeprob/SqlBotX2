using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.DomainEntity
{
    /// <summary>
    /// 分析维度标签
    /// </summary>
    [Flags]
    public enum DimensionCategory
    {
        None = 0,

        // 空间维度
        [Description("地区")]
        地区 = 1 << 0,   // 1

        // 组织维度
        [Description("部门")]
        部门 = 1 << 1,   // 2
        [Description("组织")]
        组织 = 1 << 2,   // 4
        [Description("角色")]
        角色 = 1 << 3,   // 8

        // 主体维度
        [Description("主体")]
        主体 = 1 << 4,   // 16

        // 分类维度
        [Description("分类")]
        分类 = 1 << 5,   // 32

        [Description("其他")]
        其他 = 1 << 6,   // 64
    }
}
