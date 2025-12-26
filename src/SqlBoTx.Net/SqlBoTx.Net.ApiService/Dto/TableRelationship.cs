namespace SqlBoTx.Net.ApiService.Dto
{
    /// <summary>
    /// 表关系信息
    /// </summary>
    public class TableRelationship
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string? TableName {  get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string? Description {  get; set; }

        /// <summary>
        /// 关系
        /// </summary>
        public List<TableRelationshipDetail>? Relationships { get; set; }
    } 

    /// <summary>
    /// 表关系详情
    /// </summary>
    public class TableRelationshipDetail
    {
        /// <summary>
        /// 关系字段-本表
        /// </summary>
        public string? SourceColumn { get; set; }

        /// <summary>
        /// 关系表
        /// </summary>
        public string? TargetTable { get; set; }

        /// <summary>
        /// 关系字段-目标表
        /// </summary>
        public string? TargetColumn { get; set; }

        /// <summary>
        /// 关系类型： ONE_TO_MANY/ONE_TO_ONE/MANY_TO_ONE
        /// </summary>
        public string? RelationType { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string? Description { get; set; }
    }
}
