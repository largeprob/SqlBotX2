using SqlBoTx.Net.Domain.TableRelationships;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.Domain.TableStructures
{
    public interface ITableRelationshipRepository
    {
        /// <summary>
        /// Get IQueryable
        /// </summary>
        /// <returns><see cref="IQueryable<TableRelationship>"/></returns>
        IQueryable<TableRelationship> IQueryable();

        /// <summary>
        /// Get List of TableRelationship
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        Task<List<TableRelationship>> ListAsync(Func<IQueryable<TableRelationship>, IQueryable<TableRelationship>>? includeFunc = null);

        /// <summary>
        /// Get List by Source Table ID
        /// </summary>
        /// <param name="sourceTableId"></param>
        /// <returns></returns>
        Task<List<TableRelationship>> ListBySourceTableIdAsync(int sourceTableId);

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsertAsync(List<TableRelationship> entity);

        /// <summary>
        /// Delete by Source Table ID
        /// </summary>
        /// <param name="sourceTableId"></param>
        /// <returns></returns>
        Task<int> DeleteBySourceTableIdAsync(int sourceTableId);
    }
}
