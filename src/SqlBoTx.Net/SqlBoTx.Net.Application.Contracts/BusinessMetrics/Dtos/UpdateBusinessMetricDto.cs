using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SqlBoTx.Net.Application.Contracts.BusinessMetrics.Dtos
{
    /// <summary>
    /// 编辑业务指标Dto
    /// </summary>
    public class UpdateBusinessMetricDto : AddBusinessMetricDto
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        [DisplayName("ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int Id { get; set; }
    }
}
