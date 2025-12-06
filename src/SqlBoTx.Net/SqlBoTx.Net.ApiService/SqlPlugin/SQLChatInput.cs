using System.ComponentModel.DataAnnotations;

namespace SqlBoTx.Net.ApiService.SqlPlugin
{
    public enum SQLChatMessageRole
    {
        /// <summary>
        /// 用户
        /// </summary>
        User = 0,

        /// <summary>
        /// AI
        /// </summary>
        Bot = 1,
    }

    /// <summary>
    /// 对话输入
    /// </summary>
    public class SQLChatMessage
    {
        /// <summary>
        /// 输入角色
        /// </summary>
        [Required(ErrorMessage ="输入角色不能为空")]
        public SQLChatMessageRole? Role { get; set; }

        /// <summary>
        /// 输入内容不能为空
        /// </summary>
        [Required(ErrorMessage = "输入内容不能为空")]
        public string? Content { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
