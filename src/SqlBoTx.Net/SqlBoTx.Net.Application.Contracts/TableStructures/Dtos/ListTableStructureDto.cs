using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SqlBoTx.Net.Application.Contracts.TableStructures.Dtos
{
    /// <summary>
    /// List数据库表结构Dto
    /// </summary>
    public class ListTableStructureDto
    {
        /// <summary>
        /// 表ID（主键）
        /// </summary>
        public int TableId { get; set; }

        /// <summary>
        /// 数据库连接ID（外键）
        /// </summary>
        public int ConnectionId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// 字段数量
        /// </summary>
        public int FieldCount { get; set; }

        /// <summary>
        /// 表描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 表字段列表
        /// </summary>
        public List<ListTableFieldDto> TableFields { get; set; } = new List<ListTableFieldDto>();
    }

    /// <summary>
    /// List表字段Dto
    /// </summary>
    public class ListTableFieldDto
    {
        /// <summary>
        /// 字段ID（主键）
        /// </summary>
        public int FieldId { get; set; }

        /// <summary>
        /// 字段名称（数据库中实际字段名）
        /// </summary>
        public string? FieldName { get; set; }

        /// <summary>
        /// 字段数据类型（如：int, varchar(200), Decimal(18,2), datetime等）
        /// </summary>
        public string? DataType { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// 是否为自增字段
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 字段说明/显示名称
        /// </summary>
        public string? FieldDescription { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [Description("是否可用")]
        public bool? IsAvailable { get; set; }
    }
}
