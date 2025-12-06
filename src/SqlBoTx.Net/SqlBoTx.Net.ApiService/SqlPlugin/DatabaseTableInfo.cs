namespace SqlBoTx.Net.ApiService.SqlPlugin
{
    /// <summary>
    /// 表信息
    /// </summary>
    public class DatabaseTableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyDate { get; set; }
    }
}
