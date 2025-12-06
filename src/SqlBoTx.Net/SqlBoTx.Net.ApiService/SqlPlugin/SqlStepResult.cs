namespace SqlBoTx.Net.ApiService.SqlPlugin
{
    /// <summary>
    /// sql chat 返回结果
    /// </summary>
    public class SqlStepResult
    {
        /// <summary>
        /// chat response message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// chat response message
        /// </summary>
        public string[] Tables { get; set; } = Array.Empty<string>();

        /// <summary>
        /// chat response message
        /// </summary>
        public SqlStepColumns[]? Columns { get; set; }   

        /// <summary>
        /// chat response sql
        /// </summary>
        public string Sql { get; set; } =  string.Empty;

        /// <summary>
        /// is chart needed
        /// </summary>
        public bool NeedChart { get; set; }
    }

    public class SqlStepColumns 
    {
        /// <summary>
        /// chat response data
        /// </summary>
        public string Lable { get; set; } = default!;

        /// <summary>
        /// chat response data
        /// </summary>
        public string Key{ get; set; } = default!;
    }
}
