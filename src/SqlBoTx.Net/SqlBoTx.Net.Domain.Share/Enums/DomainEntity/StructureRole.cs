using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.DomainEntity
{

    [Flags]
    public enum StructureRole
    {
        None = 0,

        [Description("主键")]
        PrimaryKey = 1 << 0,   // 1

        [Description("外键")]
        ForeignKey = 1 << 1,   // 2

        [Description("标题列")]
        TitleKey = 1 << 2,   // 4

        [Description("普通列")]
        Column = 1 << 3,   // 8

        /// <summary>
        /// 共享主键模式
        /// </summary>
        [Description("共享主键模式")]
        PrimaryForeignKey = PrimaryKey | ForeignKey,
    }
}
