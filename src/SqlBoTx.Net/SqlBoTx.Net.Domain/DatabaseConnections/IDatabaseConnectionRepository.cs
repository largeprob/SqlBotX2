using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.Domain.DatabaseConnections
{
    public interface IDatabaseConnectionRepository
    {
        /// <summary>
        /// Get IQueryable
        /// </summary>
        /// <returns><see cref="IQueryable<DatabaseConnection>"/></returns>
        IQueryable<DatabaseConnection> IQueryable();

        /// <summary>
        ///  Exist check
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(int id);

        /// <summary>
        /// Get List of DatabaseConnection
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        Task<List<DatabaseConnection>> ListAsync(Func<IQueryable<DatabaseConnection>, IQueryable<DatabaseConnection>>? includeFunc = null);

        /// <summary>
        /// Inster
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> InsterAsync(DatabaseConnection entity);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(DatabaseConnection entity);

        /// <summary>
        /// Delete  with related TableStructures and TableFields
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(int connectionId);
    }
}
