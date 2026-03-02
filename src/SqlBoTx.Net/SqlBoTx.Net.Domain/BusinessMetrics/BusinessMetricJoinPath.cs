using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.BusinessMetrics
{
    /// <summary>
    /// 业务指标路径
    /// </summary>
    [Description("业务指标路径")]
    public class BusinessMetricJoinPath
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Description("主键")]
        public int Id { get; set; }

        /// <summary>
        /// 业务指标Id
        /// </summary>
        [Description("业务指标Id")]
        public int BusinessMetricId { get; set; }

        /// <summary>
        /// 排序 (确保 Join 顺序正确：表A -> 表B -> 表C)
        /// </summary>
        [Description("排序")]
        public int Order { get; set; }

        /// <summary>
        /// 目标表Id
        /// </summary>
        [Description("目标表Id）")]
        public int TableId { get; set; }

        /// <summary>
        /// 导航属性 - 目标表
        /// </summary>
        [Description("导航属性 - 目标表")]
        public virtual TableStructure? Table { get; set; }

        /// <summary>
        /// 目标表别名 (如 Target, Used in SqlExpression)
        /// </summary>
        [Description("目标表别名")]
        public string Alias { get; set; } = string.Empty;

        /// <summary>
        /// 连接类型 (LEFT JOIN, INNER JOIN)
        /// </summary>
        [Description("连接类型")]
        public string JoinType { get; set; } = "LEFT JOIN";

        /// <summary>
        /// 连接条件 (Main.id = Order.opp_id)
        /// </summary>
        [Description("连接条件")]
        public string OnCondition { get; set; } = string.Empty;
    }
}
