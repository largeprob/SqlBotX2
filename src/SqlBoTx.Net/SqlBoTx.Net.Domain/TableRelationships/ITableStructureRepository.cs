using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.Domain.TableRelationships
{
    public interface ITableStructureRepository
    {
        /// <summary>
        /// Get IQueryable
        /// </summary>
        /// <returns><see cref="IQueryable<TableStructure>"/></returns>
        IQueryable<TableStructure> IQueryable();

        /// <summary>
        /// Get List of TableStructure
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        Task<List<TableStructure>> ListAsync(Func<IQueryable<TableStructure>, IQueryable<TableStructure>>? includeFunc = null);

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsterAsync(TableStructure entity);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(TableStructure entity);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        Task DeleteAsync(int tableId);
    }
}
