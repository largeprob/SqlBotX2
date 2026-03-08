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
    public enum BusinessObjectiveFieldDimensionLayer
    {
        None = 0,

        [Description("地区")]
        地区 = 1 << 0,  // 1

        [Description("部门")]
        部门 = 1 << 1,  // 2

        [Description("组织")]
        组织 = 1 << 2,  // 4

        [Description("角色")]
        角色 = 1 << 3,  // 8

        [Description("年")]
        年 = 1 << 4,    // 16

        [Description("月")]
        月 = 1 << 5,    // 32

        [Description("日")]
        日 = 1 << 6,    // 64

        [Description("天")]
        天 = 1 << 7,    // 128

        [Description("时")]
        时 = 1 << 8,    // 256

        [Description("分")]
        分 = 1 << 9,    // 512

        [Description("秒")]
        秒 = 1 << 10,   // 1024

        [Description("实体")]
        实体 = 1 << 11, // 2048

        [Description("分类")]
        分类 = 1 << 12, // 4096

        [Description("其他")]
        其他 = 1 << 13  // 8192
    }
}
