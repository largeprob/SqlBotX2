using Microsoft.AspNetCore.Mvc;
using SqlBoTx.Net.Application.Contracts.TableStructures;
using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using SqlBoTx.Net.Core.Controller;
using System.ComponentModel;

namespace SqlBoTx.Net.ApiService.Controllers
{
    /// <summary>
    /// 数据库表结构管理
    /// </summary>
    [Route("[controller]")]
    [Consumes("application/json")]
    [Tags("数据库表结构管理")]
    public class TableStructureController : LarApi
    {
        private readonly ITableStructureService _tableStructureService;

        public TableStructureController(ITableStructureService tableStructureService)
        {
            _tableStructureService = tableStructureService;
        }

        /// <summary>
        /// 列表（所有表结构）
        /// </summary>
        /// <returns>返回数组</returns>
        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(List<ListTableStructureDto>))]
        public async Task<IActionResult> List()
        {
            var list = await _tableStructureService.ListAsync();
            return Ok(list);
        }

        /// <summary>
        /// 列表（按连接ID筛选）
        /// </summary>
        /// <param name="connectionId">数据库连接ID</param>
        /// <returns>返回数组</returns>
        [HttpGet("list-by-connection/{connectionId}")]
        [ProducesResponseType(200, Type = typeof(List<ListTableStructureDto>))]
        public async Task<IActionResult> ListByConnectionId(int connectionId)
        {
            var list = await _tableStructureService.ListByConnectionIdAsync(connectionId);
            return Ok(list);
        }

        /// <summary>
        /// 新增表结构
        /// </summary>
        /// <returns>返回受影响的行数</returns>
        [HttpPost("add")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Add([FromBody] AddTableStructureDto input)
        {
            await _tableStructureService.AddAsync(input);
            return Ok();
        }

        /// <summary>
        /// 更新表结构
        /// </summary>
        /// <returns>返回受影响的行数</returns>
        [HttpPost("update")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Update([FromBody] UpdateTableStructureDto input)
        {
            await _tableStructureService.UpdateAsync(input);
            return Ok();
        }

        /// <summary>
        /// 删除表结构
        /// </summary>
        /// <param name="tableId">表ID</param>
        /// <returns>返回受影响的行数</returns>
        [HttpDelete("delete/{tableId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(int tableId)
        {
            await _tableStructureService.DeleteAsync(tableId);
            return Ok();
        }
    }
}
