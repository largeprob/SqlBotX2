using System.ComponentModel;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlPlugin
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OutVisualType
    {
        /// <summary>
        /// 无法判断
        /// </summary>
        None = 0,
        /// <summary>
        /// 普通文本输出
        /// </summary>
        Text = 1,
        /// <summary>
        /// 基础表格
        /// </summary>
        BasicTable = 2,
        /// <summary>
        /// Echarts
        /// </summary>
        Echarts = 3
    }

    /// <summary>
    /// 意图识别解构
    /// </summary>
    public class SqlStepIntention
    {
        /// <summary>
        /// 意图识别是否成功
        /// </summary>
        public bool ThisSuccess { get; set; } 

        /// <summary>
        /// chat response message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 可视化工具类型
        /// </summary>
        public OutVisualType OutVisualType { get; set; }

        /// <summary>
        /// 是否可以继续
        /// </summary>
        public bool IsStep { get; set; }
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IntentAnalysisCategory
    {
        /// <summary>
        /// 聚合
        /// </summary>
        [Description("聚合")]
        AGGREGATE,
        /// <summary>
        /// 明细
        /// </summary>
        [Description("明细")]
        DETAIL,
        /// <summary>
        /// 无效
        /// </summary>
        [Description("无效")]
        INVALID
    }

    /// <summary>
    /// 意图分析结果
    /// </summary>
    public class IntentAnalysis
    {
        /// <summary>
        /// 思维推理步骤
        /// </summary>
        public string? ThoughtProcess { get; set; }

      /// <summary>
      /// 分类
      /// </summary>
        public IntentAnalysisCategory Category { get; set; }

        /// <summary>
        /// 是否触发了默认明细规则
        /// </summary>
        public bool IsDefaultFallback { get; set; }

        /// <summary>
        /// 识别到的核心实体
        /// </summary>
        public string? PrimaryEntity { get; set; } = string.Empty;

        /// <summary>
        /// 引导话术，用于提升用户下一次输入的准确度
        /// </summary>
        public string? UserGuidance { get; set; } = string.Empty;
    }

}
