using SqlBoTx.Net.Domain.Share.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlBoTx.Net.Application.Contracts.DatabaseConnections.Dtos
{
    /// <summary>
    /// List数据库连接配置Dto
    /// </summary>
    public class ListDatabaseConnectionDto
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// 连接ID（主键）
        /// </summary>
        public int ConnectionId { get; set; }

        /// <summary>
        /// 连接名称
        /// </summary>
        public string? ConnectionName { get; set; }

        /// <summary>
        /// 连接类型（SQL Server、MySQL、Oracle、PostgreSQL等）
        /// </summary>
        public ConnectionType? ConnectionType { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? UserPassword { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string? Description { get; set; }
    }
}
