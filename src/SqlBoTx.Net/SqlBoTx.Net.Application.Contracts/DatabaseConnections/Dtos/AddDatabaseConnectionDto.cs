using SqlBoTx.Net.Domain.Share.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlBoTx.Net.Application.Contracts.DatabaseConnections.Dtos
{
    /// <summary>
    /// 新增数据库连接配置Dto
    /// </summary>
    public class AddDatabaseConnectionDto
    {
        /// <summary>
        /// 连接名称
        /// </summary>
        [DisplayName("连接名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Description("连接名称")]
        public string? ConnectionName { get; set; }

        /// <summary>
        /// 连接类型（SQL Server、MySQL、Oracle、PostgreSQL等）
        /// </summary>
        [DisplayName("连接类型")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Description("连接类型")]
        public ConnectionType? ConnectionType { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        [DisplayName("数据库连接")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Description("数据库连接")]
        public string? ConnectionString { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [DisplayName("用户名")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Description("用户名")]
        public string? UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [DisplayName("密码")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Description("密码")]
        public string? UserPassword { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [DisplayName("描述信息")]
        [Description("描述信息")]
        public string? Description { get; set; }
    }
}
