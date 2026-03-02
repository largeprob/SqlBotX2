using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SqlBoTx.Net.Application.Contracts.BusinessObjectives.Dtos
{
    /// <summary>
    /// 新增业务目标Dto
    /// </summary>
    public class AddBusinessObjectiveDto
    {
        /// <summary>
        /// 业务名称
        /// </summary>
        [DisplayName("业务名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string? BusinessName { get; set; }

        /// <summary>
        /// 近义词（逗号分隔）
        /// </summary>
        [DisplayName("近义词")]
        public string? Synonyms { get; set; }

        /// <summary>
        /// 依赖表
        /// </summary>
        [DisplayName("依赖表")]
        [Required(ErrorMessage = "{0}不能为空")]
        public ICollection<DependencyTable>? DependencyTables { get; set; }

        /// <summary>
        /// 业务解释
        /// </summary>
        [DisplayName("业务解释")]
        public string? Description { get; set; }
    }

    public class DependencyTable
    {
        [DisplayName("依赖表ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? TableId { get; set; }
    }


    /// <summary>
    /// 编辑业务目标Dto
    /// </summary>
    public class UpdateBusinessObjectiveDto : AddBusinessObjectiveDto
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        [DisplayName("ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int Id { get; set; }
    }

    /// <summary>
    /// List业务目标Dto
    /// </summary>
    public class ListBusinessObjectiveDto
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string BusinessName { get; set; } = string.Empty;

        /// <summary>
        /// 近义词（逗号分隔）
        /// </summary>
        public string? Synonyms { get; set; }

        /// <summary>
        /// 依赖表
        /// </summary>
        public IEnumerable<ListTableStructureDto>? DependencyTables { get; set; }

        /// <summary>
        /// 业务解释
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedDate { get; set; }


    }
}
