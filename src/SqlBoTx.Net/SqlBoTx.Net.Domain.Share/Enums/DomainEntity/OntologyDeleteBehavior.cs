using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.DomainEntity
{
    /// <summary>
    /// 基于本体论-实体删除关系
    /// </summary>
    public enum OntologyDeleteBehavior
    {
        /// <summary>
        /// 禁止删除
        /// </summary>
        [Description("禁止删除")]
        Restrict,   // 禁止删除，保护数据完整性
        /// <summary>
        /// 级联删除
        /// </summary>
        [Description("级联删除")]
        Cascade,    // 级联删除，整体-部分关系
        /// <summary>
        /// 置空
        /// </summary>
        [Description("置空")]
        SetNull,    // 置空，软断开关系
        /// <summary>
        /// 不处理
        /// </summary>
        [Description("不处理")]
        NoAction    // 不处理，由业务代码负责
    }
}
