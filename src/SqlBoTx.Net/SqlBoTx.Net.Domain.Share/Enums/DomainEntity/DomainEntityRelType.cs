using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.DomainEntity
{
    public enum DomainEntityRelType
    {
        //Source 是 Target 的组成部分，生命周期完全绑定
        /// <summary>
        /// 普通关联，两个实体独立存在，只是有联系
        /// </summary>
        [Description("普通关联，两个实体独立存在，只是有联系")]
        Association = 1,

        //Source 当前属于 Target，但有自己的独立意义
        /// <summary>
        /// 聚合，部分可以脱离整体独立存在
        /// </summary>
        [Description("聚合，部分可以脱离整体独立存在")]
        Aggregation,
        //两者完全独立，只是业务上有联系
        /// <summary>
        /// 组合，部分不能脱离整体存在，整体删除则部分也删除
        /// </summary>
        [Description("组合，部分不能脱离整体存在，整体删除则部分也删除")]
        Composition,

        //Source 借用了 Target 的定义内容
        /// <summary>
        /// 依赖，一方依赖另一方的定义
        /// </summary>
        [Description(" 依赖，一方依赖另一方的定义")]
        Dependency,
    }
}
