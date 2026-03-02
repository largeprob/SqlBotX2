using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SqlBoTx.Net.Application.Contracts.TableStructures.Dtos
{
    /// <summary>
    /// 新增数据库表结构Dto
    /// </summary>
    public class AddTableStructureDto
    {
        /// <summary>
        /// 数据库连接ID（外键）
        /// </summary>
        [DisplayName("数据库连接ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? ConnectionId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [DisplayName("表名")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? TableName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [DisplayName("显示名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// 表描述
        /// </summary>
        [DisplayName("表描述")]
        public string? Description { get; set; }

        /// <summary>
        /// 表字段列表
        /// </summary>
        [DisplayName("表字段列表")]
        public List<AddTableFieldDto> TableFields { get; set; } = new List<AddTableFieldDto>();
    }

    /// <summary>
    /// 新增表字段Dto
    /// </summary>
    public class AddTableFieldDto
    {
        /// <summary>
        /// 字段名称（数据库中实际字段名）
        /// </summary>
        [DisplayName("字段名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? FieldName { get; set; }

        /// <summary>
        /// 字段数据类型（如：int, varchar(200), Decimal(18,2), datetime等）
        /// </summary>
        [DisplayName("字段数据类型")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? DataType { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        [DisplayName("是否为主键")]
        public bool IsPrimaryKey { get; set; } = false;

        /// <summary>
        /// 是否允许为空
        /// </summary>
        [DisplayName("是否允许为空")]
        public bool IsNullable { get; set; } = true;

        /// <summary>
        /// 是否为自增字段
        /// </summary>
        [DisplayName("是否为自增字段")]
        public bool IsIdentity { get; set; } = false;

        /// <summary>
        /// 默认值
        /// </summary>
        [DisplayName("默认值")]
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 字段说明/显示名称
        /// </summary>
        [DisplayName("字段说明")]
        public string? FieldDescription { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [Description("是否可用")]
        public bool? IsAvailable { get; set; }
    }
}
