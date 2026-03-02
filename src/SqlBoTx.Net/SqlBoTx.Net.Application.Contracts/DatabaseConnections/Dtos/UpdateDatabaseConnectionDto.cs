using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlBoTx.Net.Application.Contracts.DatabaseConnections.Dtos
{
    /// <summary>
    /// 编辑数据库连接配置Dto
    /// </summary>
    public class UpdateDatabaseConnectionDto: AddDatabaseConnectionDto
    {
        /// <summary>
        /// 连接ID（主键）
        /// </summary>
        [DisplayName("ConnectionId")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int ConnectionId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModifiedDate { get; set; } = DateTime.Now;
    }
}
