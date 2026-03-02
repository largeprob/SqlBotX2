using SqlBoTx.Net.Domain.DatabaseConnections;
using SqlBoTx.Net.Domain.TableFields;
using SqlBoTx.Net.Share.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.Domain.TableStructures
{
    /// <summary>
    /// Manager 用来执行 TableStructure 相关的业务逻辑
    /// </summary>
    public class TableStructureManager
    {
        private readonly IDatabaseConnectionRepository _databaseConnectionRepository;

        public TableStructureManager(IDatabaseConnectionRepository databaseConnectionRepository)
        {
            _databaseConnectionRepository = databaseConnectionRepository;
        }

        /// <summary>
        /// Create a new table structure with fields
        /// </summary>
        /// <param name="input"></param>
        /// <param name="tableFields"></param>
        /// <returns></returns>
        public async Task<TableStructure> CreateAsync(TableStructure input, List<TableField> tableFields)
        {
            if (!await _databaseConnectionRepository.ExistAsync(input.ConnectionId))
            {
                throw new BusinessException("TableStructure001", $"连接不存在");
            }
            
            // Set the table fields
            input.TableFields = tableFields;

            // Set FieldCount based on the number of fields
            input.FieldCount = tableFields.Count;

            return input;
        }

        /// <summary>
        /// Update an existing table structure with fields
        /// </summary>
        /// <param name="input"></param>
        /// <param name="tableFields"></param>
        /// <returns></returns>
        public async Task<TableStructure> UpdateAsync(TableStructure input, List<TableField> tableFields)
        {
            // Update the table fields
            input.TableFields = tableFields;

            // Update FieldCount based on the number of fields
            input.FieldCount = tableFields.Count;

            return input;
        }
    }
}
