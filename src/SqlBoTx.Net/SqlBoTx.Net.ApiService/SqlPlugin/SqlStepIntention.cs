namespace SqlBoTx.Net.ApiService.SqlPlugin
{
    public enum OutVisualType
    {
        /// <summary>
        /// 普通文本输出
        /// </summary>
        Text = 0,
        /// <summary>
        /// 基础表格
        /// </summary>
        BasicTable = 1,
        /// <summary>
        /// Echarts
        /// </summary>
        Echarts = 2
    }

    /// <summary>
    /// 意图识别解构
    /// </summary>
    public class SqlStepIntention
    {
        /// <summary>
        /// chat response message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 是否需要图表
        /// </summary>
        public OutVisualType OutVisualType { get; set; }
    }   
}
