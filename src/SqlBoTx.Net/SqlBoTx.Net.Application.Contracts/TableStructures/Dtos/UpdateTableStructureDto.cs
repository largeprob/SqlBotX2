using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SqlBoTx.Net.Application.Contracts.TableStructures.Dtos
{
    /// <summary>
    /// 编辑数据库表结构Dto
    /// </summary>
    public class UpdateTableStructureDto : AddTableStructureDto
    {
        /// <summary>
        /// 表ID（主键）
        /// </summary>
        [DisplayName("TableId")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int TableId { get; set; }
    }

    /// <summary>
    /// 编辑表字段Dto
    /// </summary>
    public class UpdateTableFieldDto : AddTableFieldDto
    {
        /// <summary>
        /// 字段ID（主键）
        /// </summary>
        [DisplayName("FieldId")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int FieldId { get; set; }
    }
}
