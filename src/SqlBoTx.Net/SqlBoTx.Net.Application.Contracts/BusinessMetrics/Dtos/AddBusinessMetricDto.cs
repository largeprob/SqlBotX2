using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using SqlBoTx.Net.Domain.Share.Enums.BusinessObjective;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SqlBoTx.Net.Application.Contracts.BusinessMetrics.Dtos
{
    /// <summary>
    /// 新增业务指标Dto
    /// </summary>
    public class AddBusinessMetricDto
    {
        /// <summary>
        /// 指标名称
        /// </summary>
        [DisplayName("指标名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? MetricName { get; set; }

        /// <summary>
        /// 指标编码
        /// </summary>
        [DisplayName("指标编码")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? MetricCode { get; set; }

        /// <summary>
        /// 近义词/检索关键词
        /// </summary>
        [DisplayName("近义词")]
        public string? Alias { get; set; }

        /// <summary>
        /// 归属业务模块ID
        /// </summary>
        [DisplayName("归属业务模块")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? BusinessObjectiveId { get; set; }

        /// <summary>
        /// 指标解释
        /// </summary>
        [DisplayName("指标解释")]
        public string? Description { get; set; }

        /// <summary>
        /// 生命周期状态
        /// </summary>
        [DisplayName("生命周期状态")]
        [Required(ErrorMessage = "{0}不能为空")]
        public BusinessMetricStatus? Status { get; set; }

        /// <summary>
        /// 依赖主体-主表ID
        /// </summary>
        [DisplayName("依赖主体-主表")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? MainTableId { get; set; }

        /// <summary>
        /// 主表别名
        /// </summary>
        [DisplayName("主表别名")]
        public string? MainAlias { get; set; } = "Main";

        /// <summary>
        /// 计算公式
        /// </summary>
        [DisplayName("计算公式")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? Expression { get; set; }

        /// <summary>
        /// 连接路径
        /// </summary>
        [DisplayName("连接路径")]
        [Required(ErrorMessage = "{0}不能为空")]
        public List<BusinessMetricJoinPathDto>? JoinPaths { get; set; }
    }

    /// <summary>
    /// 业务指标连接路径Dto
    /// </summary>
    public class BusinessMetricJoinPathDto
    {
        /// <summary>
        /// 目标表ID
        /// </summary>
        [DisplayName("目标表ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? TableId { get; set; }

        /// <summary>
        /// 目标表别名
        /// </summary>
        [DisplayName("目标表别名")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? Alias { get; set; }

        /// <summary>
        /// 连接类型
        /// </summary>
        [DisplayName("连接类型")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? JoinType { get; set; } = "LEFT JOIN";

        /// <summary>
        /// 连接条件
        /// </summary>
        [DisplayName("连接条件")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? OnCondition { get; set; }
    }
}
