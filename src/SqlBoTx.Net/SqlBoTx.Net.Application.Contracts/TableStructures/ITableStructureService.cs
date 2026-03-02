using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.Application.Contracts.TableStructures
{
    public interface ITableStructureService
    {
        /// <summary>
        /// List all table structures
        /// </summary>
        /// <returns></returns>
        Task<List<ListTableStructureDto>> ListAsync();

        /// <summary>
        /// List all table structures by connection ID
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        Task<List<ListTableStructureDto>> ListByConnectionIdAsync(int connectionId);

        /// <summary>
        /// List all table structures with fields
        /// </summary>
        /// <returns></returns>
        Task<List<ListTableStructureDto>> ListByIdAsync(int[] ids);

        /// <summary>
        /// Add a new table structure
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task AddAsync(AddTableStructureDto input);

        /// <summary>
        /// Update an existing table structure
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateAsync(UpdateTableStructureDto input);

        /// <summary>
        /// Delete a table structure
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        Task DeleteAsync(int tableId);
    }
}
