namespace SqlBoTx.Net.ApiService.SkKernel
{
    public enum ModelType
    {
        /// <summary>
        /// 对话模型
        /// </summary>
        Chat = 1,
        /// <summary>
        /// 向量模型
        /// </summary>
        Embedding = 2,
    }

    /// <summary>
    /// 模型
    /// </summary>
    public class ModleInfo
    {
        /// <summary>
        /// 模型类型
        /// </summary>
        public ModelType ModelType { get; set; }

        /// <summary>
        /// 模型ID
        /// </summary>
        public string ModelId { get; set; } = string.Empty;

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// URL
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
