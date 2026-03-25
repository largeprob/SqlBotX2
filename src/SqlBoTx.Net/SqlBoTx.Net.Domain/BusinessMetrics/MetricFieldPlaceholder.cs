using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.BusinessMetrics
{
    /// <summary>
    /// 指标字段占位符
    /// </summary>
    [Description("指标字段占位符")]
    public class MetricFieldPlaceholder
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Description("主键")]
        public int Id { get; set; }

        /// <summary>
        /// 指标外键
        /// </summary>
        [Description("指标外键")]
        public int MetricId { get; set; }

        /// <summary>
        /// 业务域指标
        /// </summary>
        [Description("业务域指标")]
        public virtual DomainMetric DomainMetric { get; set; }

        /// <summary>
        /// 占位符序号
        /// </summary>
        [Description("占位符序号")]
        public int Index { get; set; }

        /// <summary>
        /// 归属实体
        /// </summary>
        [Description("归属实体Id")]
        public string EntityId { get; set; } = null!;

        /// <summary>
        /// 属性Id
        /// </summary>
        [Description("属性Id")]
        public int AttrId { get; set; }

        /// <summary>
        /// 属性名
        /// </summary>
        [Description("属性名")]
        public string AttrName { get; set; } = null!;
    }
}
