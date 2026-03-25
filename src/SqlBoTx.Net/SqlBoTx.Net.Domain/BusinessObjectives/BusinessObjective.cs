using SqlBoTx.Net.Domain.BusinessEntities;
using SqlBoTx.Net.Domain.BusinessMetrics;
using SqlBoTx.Net.Domain.DomainTerms;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SqlBoTx.Net.Domain.BusinessObjectives
{
    /// <summary>
    /// 业务域
    /// </summary>
    [Description("业务域")]
    public class BusinessObjective
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        [Description("主键自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 业务域名称
        /// </summary>
        [Description("业务域名称")]
        public string? BusinessName { get; set; }

        /// <summary>
        /// 父业务域ID
        /// </summary>
        [Description("父业务域ID")]
        public int? ParentId { get; set; }

        /// <summary>
        /// 导航属性-父业务域
        /// </summary>
        public virtual BusinessObjective? Parent { get; set; }

        /// <summary>
        /// 导航属性-子业务域
        /// </summary>
        public virtual ICollection<BusinessObjective>? Children { get; set; }


        /// <summary>
        /// 近义词（逗号分隔存储）
        /// </summary>
        [Description("近义词")]
        public string? Synonyms { get; set; }

        /// <summary>
        /// 实体列表
        /// </summary>
        [Description("实体列表")]
        public virtual ICollection<DomainEntity>? Entities { get; set; }

        /// <summary>
        /// 指标列表
        /// </summary>
        [Description("指标列表")]
        public virtual ICollection<DomainMetric>? Metrics { get; set; }

        /// <summary>
        /// 术语列表
        /// </summary>
        [Description("术语列表")]
        public virtual ICollection<DomainTerm>? Terms { get; set; }

        /// <summary>
        /// 业务域解释
        /// </summary>
        [Description("业务解释")]
        public string? Description { get; set; }

        /// <summary>
        /// 业务域总结
        /// </summary>
        [Description("业务域总结")]
        public string? Summary { get; set; }

        /// <summary>
        /// 关键词（逗号分隔存储）
        /// </summary>
        [Description("关键词：快速定位该领域，而非近义词")]
        public string? KeyWords { get; set; }

        /// <summary>
        /// 领域标签：核心域、支撑域
        /// </summary>
        [Description("领域标签：核心域、支撑域、通用域、系统域")]
        public string[]? Tags { get; set; }

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
