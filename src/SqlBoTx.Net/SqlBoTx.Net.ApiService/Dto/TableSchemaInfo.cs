namespace SqlBoTx.Net.ApiService.Dto
{

    /// <summary>
    /// 表描述
    /// </summary>
    public class TableSchemaInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; } = default!;

        /// <summary>
        /// 注释
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// 列信息
        /// </summary>
        public List<TableSchemaColumnInfo> Columns { get; set; } = new();

        /// <summary>
        /// 表关系
        /// </summary>
        public List<TableRelationShip> Relations { get; set; } = new();
    }

    /// <summary>
    /// 列信息
    /// </summary>
    public class TableSchemaColumnInfo
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 数据类型
        /// </summary>
        public string Type { get; set; } = default!;

        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPk { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Description { get; set; } = default!;
    }


    public class TableRelationShip
    {
        /// <summary>
        /// 外键列
        /// </summary>
        public string ForeignKeyColumn { get; set; } = default!;

        /// <summary>
        /// 关联表
        /// </summary>
        public string ReferenceTable { get; set; } = default!;

        /// <summary>
        /// 关联列
        /// </summary>
        public string ReferenceColumn { get; set; } = default!;
    }
}
