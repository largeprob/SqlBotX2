using SqlBoTx.Net.Application.Contracts.DatabaseConnections.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.Application.Contracts.DatabaseConnections
{
    public interface IDatabaseConnectionService
    {
        /// <summary>
        /// List all database connections
        /// </summary>
        /// <returns></returns>
        Task<List<ListDatabaseConnectionDto>> ListAsync();

        /// <summary>
        /// List all database connections with their tables
        /// </summary>
        /// <returns></returns>
        Task<List<ListDatabaseConnectionDto>> ListWithTablesAsync();

        /// <summary>
        /// Add a new database connection
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> AddAsync(AddDatabaseConnectionDto input);

        /// <summary>
        /// Update an existing database connection
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(UpdateDatabaseConnectionDto input);

        /// <summary>
        /// Delete  with related TableStructures and TableFields
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(int connectionId);
    }
}
