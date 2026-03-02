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
    public class BusinessObjectiveMetric
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
        public string? Alias { get; set; }

        /// <summary>
        /// 外键-归属业务模块Id
        /// </summary>
        [Description("外键-归属业务模块Id")]
        public int BusinessObjectiveId { get; set; }

        /// <summary>
        /// 导航属性 - 归属业务模块
        /// </summary>
        [Description("导航属性 - 归属业务模块")]
        public virtual BusinessObjective? BusinessObjective { get; set; }

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

        #region 锚点
        /// <summary>
        /// 依赖主体-主表
        /// </summary>
        [Description("依赖主体-主表")]
        public int MainTableId { get; set; }

        /// <summary>
        /// 导航属性 - 依赖主体-主表
        /// </summary>
        [Description("导航属性 - 依赖主体-主表")]
        public virtual TableStructure? MainTable { get; set; }

        /// <summary>
        /// 主表别名 (默认为 Main)
        /// </summary>
        public string? MainAlias { get; set; } = "Main";
        #endregion

        /// <summary>
        /// 计算公式（非原子）
        /// </summary>
        [Description("计算公式（非原子）")]
        public string? Expression { get; set; }

        /// <summary>
        /// 路径依赖
        /// </summary>
        [Description("路径依赖")]
        public virtual List<BusinessMetricJoinPath>? JoinPaths { get; set; }

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
