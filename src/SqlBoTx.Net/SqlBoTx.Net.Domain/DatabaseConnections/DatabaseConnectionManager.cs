using SqlBoTx.Net.Share.Exceptions;
using SqlBoTx.Net.Share.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SqlBoTx.Net.Domain.DatabaseConnections
{
    /// <summary>
    /// Manager 用来执行 DatabaseConnection 相关的业务逻辑
    /// </summary>
    public class DatabaseConnectionManager
    {
        private readonly IDatabaseConnectionRepository _databaseConnectionRepository;

        public DatabaseConnectionManager(IDatabaseConnectionRepository databaseConnectionRepository)
        {
            _databaseConnectionRepository = databaseConnectionRepository;
        }

        /// <summary>
        /// Exist check
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IsExist(int id)
        {
            return await _databaseConnectionRepository.ExistAsync(id);
        }

        /// <summary>
        /// Encrypt the database user password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string EncryptPassword(string password)
        {
            var key = Environment.GetEnvironmentVariable("DBAESKEY")
            .ThrowIfNull();
            var iv = Environment.GetEnvironmentVariable("DBAESIV")
            .ThrowIfNull();
            return AesEncryption.Encrypt(password, key, iv);
        }

        /// <summary>
        /// Create a new database connection
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DatabaseConnection> CreateAsync(DatabaseConnection input)
        {
            input.CreatedDate = DateTime.Now;
            input.UserPassword = EncryptPassword(input.UserPassword!);
            return input;
        }

        /// <summary>
        /// Update an existing database connection
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DatabaseConnection> UpdateAsync(DatabaseConnection input)
        {
            input.LastModifiedDate = DateTime.Now;
            input.UserPassword = EncryptPassword(input.UserPassword!);
            return input;
        }
    }
}
