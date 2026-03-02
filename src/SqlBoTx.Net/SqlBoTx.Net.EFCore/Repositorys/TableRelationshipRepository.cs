using Microsoft.EntityFrameworkCore;
using SqlBoTx.Net.Domain.TableRelationships;
using SqlBoTx.Net.Domain.TableStructures;
using SqlBoTx.Net.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlBoTx.Net.EFCore.Repositorys
{
    /// <summary>
    /// table relationship repository
    /// </summary>
    public class TableRelationshipRepository : ITableRelationshipRepository
    {
        private readonly SqlBotxDBContext _dbContext;

        public TableRelationshipRepository(SqlBotxDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get IQueryable
        /// </summary>
        /// <returns><see cref="IQueryable<TableRelationship>"/></returns>
        public IQueryable<TableRelationship> IQueryable()
        {
            return _dbContext.Set<TableRelationship>();
        }

        /// <summary>
        /// Get List of TableRelationship
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        public async Task<List<TableRelationship>> ListAsync(Func<IQueryable<TableRelationship>, IQueryable<TableRelationship>>? includeFunc = null)
        {
            var query = _dbContext.Set<TableRelationship>().AsQueryable();
            if (includeFunc != null)
            {
                query = includeFunc(query);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Get List by Source Table ID
        /// </summary>
        /// <param name="sourceTableId"></param>
        /// <returns></returns>
        public async Task<List<TableRelationship>> ListBySourceTableIdAsync(int sourceTableId)
        {
            return await _dbContext.Set<TableRelationship>()
                .Where(x => x.SourceTableId == sourceTableId)
                .Include(x => x.TargetTable)
                .Include(x => x.SourceTable)   
                .ToListAsync();
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task InsertAsync(List<TableRelationship> entity)
        {
           await _dbContext.Set<TableRelationship>().AddRangeAsync(entity);
        }

        /// <summary>
        /// Delete by Source Table ID
        /// </summary>
        /// <param name="sourceTableId"></param>
        /// <returns></returns>
        public async Task<int> DeleteBySourceTableIdAsync(int sourceTableId)
        {
            return await _dbContext.Set<TableRelationship>()
                .Where(x => x.SourceTableId == sourceTableId)
                .ExecuteDeleteAsync();
        }
    }
}
