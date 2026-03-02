using Microsoft.AspNetCore.Mvc;
using SqlBoTx.Net.ApiService.Dto;
using SqlBoTx.Net.Application.Contracts.DatabaseConnections;
using SqlBoTx.Net.Application.Contracts.DatabaseConnections.Dtos;
using SqlBoTx.Net.Core.Controller;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace SqlBoTx.Net.ApiService.Controllers
{
    /// <summary>
    /// 数据库连接配置管理
    /// </summary>
    [Route("[controller]")]
    [Consumes("application/json")]
    [Tags("数据库连接配置管理")]
    public class DatabaseSettingController : LarApi
    {
        private readonly IDatabaseConnectionService _databaseConnectionService;

        public DatabaseSettingController(IDatabaseConnectionService databaseConnectionService)
        {
            _databaseConnectionService = databaseConnectionService;
        }

        /// <summary>
        /// 列表（不包含表结构）
        /// </summary>
        /// <returns>返回数组（不包含表结构）</returns>
        [HttpGet("list")]
        [ProducesResponseType(typeof(List<ListDatabaseConnectionDto>), 200)]
        public async Task<IActionResult> List()
        {
            var list = await _databaseConnectionService.ListAsync();
            return Ok(list);
        }

        /// <summary>
        /// 列表（包含表结构）
        /// </summary>
        /// <returns>返回数组（包含表结构）</returns>
        [HttpGet("list-with-tables")]
        [ProducesResponseType(typeof(List<ListDatabaseConnectionDto>), 200)]
        public async Task<IActionResult> ListWithTables()
        {
            var list = await _databaseConnectionService.ListWithTablesAsync();
            return Ok(list);
        }

        /// <summary>
        /// 新增数据库连接
        /// </summary>
        /// <returns>返回受影响的行数</returns>
        [HttpPost("add")]
        [ProducesResponseType(typeof(Int32), 200)]
        public async Task<IActionResult> Add([FromBody] AddDatabaseConnectionDto input)
        {
            var result = await _databaseConnectionService.AddAsync(input);
            return Ok(result);
        }

        /// <summary>
        /// 更新数据库连接
        /// </summary>
        /// <returns>返回受影响的行数</returns>
        [HttpPost("update")]
        [ProducesResponseType(typeof(Int32), 200)]
        public async Task<IActionResult> Update([FromBody] UpdateDatabaseConnectionDto input)
        {
            var result = await _databaseConnectionService.UpdateAsync(input);
            return Ok(result);
        }

        /// <summary>
        /// 删除数据库连接
        /// </summary>
        /// <returns>返回受影响的行数</returns>
        [HttpDelete("delete/{connectionId}")]
        [ProducesResponseType(typeof(string), 200, "application/text")]
        public async Task<IActionResult> Delete(int connectionId)
        {
            var result = await _databaseConnectionService.DeleteAsync(connectionId);
            return Ok("删除成功");
        }
    }
}
