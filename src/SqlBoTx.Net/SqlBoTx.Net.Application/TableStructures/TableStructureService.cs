using Mapster;
using Microsoft.EntityFrameworkCore;
using SqlBoTx.Net.Application.Contracts.TableStructures;
using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using SqlBoTx.Net.Domain;
using SqlBoTx.Net.Domain.TableFields;
using SqlBoTx.Net.Domain.TableRelationships;
using SqlBoTx.Net.Domain.TableStructures;

namespace SqlBoTx.Net.Application.TableStructures
{
    /// <summary>
    /// Service 用来处理 TableStructure 相关的应用逻辑
    /// </summary>
    public class TableStructureService : ITableStructureService
    {
        private readonly ITableStructureRepository _tableStructureRepository;
        private readonly TableStructureManager _tableStructureManager;
        private readonly IUnitOfWork _unitOfWork;

        public TableStructureService(ITableStructureRepository tableStructureRepository, TableStructureManager tableStructureManager, IUnitOfWork unitOfWork)
        {
            _tableStructureRepository = tableStructureRepository;
            _tableStructureManager = tableStructureManager;
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// List all table structures with fields
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListTableStructureDto>> ListAsync()
        {
            var list = await _tableStructureRepository.ListAsync(q => q.Include(x => x.TableFields));
            return list.Adapt<List<ListTableStructureDto>>();
        }

        /// <summary>
        /// List all table structures by connection ID with fields
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task<List<ListTableStructureDto>> ListByConnectionIdAsync(int connectionId)
        {
            var list = await _tableStructureRepository.ListAsync(q => q
                .Where(x => x.ConnectionId == connectionId)
                .Include(x => x.TableFields));
            return list.Adapt<List<ListTableStructureDto>>();
        }

        /// <summary>
        /// List all table structures with fields
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListTableStructureDto>> ListByIdAsync(int[] ids)
        {
            var list = await _tableStructureRepository.ListAsync(q => q.Where(x => ids.Contains(x.TableId)).Include(x => x.TableFields));
            return list.Adapt<List<ListTableStructureDto>>();
        }

        /// <summary>
        /// Add a new table structure with fields
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddAsync(AddTableStructureDto input)
        {
            await using (var uow = await _unitOfWork.BeginTransactionAsync())
            {
                var tableFields = input.TableFields.Adapt<List<TableField>>();

                var entity = await _tableStructureManager.CreateAsync(input.Adapt<TableStructure>(), tableFields);
                await _tableStructureRepository.InsterAsync(entity);

                await uow.CommitAsync();
            }
        }

        /// <summary>
        /// Update an existing table structure with fields
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateAsync(UpdateTableStructureDto input)
        {
            await using (var uow = await _unitOfWork.BeginTransactionAsync())
            {
                // Convert DTO fields to entities
                var tableFields = input.TableFields.Adapt<List<TableField>>();

                var entity = await _tableStructureManager.UpdateAsync(input.Adapt<TableStructure>(), tableFields);
                await _tableStructureRepository.UpdateAsync(entity);

                await uow.CommitAsync();
            }
        }

        /// <summary>
        /// Delete a table structure (will cascade delete fields)
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int tableId)
        {
             await _tableStructureRepository.DeleteAsync(tableId);
        }
    }
}
