using SqlBoTx.Net.Domain.BusinessEntities;
using SqlBoTx.Net.Domain.BusinessObjectives;
using SqlBoTx.Net.Domain.Share.Enums.BusinessObjective;
using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.Text;

namespace SqlBoTx.Net.Domain.BusinessMetrics
{
    /// <summary>
    /// 业务指标
    /// </summary>
    [Description("业务指标")]
    public class DomainMetric
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Description("主键")]
        public int Id { get; set; }

        /// <summary>
        /// 指标名称
        /// </summary>
        [Description("指标名称")]
        public string? MetricName { get; set; }

        /// <summary>
        /// 指标编码
        /// </summary>
        [Description("指标编码")]
        public string? MetricCode { get; set; }

        /// <summary>
        /// 近义词/检索关键词
        /// </summary>
        [Description("近义词/检索关键词")]
        public string[]? Alias { get; set; }

        /// <summary>
        /// 作用域
        /// </summary>
        [Description("作用域")]
        public MetricScope Scope { get; set; }

        /// <summary>
        /// 外键-归属业务模块Id
        /// </summary>
        [Description("外键-归属业务模块Id")]
        public int? DomainId { get; set; }

        /// <summary>
        /// 导航属性 - 归属业务模块
        /// </summary>
        [Description("导航属性 - 归属业务模块")]
        public virtual BusinessObjective? Domain { get; set; }

        /// <summary>
        /// 指标解释
        /// </summary>
        [Description("指标解释")]
        public string? Description { get; set; }

        /// <summary>
        /// 生命周期
        /// </summary>
        [Description("生命周期")]
        public BusinessMetricStatus? Status { get; set; }

        /// <summary>
        /// 字段占位符
        /// </summary>
        [Description("生命周期")]
        public virtual List<MetricFieldPlaceholder> Placeholders { get; set; } = new();

        /// <summary>
        /// 计算公式-数据库公式（非原子）
        /// 例："SUM({1}) / COUNT(DISTINCT {2})"
        /// </summary>
        [Description("计算公式（非原子）")]
        public string Expression { get; set; } = null!;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        [Description("更新时间")]
        public DateTime? UpdatedDate { get; set; }
    }

  
    
}
