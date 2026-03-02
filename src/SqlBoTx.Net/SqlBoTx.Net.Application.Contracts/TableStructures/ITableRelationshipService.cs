using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.Application.Contracts.TableStructures
{
    /// <summary>
    /// 表关系服务接口
    /// </summary>
    public interface ITableRelationshipService
    {
        /// <summary>
        /// 获取指定表的所有关系
        /// </summary>
        /// <param name="tableIds">表Id</param>
        /// <returns></returns>
        Task<List<ListTableRelationshipDto>> ListByTableIdAsync(int[] tableIds);

        /// <summary>
        /// 获取指定源表的所有关系列表
        /// </summary>
        /// <param name="sourceTableId">源表ID</param>
        /// <returns></returns>
        Task<List<ListTableRelationshipDto>> ListBySourceTableIdAsync(int sourceTableId);

        /// <summary>
        /// 添加表关系
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        Task AddAsync(List<AddTableRelationshipDto> inputs);

       
        /// <summary>
        /// 删除表关系
        /// </summary>
        /// <param name="relationshipId">关系ID</param>
        /// <returns></returns>
        Task DeleteBySourceTableIdAsync(int relationshipId);
    }
}
