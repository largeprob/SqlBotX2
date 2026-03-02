using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives.Dtos;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives.Embeddings;
using SqlBoTx.Net.Core.Controller;

namespace SqlBoTx.Net.ApiService.Controllers
{
    /// <summary>
    /// 业务目标管理
    /// </summary>
    [Route("[controller]")]
    [Consumes("application/json")]
    [Tags("业务目标管理")]
    public class BusinessObjectiveController : LarApi
    {
        private readonly IBusinessObjectiveService _businessObjectiveService;
        private readonly Kernel _kernel;

        public BusinessObjectiveController(IBusinessObjectiveService businessObjectiveService, Kernel kernel)
        {
            _businessObjectiveService = businessObjectiveService;
            _kernel = kernel;
        }

        /// <summary>
        /// 列表（所有业务目标）
        /// </summary>
        /// <returns>返回业务目标数组</returns>
        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(List<ListBusinessObjectiveDto>))]
        public async Task<IActionResult> List()
        {
            var list = await _businessObjectiveService.ListAsync();
            return Ok(list);
        }

        /// <summary>
        /// 新增业务目标
        /// </summary>
        /// <returns>返回操作结果</returns>
        [HttpPost("add")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Add([FromBody] AddBusinessObjectiveDto input)
        {
            await _businessObjectiveService.AddAsync(input);
            return Ok();
        }

        /// <summary>
        /// 更新业务目标
        /// </summary>
        /// <returns>返回操作结果</returns>
        [HttpPost("update")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Update([FromBody] UpdateBusinessObjectiveDto input)
        {
            await _businessObjectiveService.UpdateAsync(input);
            return Ok();
        }

        /// <summary>
        /// 删除业务目标
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>返回操作结果</returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(int id)
        {
            await _businessObjectiveService.DeleteAsync(id);
            return Ok();
        }
 
    }
}
