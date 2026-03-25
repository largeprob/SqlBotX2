using Microsoft.EntityFrameworkCore;
using SqlBoTx.Net.Domain.DatabaseConnections;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlBoTx.Net.EFCore.Repositorys
{
    /// <summary>
    /// database connection repository
    /// </summary>
    public class DatabaseConnectionRepository : IDatabaseConnectionRepository
    {
        private readonly SqlBotxDBContext _dbContext;

        public DatabaseConnectionRepository(SqlBotxDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get IQueryable
        /// </summary>
        /// <returns><see cref="IQueryable<DatabaseConnection>"/></returns>
        public IQueryable<DatabaseConnection> IQueryable()
        {
            return _dbContext.Set<DatabaseConnection>();
        }

        /// <summary>
        ///  Exist check
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> ExistAsync(int id)
        {
            var entity = await _dbContext.Set<DatabaseConnection>().FirstOrDefaultAsync(x => x.Id == id);
            return entity != null;
        }

        /// <summary>
        /// Get List of DatabaseConnection
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        public async Task<List<DatabaseConnection>> ListAsync(Func<IQueryable<DatabaseConnection>, IQueryable<DatabaseConnection>>? includeFunc = null)
        {
            var query = _dbContext.Set<DatabaseConnection>().AsQueryable();
            if (includeFunc != null)
            {
                query = includeFunc(query);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Inster
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> InsterAsync(DatabaseConnection entity)
        {
            _dbContext.Set<DatabaseConnection>().Add(entity);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(DatabaseConnection entity)
        {
            return
            await _dbContext.Set<DatabaseConnection>()
             .Where(x => x.Id == entity.Id)
             .ExecuteUpdateAsync(s => s
                 .SetProperty(x => x.ConnectionName, entity.ConnectionName)
                 .SetProperty(x => x.ConnectionType, entity.ConnectionType)
                 .SetProperty(x => x.ConnectionString, entity.ConnectionString)
                 .SetProperty(x => x.UserName, entity.UserName)
                 .SetProperty(x => x.UserPassword, entity.UserPassword)
                 .SetProperty(x => x.Description, entity.Description)
                 .SetProperty(x => x.LastModifiedDate, DateTime.Now)
             );
        }

        /// <summary>
        /// Delete  with related TableStructures and TableFields
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int connectionId)
        {
            return
            await _dbContext.Set<DatabaseConnection>()
             .Where(x => x.Id == connectionId)
             .Include(x => x.TableStructures)
             .ThenInclude(x=>x.Columns)
             .ExecuteDeleteAsync();
        }
    }
}
