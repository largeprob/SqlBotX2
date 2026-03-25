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
    /// table structure repository
    /// </summary>
    public class TableStructureRepository : ITableStructureRepository
    {
        private readonly SqlBotxDBContext _dbContext;

        public TableStructureRepository(SqlBotxDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get IQueryable
        /// </summary>
        /// <returns><see cref="IQueryable<TableStructure>"/></returns>
        public IQueryable<TableStructure> IQueryable()
        {
            return _dbContext.Set<TableStructure>();
        }

        /// <summary>
        /// Get List of TableStructure
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        public async Task<List<TableStructure>> ListAsync(Func<IQueryable<TableStructure>, IQueryable<TableStructure>>? includeFunc = null)
        {
            var query = _dbContext.Set<TableStructure>().AsQueryable();
            if (includeFunc != null)
            {
                query = includeFunc(query);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Insert (with cascade fields)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task InsterAsync(TableStructure entity)
        {
            await _dbContext.Set<TableStructure>().AddAsync(entity);
        }

        /// <summary>
        /// Update (with cascade fields - delete old fields and add new ones)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateAsync(TableStructure entity)
        {
            // First, delete existing fields for this table
            await _dbContext.Set<TableStructureColumn>()
                .Where(x => x.TableId == entity.TableId)
                .ExecuteDeleteAsync();

            // Update table structure
            await _dbContext.Set<TableStructure>()
                .Where(x => x.TableId == entity.TableId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.ConnectionId, entity.ConnectionId)
                    .SetProperty(x => x.TableName, entity.TableName)
                    .SetProperty(x => x.Alias, entity.Alias)
                    .SetProperty(x => x.FieldCount, entity.FieldCount)
                    .SetProperty(x => x.Description, entity.Description)
                );

            // Add new fields
            foreach (var field in entity.Columns)
            {
                field.TableId = entity.TableId;
            }
            await _dbContext.Set<TableStructureColumn>().AddRangeAsync(entity.Columns);
        }

        /// <summary>
        /// Delete (cascade delete fields)
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public async Task  DeleteAsync(int tableId)
        {
            // Cascade delete will handle fields via EF Core configuration
            await _dbContext.Set<TableStructure>()
               .Where(x => x.TableId == tableId)
               .Include(x => x.Columns)
               .ExecuteDeleteAsync();
        }
    }
}
