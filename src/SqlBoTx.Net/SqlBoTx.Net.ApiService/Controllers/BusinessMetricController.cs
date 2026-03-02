using Microsoft.AspNetCore.Mvc;
using SqlBoTx.Net.Application.Contracts.BusinessMetrics;
using SqlBoTx.Net.Application.Contracts.BusinessMetrics.Dtos;
using SqlBoTx.Net.Core.Controller;

namespace SqlBoTx.Net.ApiService.Controllers
{
    /// <summary>
    /// 业务指标管理
    /// </summary>
    [Route("[controller]")]
    [Consumes("application/json")]
    [Tags("业务指标管理")]
    public class BusinessMetricController : LarApi
    {
        private readonly IBusinessMetricService _businessMetricService;

        public BusinessMetricController(IBusinessMetricService businessMetricService)
        {
            _businessMetricService = businessMetricService;
        }

        /// <summary>
        /// 列表（所有业务指标）
        /// </summary>
        /// <returns>返回业务指标数组</returns>
        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(List<ListBusinessMetricDto>))]
        public async Task<IActionResult> List()
        {
            var list = await _businessMetricService.ListAsync();
            return Ok(list);
        }

        /// <summary>
        /// 新增业务指标
        /// </summary>
        /// <returns>返回操作结果</returns>
        [HttpPost("add")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Add([FromBody] AddBusinessMetricDto input)
        {
            await _businessMetricService.AddAsync(input);
            return Ok();
        }

        /// <summary>
        /// 更新业务指标
        /// </summary>
        /// <returns>返回操作结果</returns>
        [HttpPost("update")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Update([FromBody] UpdateBusinessMetricDto input)
        {
            await _businessMetricService.UpdateAsync(input);
            return Ok();
        }

        /// <summary>
        /// 删除业务指标
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>返回操作结果</returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(int id)
        {
            await _businessMetricService.DeleteAsync(id);
            return Ok();
        }
    }
}
