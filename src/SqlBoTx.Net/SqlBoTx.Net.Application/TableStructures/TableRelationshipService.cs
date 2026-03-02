using Mapster;
using Microsoft.EntityFrameworkCore;
using SqlBoTx.Net.Application.Contracts.TableStructures;
using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using SqlBoTx.Net.Domain;
using SqlBoTx.Net.Domain.TableRelationships;
using SqlBoTx.Net.Domain.TableStructures;
using SqlBoTx.Net.Share.Exceptions;
using System;
using System.Linq;

namespace SqlBoTx.Net.Application.TableStructures
{
    /// <summary>
    /// 表关系服务实现
    /// </summary>
    public class TableRelationshipService : ITableRelationshipService
    {
        private readonly ITableRelationshipRepository _tableRelationshipRepository;
        private readonly ITableStructureRepository _tableStructureRepository;
        private readonly IUnitOfWork  _unitOfWork;

        public TableRelationshipService(ITableRelationshipRepository tableRelationshipRepository, ITableStructureRepository tableStructureRepository, IUnitOfWork unitOfWork)
        {
            _tableRelationshipRepository = tableRelationshipRepository;
            _tableStructureRepository = tableStructureRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获取指定表的所有关系
        /// </summary>
        /// <param name="tableIds">表Id</param>
        /// <returns></returns>
        public async Task<List<ListTableRelationshipDto>> ListByTableIdAsync(int[] tableIds)
        {
            var relationships = await _tableRelationshipRepository.ListAsync((p) => p.Where(x =>
                tableIds.Contains(x.SourceTableId) ||
                tableIds.Contains(x.TargetTableId)
            ).Include(x => x.SourceTable).Include(x => x.TargetTable));

            return relationships.Select(r => new ListTableRelationshipDto
            {
                Id = r.Id,
                SourceTableId = r.SourceTableId,
                TargetTableId = r.TargetTableId,
                RelationshipType = r.RelationshipType,
                JoinConditions = r.Conditions,
                SourceTableName = r.SourceTable?.TableName,
                SourceTableDisplayName = r.SourceTable?.DisplayName,
                TargetTableName = r.TargetTable?.TableName,
                TargetTableDisplayName = r.TargetTable?.DisplayName
            }).ToList();
        }

        /// <summary>
        /// 获取指定源表的所有关系列表
        /// </summary>
        /// <param name="sourceTableId">源表ID</param>
        /// <returns></returns>
        public async Task<List<ListTableRelationshipDto>> ListBySourceTableIdAsync(int sourceTableId)
        {
            var relationships = await _tableRelationshipRepository.ListBySourceTableIdAsync(sourceTableId);

            return relationships.Select(r => new ListTableRelationshipDto
            {
                Id = r.Id,
                SourceTableId = r.SourceTableId,
                TargetTableId = r.TargetTableId,
                RelationshipType = r.RelationshipType,
                JoinConditions = r.Conditions,
                SourceTableName = r.SourceTable?.TableName,
                SourceTableDisplayName = r.SourceTable?.DisplayName,
                TargetTableName = r.TargetTable?.TableName,
                TargetTableDisplayName = r.TargetTable?.DisplayName
            }).ToList();
        }

        /// <summary>
        /// 添加表关系
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddAsync(List<AddTableRelationshipDto> inputs)
        {
            var sourceTableId = inputs.First().SourceTableId;

            // 验证源表是否存在
            var sourceTableExists = await _tableStructureRepository.IQueryable()
                .AnyAsync(x => x.TableId == sourceTableId);
            if (!sourceTableExists)
            {
                throw new BusinessException("TableRelationship001", $"源表不存在");
            }

            var entity = inputs.Adapt<List<TableRelationship>>();

            await using var uow = await _unitOfWork.BeginTransactionAsync();

            await _tableRelationshipRepository.DeleteBySourceTableIdAsync(sourceTableId.Value);
            await _tableRelationshipRepository.InsertAsync(entity);

            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 删除表关系
        /// </summary>
        /// <param name="relationshipId">关系ID</param>
        /// <returns></returns>
        public async Task DeleteBySourceTableIdAsync(int relationshipId)
        {
            await using var uow = await _unitOfWork.BeginTransactionAsync();

            await _tableRelationshipRepository.DeleteBySourceTableIdAsync(relationshipId);

            await _unitOfWork.CommitAsync();
        }
    }
}
