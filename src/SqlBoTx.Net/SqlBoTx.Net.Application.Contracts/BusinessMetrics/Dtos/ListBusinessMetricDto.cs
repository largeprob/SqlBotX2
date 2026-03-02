using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using SqlBoTx.Net.Domain.Share.Enums.BusinessObjective;

namespace SqlBoTx.Net.Application.Contracts.BusinessMetrics.Dtos
{
    /// <summary>
    /// List业务指标Dto
    /// </summary>
    public class ListBusinessMetricDto
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 指标名称
        /// </summary>
        public string MetricName { get; set; } = string.Empty;

        /// <summary>
        /// 指标编码
        /// </summary>
        public string MetricCode { get; set; } = string.Empty;

        /// <summary>
        /// 近义词/检索关键词
        /// </summary>
        public string? Alias { get; set; }

        /// <summary>
        /// 归属业务模块ID
        /// </summary>
        public int BusinessObjectiveId { get; set; }

        /// <summary>
        /// 指标解释
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 生命周期状态
        /// </summary>
        public BusinessMetricStatus Status { get; set; }

        /// <summary>
        /// 依赖主体-主表ID
        /// </summary>
        public int MainTableId { get; set; }

        /// <summary>
        /// 主表别名
        /// </summary>
        public string MainAlias { get; set; } = "Main";

        /// <summary>
        /// 计算公式
        /// </summary>
        public string Expression { get; set; } = string.Empty;

        /// <summary>
        /// 连接路径
        /// </summary>
        public IEnumerable<ListBusinessMetricJoinPathDto>? JoinPaths { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedDate { get; set; }
    }

    /// <summary>
    /// List业务指标连接路径Dto
    /// </summary>
    public class ListBusinessMetricJoinPathDto
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 目标表ID
        /// </summary>
        public int TableId { get; set; }

        /// <summary>
        /// 目标表别名
        /// </summary>
        public string Alias { get; set; } = string.Empty;

        /// <summary>
        /// 连接类型
        /// </summary>
        public string JoinType { get; set; } = "LEFT JOIN";

        /// <summary>
        /// 连接条件
        /// </summary>
        public string OnCondition { get; set; } = string.Empty;
    }
}
