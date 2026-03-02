using SqlBoTx.Net.Domain.BusinessMetrics;
using System.ComponentModel;

namespace SqlBoTx.Net.Domain.BusinessObjectives
{
    /// <summary>
    /// 业务目标
    /// </summary>
    [Description("业务目标")]
    public class BusinessObjective
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        [Description("主键自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        [Description("业务名称")]
        public string? BusinessName { get; set; }

        /// <summary>
        /// 近义词（逗号分隔存储）
        /// </summary>
        [Description("近义词")]
        public string? Synonyms { get; set; }

        /// <summary>
        /// 依赖表
        /// </summary>
        [Description("依赖表")]
        public virtual ICollection<BusinessObjectiveDependencyTable>? DependencyTables { get; set; }

        /// <summary>
        /// 固定字段
        /// </summary>
        [Description("固定字段")]
        public virtual ICollection<BusinessObjectiveField>? Fields { get; set; }

        /// <summary>
        /// 指标集/计算字段
        /// </summary>
        [Description("指标集/计算字段")]
        public virtual ICollection<BusinessObjectiveMetric>? Metrics { get; set; }

        /// <summary>
        /// 业务解释
        /// </summary>
        [Description("业务解释")]
        public string? Description { get; set; }

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
