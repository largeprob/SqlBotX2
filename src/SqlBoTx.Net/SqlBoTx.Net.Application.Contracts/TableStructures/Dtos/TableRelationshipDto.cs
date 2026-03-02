using SqlBoTx.Net.Domain.Share.Enums;
using System.ComponentModel.DataAnnotations;
using SqlBoTx.Net.Domain.Share.TableRelationships;

namespace SqlBoTx.Net.Application.Contracts.TableStructures.Dtos
{
    /// <summary>
    /// 添加表关系Dto
    /// </summary>
    public class AddTableRelationshipDto
    {
        /// <summary>
        /// 源表ID
        /// </summary>
        [Required(ErrorMessage = "来源表不能为空")]
        public int? SourceTableId { get; set; }

        /// <summary>
        /// 目标表ID
        /// </summary>
        [Required(ErrorMessage = "目标表不能为空")]
        public int TargetTableId { get; set; }

        /// <summary>
        /// 关系类型
        /// </summary>
        [Required(ErrorMessage = "关系类型不能为空")]
        public TableRelationshipType? RelationshipType { get; set; }

        /// <summary>
        /// 关联条件（JSON格式）
        /// </summary>
        [Required(ErrorMessage = "关联条件不能为空")]
        public string JoinConditions { get; set; } = "[]";
    }

    /// <summary>
    /// 更新表关系Dto
    /// </summary>
    public class UpdateTableRelationshipDto: AddTableRelationshipDto
    {
      
    }

    /// <summary>
    /// 列表表关系Dto
    /// </summary>
    public class ListTableRelationshipDto
    {
        /// <summary>
        /// 关系ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 源表ID
        /// </summary>
        public int SourceTableId { get; set; }

        /// <summary>
        /// 源表名
        /// </summary>
        public string? SourceTableName { get; set; }

        /// <summary>
        /// 源表显示名称
        /// </summary>
        public string? SourceTableDisplayName { get; set; }

        /// <summary>
        /// 目标表ID
        /// </summary>
        public int TargetTableId { get; set; }

        /// <summary>
        /// 目标表名
        /// </summary>
        public string? TargetTableName { get; set; }

        /// <summary>
        /// 目标表显示名称
        /// </summary>
        public string? TargetTableDisplayName { get; set; }

        /// <summary>
        /// 关系类型
        /// </summary>
        public TableRelationshipType RelationshipType { get; set; }

        /// <summary>
        /// 关联条件（JSON格式）
        /// </summary>
        public List<TableRelationshipCondition>? JoinConditions { get; set; }
    }
}
