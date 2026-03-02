using Microsoft.AspNetCore.Mvc;
using SqlBoTx.Net.Application.Contracts.TableStructures;
using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using SqlBoTx.Net.Core.Controller;
using System.ComponentModel;

namespace SqlBoTx.Net.ApiService.Controllers
{
    /// <summary>
    /// 表关系维护管理
    /// </summary>
    [Route("[controller]")]
    [Consumes("application/json")]
    [Tags("表关系维护管理")]
    public class TableRelationshipController : LarApi
    {
        private readonly ITableRelationshipService _tableRelationshipService;

        public TableRelationshipController(ITableRelationshipService tableRelationshipService)
        {
            _tableRelationshipService = tableRelationshipService;
        }

        /// <summary>
        /// 获取指定源表的关系列表
        /// </summary>
        /// <param name="sourceTableId">源表ID</param>
        /// <returns>返回关系列表</returns>
        [HttpGet("list-by-source-table/{sourceTableId}")]
        [ProducesResponseType(200, Type = typeof(List<ListTableRelationshipDto>))]
        public async Task<IActionResult> ListBySourceTableId(int sourceTableId)
        {
            var list = await _tableRelationshipService.ListBySourceTableIdAsync(sourceTableId);
            return Ok(list);
        }

        /// <summary>
        /// 添加表关系
        /// </summary>
        /// <param name="inputs">添加关系参数</param>
        /// <returns>返回操作结果</returns>
        [HttpPost("add")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Add([FromBody] List<AddTableRelationshipDto> inputs)
        {
            await _tableRelationshipService.AddAsync(inputs);
            return Ok();
        }

        /// <summary>
        /// 更新表关系
        /// </summary>
        /// <param name="inputs">更新关系参数</param>
        /// <returns>返回操作结果</returns>
        [HttpPost("update")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Update([FromBody] List<AddTableRelationshipDto> inputs)
        {
            await _tableRelationshipService.AddAsync(inputs);
            return Ok();
        }

        /// <summary>
        /// 删除表关系
        /// </summary>
        /// <param name="sourceTableId">来源表ID</param>
        /// <returns>返回操作结果</returns>
        [HttpDelete("DeleteBySourceTableId/{sourceTableId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteBySourceTableId(int sourceTableId)
        {
            await _tableRelationshipService.DeleteBySourceTableIdAsync(sourceTableId);
            return Ok();
        }
    }
}
