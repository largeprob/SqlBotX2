using Mapster;
using Microsoft.EntityFrameworkCore;
using SqlBoTx.Net.Application.Contracts.DatabaseConnections;
using SqlBoTx.Net.Application.Contracts.DatabaseConnections.Dtos;
using SqlBoTx.Net.Domain.DatabaseConnections;


namespace SqlBoTx.Net.Application.DatabaseConnections
{
    /// <summary>
    /// Service 用来处理 DatabaseConnection 相关的应用逻辑
    /// </summary>
    public class DatabaseConnectionService: IDatabaseConnectionService
    {
        private readonly IDatabaseConnectionRepository _databaseConnectionRepository;
        private readonly DatabaseConnectionManager _databaseConnectionManager;

        public DatabaseConnectionService(IDatabaseConnectionRepository databaseConnectionRepository, DatabaseConnectionManager databaseConnectionManager)
        {
            _databaseConnectionRepository = databaseConnectionRepository;
            _databaseConnectionManager = databaseConnectionManager;
        }

        /// <summary>
        /// List all database connections
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListDatabaseConnectionDto>> ListAsync()
        {
            var list = await _databaseConnectionRepository.ListAsync();
            return list.Adapt<List<ListDatabaseConnectionDto>>();
        }

        /// <summary>
        /// List all database connections with their tables
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListDatabaseConnectionDto>> ListWithTablesAsync()
        {
            var list = await _databaseConnectionRepository.ListAsync((p) => p.Include(x => x.TableStructures));
            return list.Adapt<List<ListDatabaseConnectionDto>>();
        }

        /// <summary>
        /// Add a new database connection
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(AddDatabaseConnectionDto input)
        {
            var entity = await _databaseConnectionManager.CreateAsync(input.Adapt<DatabaseConnection>());
            return await _databaseConnectionRepository.InsterAsync(entity);
        }

        /// <summary>
        /// Update an existing database connection
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(UpdateDatabaseConnectionDto input)
        {
            var entity = await _databaseConnectionManager.UpdateAsync(input.Adapt<DatabaseConnection>());
            return await _databaseConnectionRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// Delete  with related TableStructures and TableFields
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int connectionId)
        {
            return await _databaseConnectionRepository.DeleteAsync(connectionId);
        }
    } 
}
